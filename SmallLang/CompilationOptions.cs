using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallLang
{
    public enum CompilationOutputType
    {
        Exe,
        Dll
    }

    public struct CompilationOptions
    {
        public string AssemblyName { get; set; }
        public string ExeName { get; set; }
        public string OutputPath { get; set; }
        public string TypeName { get; set; }
        public string ModuleName { get; set; }
        public CompilationOutputType OutputType { get; set; }

        public CompilationOptions(string pAssembly, string pOutputPath, string pExe, CompilationOutputType pOutput)
        {
            AssemblyName = pAssembly;
            ExeName = pExe;
            OutputPath = pOutputPath;
            OutputType = pOutput;
            TypeName = "Program";
            ModuleName = "__main__";
        }
    }
}
