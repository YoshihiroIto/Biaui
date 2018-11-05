using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Biaui.Internals
{
    internal static class Evaluator
    {
        internal static string Eval(string statement)
        {
            try
            {
                return _EvaluatorType.InvokeMember(
                    "Eval",
                    BindingFlags.InvokeMethod,
                    null,
                    _EvaluatorInstance,
                    new object[] {statement}
                ).ToString();
            }
            catch
            {
                return "";
            }
        }

        static Evaluator()
        {
            const string source =
                @"package Evaluator
                {
                   class Evaluator
                   {
                      public function Eval(expr : String) : String 
                      { 
                         return eval(expr); 
                      }
                   }
                }";

            var provider = CodeDomProvider.CreateProvider("JScript");
            var parameters = new CompilerParameters {GenerateInMemory = true};
            var results = provider.CompileAssemblyFromSource(parameters, source);
            var assembly = results.CompiledAssembly;

            _EvaluatorType = assembly.GetType("Evaluator.Evaluator");
            _EvaluatorInstance = Activator.CreateInstance(_EvaluatorType);
        }

        private static readonly Type _EvaluatorType;
        private static readonly object _EvaluatorInstance;
    }
}