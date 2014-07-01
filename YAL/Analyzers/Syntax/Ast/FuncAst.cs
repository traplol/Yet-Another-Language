
namespace YAL.Analyzers.Syntax.Ast
{
    class FuncAst : ExprAstBase
    {
        public PrototypeAst Proto { get; set; }
        public BlockScopeExprAst Body { get; set; }

        public FuncAst(PrototypeAst proto, BlockScopeExprAst body)
        {
            Type = ExprValueType.Func;
            Proto = proto;
            Body = body;
        }

        public override object Execute(Context<string, object> context)
        {
            Body.Parent = this;
            return Body.Execute(context);
        }
    }
}
