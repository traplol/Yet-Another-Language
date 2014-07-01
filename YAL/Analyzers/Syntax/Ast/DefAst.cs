
namespace YAL.Analyzers.Syntax.Ast
{
    class DefAst : ExprAstBase
    {
        public string Name { get; set; }
        public IExprAst Value { get; set; }

        public DefAst(string name,  IExprAst value)
        {
            Type = ExprValueType.Def;
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
