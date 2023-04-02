using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using doob.Scripter.Engine.Powershell.Cmdlets;

namespace doob.Scripter.Engine.Powershell
{
    internal class PsRunspace: IDisposable
    {
        private CustomPsHost PsHost { get; }
        private Runspace Runspace { get; }

        public PsRunspace()
        {

            var config = InitialSessionState.CreateDefault();
            SessionStateCmdletEntry requireModule = new SessionStateCmdletEntry(
                "Require-Module", typeof(RequireCommand), "");

            var requireAlias = new SessionStateAliasEntry("Require", "Require-Module");

            config.Commands.Add(requireModule);
            config.Commands.Add(requireAlias);

            PsHost = new CustomPsHost();
            
            Runspace = RunspaceFactory.CreateRunspace(PsHost, config);
            Runspace.DefaultRunspace = Runspace;
            Runspace.Open();
            
        }

        public PsRunspace SetVariable(string name, object value)
        {
            Runspace.SessionStateProxy.SetVariable(name, value);
            return this;
        }

        public object GetVariable(string name)
        {
            return Runspace.SessionStateProxy.GetVariable(name);
        }


        private object _psLock = new object();

        private Pipeline? _pipeline;

        public IEnumerable<PSObject>? ExecuteFunction(string functionName, IDictionary<string, object> arguments)
        {


            var comm = new Command(functionName);
            foreach (var argument in arguments)
            {
                comm.Parameters.Add(argument.Key, argument.Value);
            }

            return Invoke(comm);
        }

        public IEnumerable<PSObject>? ExecuteFunction(string functionName, params object[] arguments)
        {

            
            var comm = new Command(functionName);
            foreach (var argument in arguments)
            {
                comm.Parameters.Add(null, argument);
            }

            return Invoke(comm);
        }

        public IEnumerable<PSObject>? Invoke(string command)
        {

            var comm = new Command(command, true);
            return Invoke(comm);

        }

        public IEnumerable<PSObject>? Invoke(Command command)
        {
            lock (_psLock)
            {

                try
                {
                    
                    var ass = this.GetType().Assembly.Location;
                    _pipeline = Runspace.CreatePipeline();
                    _pipeline.Commands.Add(command);
                    
                    var ret = _pipeline.Invoke();
                    var errorList = new List<string>();
                    if (_pipeline.Error.Count > 0)
                    {
                        while (!_pipeline.Error.EndOfPipeline)
                        {
                            if (_pipeline.Error.Read() is PSObject value)
                            {
                                if (value.BaseObject is ErrorRecord r)
                                {
                                    errorList.Add(
                                        $"{r.Exception.Message}{Environment.NewLine}{r.InvocationInfo.PositionMessage}");
                                }
                            }
                        }
                    }

                    if (errorList.Any())
                    {
                        throw new Exception(String.Join(Environment.NewLine, errorList));
                    }


                    return ret;
                }
                catch (RuntimeException rex)
                {
                    throw new Exception($"{rex.ErrorRecord.Exception.Message}{Environment.NewLine}{rex.ErrorRecord.InvocationInfo.PositionMessage}");
                }
            }

        }

        public void Stop()
        {
            _pipeline?.StopAsync();
        }

        public void Dispose()
        {
            _pipeline?.Dispose();
            Runspace?.Dispose();
        }
    }
}
