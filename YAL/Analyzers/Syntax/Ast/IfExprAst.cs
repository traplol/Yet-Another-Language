
namespace YAL.Analyzers.Syntax.Ast
{
    class IfExprAst : ExprAstBase
    {
        public IExprAst Condition { get; set; }
        public IExprAst IfBody { get; set; }
        public IExprAst ElseBody { get; set; }

        public IfExprAst(IExprAst condition, IExprAst ifBody, IExprAst elseBody)
        {
            Type = ExprValueType.If;
            Condition = condition;
            Condition.Parent = this;
            IfBody = ifBody;
            IfBody.Parent = this;
            ElseBody = elseBody;
            if (ElseBody != null)
                ElseBody.Parent = this;
        }

        public override object Execute(Context<string, object> context)
        {
            var condition = Condition.Execute(context);
            if (!(condition is bool))
                return false;

            if ((bool) condition)
            {
                return IfBody.Execute(context);
            }
            if (ElseBody != null && !(bool)condition)
            {
                return ElseBody.Execute(context);
            }

            return null;
        }
    }
}
