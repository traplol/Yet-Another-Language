using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAL.Analyzers.Lexical;
using YAL.Analyzers.Syntax.Ast;

namespace YAL.Analyzers.Syntax
{
    partial class AstBuilder
    {

        private IExprAst ParseDefine()
        {
            switch (_curTok.Type)
            {
                case TokenType.Def:
                    return ParseDefExpr();
                case TokenType.Var:
                    return ParseVarExpr();
                case TokenType.Func:
                    return ParseFunc();
                case TokenType.Array:
                    return ParseArrayAst();
            }
            return null;
        }

        // Note that these differ from a regular '<expression>' because they
        // are outside of a function scope and get executed top-down immediately
        // at runtime, before anything else is executed.
        //
        // <toplevelexpr>   ::= <expression> ';'
        private IExprAst ParseTopLevelExpr()
        {
            // to 'execute' these statements without a function call, we'll wrap them
            // in an anonymous function.
            var expr = ParseExpression();
            if (_curTok == TokenType.Semicolon)
                Next(); // eat ';'
            if (expr == null)
                return null;
            var proto = new PrototypeAst("", new List<string>());
            return new FuncAst(proto, new BlockScopeExprAst(new List<IExprAst>(new[]{expr})));
        }



        // <expression> ::= <primary> <binoprhs>
        //              |   ?<expression> ';'
        private IExprAst ParseExpression()
        {
            var lhs = ParsePrimary();
            if (_curTok == TokenType.Semicolon)
            {
                Next(); // eat ';'
                return lhs;
            }
                
            return lhs == null ? null : ParseBinOpRhs(0, ref lhs);
        }

        // <binoprhs>   ::= ( operator <primary> )
        private IExprAst ParseBinOpRhs(int precedence, ref IExprAst lhs)
        {
            IExprAst rhs;
            Token binOp = null;
            while (true)
            {
                var tokPrec = OperPrecedence();
                if (tokPrec < precedence)
                    return lhs;

                binOp = _curTok;
                Next(); // eat binop

                rhs = ParsePrimary();
                if (rhs == null)
                    return null;

                var nextPrec = OperPrecedence();
                if (tokPrec < nextPrec)
                {
                    rhs = ParseBinOpRhs(tokPrec + 1, ref rhs);
                    if (rhs == null) return null;
                }
                lhs = new BinaryOperatorExprAst(binOp.Value, lhs, rhs);
                if (_curTok == TokenType.Semicolon)
                {
                    Next();
                    break;
                }
                    
            }

            if (binOp == null)
                return null;
            // should only break on semicolon
            return lhs;
        }

        //<primary>     ::= <identifierexp>
        //              |   <parenexpr>
        //              |   <numberexpr>
        //              |   <booleanexpr>
        //              |   <stringexpr>
        //              |   <ifexpr>
        //              |   <forexpr>
        //              |   <whileexpr>
        //              |   <defexpr>
        //              |   <varexpr>
        //              |   <returnexpr>
        private IExprAst ParsePrimary()
        {
            if (_curTok == null)
                return null;
            if (_curTok == TokenType.Semicolon)
                Next();
            switch (_curTok.Type)
            {
                default: Console.WriteLine("Expected parsable expression type, got {0} at L{1}:C{2}", 
                    _curTok.Type, _curTok.LineNumber, _curTok.ColumnNumber);
                    ProgramSpace.ParsingSuccess = false;
                    return null;
                case TokenType.Identifier:
                    return ParseIdentifierExpr();
                case TokenType.LParen:
                    return ParseParenExpr();
                case TokenType.NumberLiteral:
                    return ParseNumberExpr();
                case TokenType.BooleanLiteral:
                    return ParseBooleanExpr();
                case TokenType.StringLiteral:
                    return ParseStringExpr();
                case TokenType.If:
                    return ParseIfExpr();
                case TokenType.For:
                    return ParseForExpr();
                case TokenType.While:
                    return ParseWhileExpr();
                case TokenType.Def:
                    return ParseDefExpr();
                case TokenType.Var:
                    return ParseVarExpr();
                case TokenType.Array:
                    return ParseArrayAst();
                case TokenType.Return:
                    return ParseReturnExpr();
            }
        }

        // <identifierexpr> ::= identifier
        // <callexpr>       ::= identifier '(' <expression>* ?( ',' <expression> )* ')'
        // <arrayaccessexpr>::= identifier '[' <expression> ']'
        private IExprAst ParseIdentifierExpr()
        {
            string name;
            List<IExprAst> args;

            if (_curTok != TokenType.Identifier)
                return LogError(TokenType.Identifier);

            name = _curTok.Value;
            Next(); // eat identifier

            // var reference
            if (_curTok != TokenType.LParen)
            {
                return new IdentifierExprAst(name);
            }

            if (_curTok == TokenType.LBracket) // array access
            {
                Next(); // eat '['

                var exp = ParseExpression();

                if (_curTok != TokenType.RBracket)
                    return LogError(TokenType.RBracket);
                Next();
            }

            // otherwise a call
            args = new List<IExprAst>();

            Next(); // eat '('

            while (true)
            {
                if (_curTok == TokenType.RParen)
                    break;
                var expr = ParseExpression();
                if (expr == null)
                    return null;
                args.Add(expr);
                if (_curTok == TokenType.RParen)
                    break;
                if (_curTok != TokenType.Comma)
                    return LogError(TokenType.Comma);
                Next(); // eat ','
            }
            if (_curTok != TokenType.RParen)
                return LogError(TokenType.RParen);
            Next(); // eat ')'
            
            return new CallExprAst(name, args);
        }

