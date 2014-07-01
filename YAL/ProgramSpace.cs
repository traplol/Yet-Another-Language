using System;
using System.Collections.Generic;
using YAL.Analyzers.Syntax;
using YAL.Analyzers.Syntax.Ast;

namespace YAL
{
    class ProgramSpace
    {
        public static Context<string, object> GsVars;
        public static Dictionary<string, FuncAst> GsFuncs;
        public static List<IExprAst> TopLevelExpressions;
        public static List<KeyValuePair<ExpressionType, IExprAst>> Program;
        public static bool ParsingSuccess = true;
        public static void Setup(List<KeyValuePair<ExpressionType, IExprAst>> program)
        {
            if (ParsingSuccess)
                Console.WriteLine("\r\nParsing was successful.\r\nPress any key to execute...");
            else
                Console.WriteLine("\r\nParsing was unsuccessful.\r\nIf you want to attempt execution, press any key...");
            Console.ReadKey();
            Console.Clear();
            Program = program;
            TopLevelExpressions = new List<IExprAst>();
            GsVars = new Context<string,object>();
            GsFuncs = new Dictionary<string, FuncAst>();

            foreach (var e in Program)
            {
                if (e.Key == ExpressionType.Definition)
                {
                    StoreGlobalScopeDefinition(e.Value);
                }
                else
                {
                    StoreTopLevelExpressions(e.Value);
                }
            }
        }

        public static void Execute()
        {
            foreach (var e in TopLevelExpressions)
            {
                e.Execute(GsVars);
            }
        }

        private static void StoreTopLevelExpressions(IExprAst expr)
        {
            TopLevelExpressions.Add(expr);
        }

        private static void StoreGlobalScopeDefinition(IExprAst expr)
        {
            var val = expr;
            if (val.Type == ExprValueType.Var)
            {
                var @var = val as VarAst;
                GsVars.PushBack(@var.Name, var.Execute(GsVars));
            }
            else if (val.Type == ExprValueType.Func)
            {
                var func = val as FuncAst;
                GsFuncs.Add(func.Proto.Name, func);
            }
            else if (val.Type == ExprValueType.Def)
            {
                var def = val as DefAst;
                GsVars.PushBack(def.Name, def.Execute(GsVars));
            }
        }

    }
}
