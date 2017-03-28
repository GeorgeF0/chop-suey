using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ChopSuey
{
    public class LambdaCompiler
    {
        public static Init CreateInit(string str) => 
            CSharpScript.EvaluateAsync<Init>(str, ScriptOptions.Default.WithReferences(typeof(Init).Assembly).WithImports("System.Collections.Generic")).Result;

        public static Aggregate CreateLambda(string str) => 
            CSharpScript.EvaluateAsync<Aggregate>(str, ScriptOptions.Default.WithReferences(typeof(Init).Assembly).WithImports("System.Collections.Generic")).Result;
    }
}
