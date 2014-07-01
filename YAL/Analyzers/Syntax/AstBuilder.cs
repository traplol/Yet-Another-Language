using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAL.Analyzers.Lexical;
using YAL.Analyzers.Syntax.Ast;

namespace YAL.Analyzers.Syntax
{
    enum ExpressionType
    {
        TopLevel,
        Definition,
    }
    partial class AstBuilder
    {
        private readonly Lexer _lexer;
        private Token _curTok;
        private List<KeyValuePair<ExpressionType, IExprAst>> _program;

        public AstBuilder(Lexer lexer)
        {
            _program = new List<KeyValuePair<ExpressionType, IExprAst>>();
            _lexer = lexer;
        }

        public void Build()
        {
            Next(); // get the first token
            while (_curTok != null)
            {
                switch (_curTok.Type)
                {
                    default: Console.WriteLine("Unexpected token type: {0} with value '{1}' at L{2}:C{3}",
                        _curTok.Type, _curTok.Value, _curTok.LineNumber, _curTok.ColumnNumber);
                        ProgramSpace.ParsingSuccess = false;
                        Next();
                        break;
                    case TokenType.Func:
                    case TokenType.Def:
                    case TokenType.Var:
                    case TokenType.Array:
                        HandleDefinition();
                        break;
                    case TokenType.Identifier:
                    case TokenType.BooleanLiteral:
                    case TokenType.NumberLiteral:
                    case TokenType.StringLiteral:
                        HandleTopLevelExpression();
                        break;
                }
            }
            ProgramSpace.Setup(_program);
            ProgramSpace.Execute();
        }

        private int OperPrecedence()
        {
            return _curTok == null ? -1 : _curTok.OperatorPrecedence;
        }

        private void Next()
        {
            
            _curTok = _lexer.GetToken();
            if (_curTok == null)
            {
                
            }
        }

        private void HandleDefinition()
        {
            var infotok = _curTok;
            var expression = ParseDefine();

            if (expression != null)
            {
                //Console.WriteLine("Parsed '{0}' definition.", infotok.Type);
                _program.Add(new KeyValuePair<ExpressionType, IExprAst>(ExpressionType.Definition, expression));
            }
            else
            {
                ProgramSpace.ParsingSuccess = false;
                Console.WriteLine("Failed to parse '{0}' definition at L{1}:{2}", 
                    infotok.Type, infotok.LineNumber, infotok.ColumnNumber);
            }
        }

        private void HandleTopLevelExpression()
        {
            var infotok = _curTok;
            var topLevelExpr = ParseTopLevelExpr();

            if (topLevelExpr != null)
            {
                //Console.WriteLine("Parsed '{0}' top level expression.", infotok.Type);
                _program.Add(new KeyValuePair<ExpressionType, IExprAst>(ExpressionType.TopLevel, topLevelExpr));
            }
            else
            {
                ProgramSpace.ParsingSuccess = false;
                Console.WriteLine("Failed to parse '{0}' top level expression at L{1}:{2}",
                    infotok.Type, infotok.LineNumber, infotok.ColumnNumber);
            }
        }


        private IExprAst LogError(TokenType expected)
        {
            return LogError<IExprAst>(expected);
        }

        private T LogError<T>(TokenType expected)
        {
            ProgramSpace.ParsingSuccess = false;
            if (_curTok == null)
            {
                Console.WriteLine("Error: Expected {0}, found (null) token. Possible incomplete statement?\r\n{1}", expected, typeof(T).Name);
                //throw new Exception();
                return default(T);
            }


            Console.WriteLine("Error: Expected {0}, found {1} with value {2} at L{3}:{4}\r\n{5}",
                expected, _curTok.Type, _curTok.Value, _curTok.LineNumber, _curTok.ColumnNumber, typeof(T).Name);
            Next();
            return default(T);
        }
    }
}
