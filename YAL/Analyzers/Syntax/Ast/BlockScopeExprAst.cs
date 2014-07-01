using System.Collections.Generic;

namespace YAL.Analyzers.Syntax.Ast
{
    class BlockScopeExprAst : ExprAstBase
    {
        public List<IExprAst> Statements { get; set; }

        public BlockScopeExprAst(List<IExprAst> statements)
        {
            Type = ExprValueType.BlockScope;
            Statements = statements;
            foreach (var s in statements)
                s.Parent = this;
        }


        public override object Execute(Context<string, object> context)
        {
            int pushed = 0;
            foreach (var s in Statements)
            {
                object ret;
                if (s.Type == ExprValueType.Var || s.Type == ExprValueType.Def)
                {
                    context.PushBack(((VarAst)s).Name, s.Execute(context));
                    pushed++;
                }
                ret = s.Execute(context);
                if (Returning && pushed > 0)
                {
                    context.RemoveLast(pushed);
                    return ret;
                }
                if (Returning)
                {
                    return ret;
                }
            }
            return null;
        }
    }
}
