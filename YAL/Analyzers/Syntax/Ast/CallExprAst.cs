using System;
using System.Collections.Generic;

namespace YAL.Analyzers.Syntax.Ast
{
    class CallExprAst : ExprAstBase
    {
        public string Name { get; set; }
        public List<IExprAst> Args { get; set; }

        public CallExprAst(string name, List<IExprAst> args)
        {
            Type = ExprValueType.Call;
            Name = name;
            Args = args;
            foreach (var s in Args)
                s.Parent = this;
        }

        public override object Execute(Context<string, object> context)
        {
            if (Name == "print") // TODO: Not so hacky way to implement STL functions.
            {
                return PrintF(context);
            }

            if (!ProgramSpace.GsFuncs.ContainsKey(Name))
            {
                Console.WriteLine("function '{0}' is undefined.", Name);
                return null;
            }
            var func = ProgramSpace.GsFuncs[Name];

            int pushed = 0;
            for (int i = 0; i < Args.Count && i < func.Proto.Args.Count; ++i)
            {
                context.PushBack(func.Proto.Args[i], Args[i].Execute(context));
                pushed++;
            }

            var result = func.Execute(context);
            context.RemoveLast(pushed);
            return result;
        }

        private object PrintF(Context<string, object> context)
        {
            if (Args.Count == 0)
                return null;

            // TODO: first arg should be format string instead of generating one.
            var results = new object[Args.Count];


            string fmt = "" + Args[0].Execute(context);
            for (int i = 1; i < Args.Count; ++i)
            {
                results[i - 1] = Args[i].Execute(context);
            }
            Console.WriteLine(fmt, results);
            return null;
        }


    }
}
