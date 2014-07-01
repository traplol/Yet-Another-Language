using System;

namespace YAL.Analyzers.Syntax.Ast
{
    class WhileExprAst : ExprAstBase
    {
        public IExprAst Condition { get; set; }
        public IExprAst Body { get; set; }

        public WhileExprAst(IExprAst condition, IExprAst body)
        {
            Type = ExprValueType.While;
            Condition = condition;
            Condition.Parent = this;
            Body = body;
            Body.Parent = this;
        }

        public override object Execute(Context<string, object> context)
        {
            while ((bool)Condition.Execute(context))
            {
                if (Returning)
                    return Body.Execute(context);
                Body.Execute(context);
            }
            return null;
        }
    }
}