        // <parenexpr>      ::= '(' <expression> ')'
        private IExprAst ParseParenExpr()
        {
            if (_curTok != TokenType.LParen)
                return LogError(TokenType.LParen);

            Next(); // eat '('

            var expr = ParseExpression();
            if (expr == null)
                return null;

            if (_curTok != TokenType.RParen)
                return LogError(TokenType.RParen);
            Next(); // eat ')'
            return expr;
        }

        // <numberexpr>     ::= number
        private IExprAst ParseNumberExpr()
        {
            if (_curTok != TokenType.NumberLiteral)
                return LogError(TokenType.NumberLiteral);

            var num = _curTok.ValueAsDouble;
            Next(); // eat num
            if (num == (int)num) // 1.0 == 1 is true
                return new NumberExprAst((int)num);
            return new NumberExprAst(num);
        }

        // <booleanexpr>    ::= ( "true" | "false" )
        private IExprAst ParseBooleanExpr()
        {
            if (_curTok != TokenType.BooleanLiteral)
                return LogError(TokenType.BooleanLiteral);

            var @bool = _curTok.ValueAsBoolean;
            Next(); // eat boolean
            return new BooleanExprAst(@bool);
        }

        // The string token should be generated by the lexer, so no need to
        // parse anything here.
        // <stringexpr>     ::= '"' string '"'
        private IExprAst ParseStringExpr()
        {
            if (_curTok != TokenType.StringLiteral)
                return LogError(TokenType.StringLiteral);

            var @string = _curTok.Value;
            Next(); // eat string
            return new StringExprAst(@string);
        }

        // <ifexpr>         ::= 'if' <parenexpr> <blockscopeexpr> ?( 'else' <blockscopeexpr> )
        private IfExprAst ParseIfExpr()
        {
            IExprAst condition;
            IExprAst ifBody, elseBody;

            if (_curTok != TokenType.If)
                return LogError<IfExprAst>(TokenType.If);
            Next();// eat 'if'

            condition = ParseParenExpr();
            ifBody = _curTok.Type == TokenType.LBrace ? ParseExpressionBlock() : ParseExpression();
            //ifBody = ParseExpressionBlock();
            if (_curTok != TokenType.Else)
            {
                return new IfExprAst(condition, ifBody, null);
            }
            Next(); // eat 'else'

            //elseBody = ParseExpressionBlock();
            elseBody = _curTok.Type == TokenType.LBrace ? ParseExpressionBlock() : ParseExpression();
            return new IfExprAst(condition, ifBody, elseBody);
        }

        // <forexpr>        ::= 'for' '(' <expression> <expression> <expression> ')' <blockscopeexpr>
        private ForExprAst ParseForExpr()
        {
            IExprAst inital, conditional, update;
            IExprAst body;
            

            if (_curTok != TokenType.For)
                return LogError<ForExprAst>(TokenType.For);

            Next(); // eat for

            if (_curTok != TokenType.LParen)
                return LogError<ForExprAst>(TokenType.LParen);
            Next(); // eat '('

            inital = ParseExpression();
            if (inital == null) return null;
            
            conditional = ParseExpression();
            if (conditional == null) return null;
            
            update = ParseExpression();
            if (update == null) return null;

            if (_curTok != TokenType.RParen)
                return LogError<ForExprAst>(TokenType.RParen);
            Next(); // eat ')'

            //body = ParseExpressionBlock();
            body = _curTok.Type == TokenType.LBrace ? ParseExpressionBlock() : ParseExpression();
            if (body == null) return null;

            

            return new ForExprAst(inital, conditional, update, body);
        }

        // <whileexpr>      ::= 'while' <parenexpr> <blockscopeexpr>
        private WhileExprAst ParseWhileExpr()
        {
            IExprAst conditional;
            IExprAst body;

            if (_curTok != TokenType.While)
                LogError<WhileExprAst>(TokenType.While);

            Next(); // eat 'while'

            conditional = ParseParenExpr();
            if (conditional == null)
                return null;

            body = _curTok.Type == TokenType.LBrace ? ParseExpressionBlock() : ParseExpression();
            return body == null ? null : new WhileExprAst(conditional, body);
        }

        //<returnexpr>      ::= 'return' <expression>
        //                  |   'return '(' <expression> ')'
        private ReturnExprAst ParseReturnExpr()
        {
            IExprAst expr;

            if (_curTok != TokenType.Return)
                return LogError<ReturnExprAst>(TokenType.Return);

            Next(); // eat 'return'

            expr = _curTok == TokenType.LParen ? ParseParenExpr() : ParseExpression();

            return expr == null ? null : new ReturnExprAst(expr);
        }



