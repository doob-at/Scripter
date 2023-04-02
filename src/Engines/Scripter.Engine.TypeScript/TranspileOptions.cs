using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doob.Scripter.Engine.TypeScript
{
    internal class TranspileOptions
    {
        public CompilerOptions? compilerOptions { get; set; }
        public string? fileName { get; set; }
        public bool? reportDiagnostics { get; set; }
        public string? moduleName { get; set; }
        //renamedDependencies?: MapLike<string>;
        //transformers?: CustomTransformers;
    }

    public class CompilerOptions
    {
        public ScriptTarget? target { get; set; }
    }


    public enum ScriptTarget
    {
        ES3 = 0,
        ES5 = 1,
        ES2015 = 2,
        ES2016 = 3,
        ES2017 = 4,
        ES2018 = 5,
        ES2019 = 6,
        ES2020 = 7,
        ES2021 = 8,
        ES2022 = 9,
        ESNext = 99,
        JSON = 100,
        Latest = ESNext,
    }
}
