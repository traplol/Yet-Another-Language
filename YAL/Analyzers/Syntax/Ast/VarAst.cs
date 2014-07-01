
namespace YAL.Analyzers.Syntax.Ast
{
    class VarAst : ExprAstBase
    {
        public string Name { get; set; }
        private IExprAst Value { get; set; }

        public VarAst(string name, IExprAst value)
        {
            Type = ExprValueType.Var;
            Name = name;
            Value = value;
            Value.Parent = this;
        }

        public override object Execute(Context<string, object> context)
        {
            return Value.Execute(context);
        }
    }
}