        // <prototype>  ::= identifier '(' identifier, ... ')'
        private PrototypeAst ParsePrototype()
        {
            string name;
            var args = new List<string>();

            if (_curTok != TokenType.Identifier)
                return LogError<PrototypeAst>(TokenType.Identifier);

            name = _curTok.Value; // identifier
            Next(); // eat identifier
            if (_curTok != TokenType.LParen)
                return LogError<PrototypeAst>(TokenType.LParen);
            Next(); // eat '('
            while (_curTok == TokenType.Identifier)
            {
                args.Add(_curTok.Value);
                Next(); // eat identifier
                if (_curTok == TokenType.RParen)
                    break;
                if (_curTok != TokenType.Comma)
                    return LogError<PrototypeAst>(TokenType.Comma);
                Next(); // eat ','
                if (_curTok != TokenType.Identifier)
                    return LogError<PrototypeAst>(TokenType.Identifier);
            }
            if (_curTok != TokenType.RParen)
                return LogError<PrototypeAst>(TokenType.RParen);
            Next(); // eat ')'
            return new PrototypeAst(name, args);
        }

        // <blockscopeexpr> ::= '{' ( <statementexpr> )* '}'
        private BlockScopeExprAst ParseExpressionBlock()
        {
            var body = new List<IExprAst>();

            if (_curTok != TokenType.LBrace)
                return LogError<BlockScopeExprAst>(TokenType.LBrace);

            Next(); // eat '{'

            while (_curTok != TokenType.RBrace)
            {
                var expr = ParseExpression();
                if (expr == null)
                    return null;
                body.Add(expr);
            }

            if (_curTok != TokenType.RBrace)
                return LogError<BlockScopeExprAst>(TokenType.RBrace);

            Next(); // eat '}'
            return new BlockScopeExprAst(body);
        }


        // <func>   ::= 'func' <prototype> <blockscopeexpr>
        private FuncAst ParseFunc()
        {
            PrototypeAst proto;
            BlockScopeExprAst body;

            if (_curTok != TokenType.Func)
                return LogError<FuncAst>(TokenType.Func);

            Next(); // eat the 'func'
            if (_curTok != TokenType.Identifier)
                return LogError<FuncAst>(TokenType.Identifier);

            proto = ParsePrototype(); // get the prototype
            if (_curTok != TokenType.LBrace)
                return LogError<FuncAst>(TokenType.LBrace);
            body = ParseExpressionBlock(); // get the body block

            return body == null ? null : new FuncAst(proto, body);
        }

        // <arrayexpr>  ::= 'array' identifier ';'
        private ArrayExprAst ParseArrayAst()
        {
            string name;

            if (_curTok != TokenType.Array)
                return LogError<ArrayExprAst>(TokenType.Array);
            Next(); // eat 'array'
            if (_curTok != TokenType.Identifier)
                return LogError<ArrayExprAst>(TokenType.Identifier);
            name = _curTok.Value;
            Next(); // eat identifier
            if (_curTok != TokenType.Semicolon)
                return LogError<ArrayExprAst>(TokenType.Semicolon);
            Next(); // eat ';'

            return new ArrayExprAst(name);
        }

        // <varexpr>    ::= 'var' identifier ?( '=' <expression> ) ';'
        //              |   identifier '=' <statementexpr>
        //              |   'var' '[]' identifier ';'
        private VarAst ParseVarExpr()
        {
            string name;
            IExprAst value;

            if (_curTok != TokenType.Var)
                return LogError<VarAst>(TokenType.Var);

            Next(); // eat the 'var'
            if (_curTok != TokenType.Identifier)
                return LogError<VarAst>(TokenType.Identifier);
            name = _curTok.Value;
            Next(); // eat the identifier

            if (_curTok == TokenType.Semicolon)
            {
                Next(); // eat ';'
                return new VarAst(name, new BooleanExprAst(false));
            }
            if (_curTok != TokenType.Assign)
                return LogError<VarAst>(TokenType.Assign);
            Next(); // eat the '='
            value = ParseExpression();

            return value == null ? null : new VarAst(name, value);
        }

        // <defexpr>    ::= 'def' identifier '=' <statementexpr>
        private DefAst ParseDefExpr()
        {
            string name;
            IExprAst value;

            if (_curTok != TokenType.Def)
                return LogError<DefAst>(TokenType.Def);

            Next(); // eat the 'def'
            if (_curTok != TokenType.Identifier)
                return LogError<DefAst>(TokenType.Identifier);
            name = _curTok.Value;
            Next(); // eat the identifier
            if (_curTok != TokenType.Assign)
                return LogError<DefAst>(TokenType.Assign);
            Next(); // eat the '='
            value = ParseExpression();

            return value == null ? null : new DefAst(name,value);
        }


    }
}
