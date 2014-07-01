
namespace YAL.Analyzers.Syntax.Ast
{
    class ReturnExprAst : ExprAstBase
    {
        public IExprAst Expr { get; set; }

        public ReturnExprAst(IExprAst expr)
        {
            Type = ExprValueType.Return;
            Expr = expr;
            Expr.Parent = this;
        }

        public override object Execute(Context<string, object> context)
        {
            var parent = Parent;
            while (parent != null)// setup the return short circuit.
            {
                parent.Returning = Returning = true;
                parent = parent.Parent; 
            }

            return Expr.Execute(context);
        }
    }
}
