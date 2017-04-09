using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ChopSuey
{
    public class LambdaCompiler
    {
        private static ScriptOptions Options =>
            ScriptOptions.Default
            .AddReferences(Assembly.GetExecutingAssembly(), Assembly.Load("MoreLinq"))
            .AddReferences("System")
            .AddImports("System", "System.Collections.Generic", "System.Linq", "System.Dynamic", "System.Text.RegularExpressions", "MoreLinq");

        public static Init CreateInit(string str) => CSharpScript.EvaluateAsync<Init>($"() => {str}", Options).Result;

        public static Aggregate CreateLambda(string str) =>
            CSharpScript.EvaluateAsync<Aggregate>($"(Streak.Core.Event e, dynamic d, ref dynamic s) => {{{str}}}", Options).Result;
    }
}
