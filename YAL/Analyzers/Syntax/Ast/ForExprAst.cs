using System;

namespace YAL.Analyzers.Syntax.Ast
{
    class ForExprAst : ExprAstBase
    {
        public IExprAst InitialExpr { get; set; }
        public IExprAst ConditionalExpr { get; set; }
        public IExprAst Update { get; set; }
        public IExprAst Body { get; set; }

        public ForExprAst(IExprAst initialExpr, IExprAst conditionalExpr, IExprAst update, IExprAst body)
        {
            Type = ExprValueType.For;
            InitialExpr = initialExpr;
            InitialExpr.Parent = this;
            ConditionalExpr = conditionalExpr;
            ConditionalExpr.Parent = this;
            Update = update;
            Update.Parent = this;
            Body = body;
            Body.Parent = this;
        }

        public override object Execute(Context<string, object> context)
        {
            int pushed = 0;
            if (InitialExpr.Type == ExprValueType.Var) // new var
            {
                pushed++;
                var name = ((VarAst) InitialExpr).Name;
                context.PushBack(name, InitialExpr.Execute(context));
            }
            else // var might exist outside of scope of for loop
            {
                InitialExpr.Execute(context);
            }

            while ((bool) ConditionalExpr.Execute(context))
            {
                if (Returning)
                {
                    if (pushed > 0)
                    {
                        context.RemoveLast(pushed);
                    }
                    return Body.Execute(context);
                }
                Body.Execute(context);
                Update.Execute(context);
            }


            if (pushed > 0)
            {
                context.RemoveLast(pushed);
            }
            return null;
        }
    }
}
