using System.Management.Automation;

namespace doob.Scripter.Engine.Powershell.Cmdlets
{
    [Cmdlet("Require","Module")]
    public class RequireCommand: PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string? Name { get; set; }

        protected override void ProcessRecord()
        {
            
            var registeredModules = (ScripterModulesProvider)this.GetVariableValue("ModulesProvider");
            WriteObject(registeredModules.GetModule(Name!));
        }

    }
}
