﻿using System;
using System.Collections.Generic;


namespace doob.Scripter.Shared
{
    public class ScriptFunction
    {
        private readonly IScriptEngine _scriptEngine;

        public string Name { get; }
        public Dictionary<string, Type> ParameterTypes { get; }

        public ScriptFunction(string name, Dictionary<string, Type> parameterTypes, IScriptEngine scriptEngine)
        {
            _scriptEngine = scriptEngine;
            Name = name;
            ParameterTypes = parameterTypes;
        }

        public object Invoke(params object[] parameters)
        {
            return _scriptEngine.InvokeFunction(Name, parameters);
        }
    }
}
