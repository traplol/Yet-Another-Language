using System.Collections.Generic;

namespace YAL.Analyzers.Syntax.Ast
{
    enum ExprValueType
    {
        Default,
        Number,
        String,
        Boolean,
        Array,
        ArrayAccess,
        BinOp,
        BlockScope,
        Call,
        Func,
        Def,
        For,
        Identifier,
        If,
        Return,
        Var,
        While
    }
    interface IExprAst
    {
        IExprAst Parent { get; set; }
        bool Returning { get; set; }
        ExprValueType Type { get; set; }

        object Execute(Context<string, object> context );
    }

    abstract class ExprAstBase : IExprAst
    {
        public ExprValueType Type { get; set; }
        public IExprAst Parent { get; set; }
        public bool Returning { get; set; }
        protected ExprAstBase()
        {
            Parent = null;
            Returning = false;
            Type = ExprValueType.Default;
        }
        public virtual object Execute(Context<string, object> context)
        {
            return null;
        }

        protected void CleanUp(Context<string, object> context, int items)
        {
            context.RemoveLast(items);
        }
    }

    class Context<TKey,TVal>
    {
        private readonly List<KeyValuePair<TKey, TVal>> _storage;

        public KeyValuePair<TKey, TVal> this[TKey key]
        {
            get
            {
                for (int i = _storage.Count - 1; i >= 0; --i) // start from the end and work backwards.
                {
                    if (_storage[i].Key.Equals(key))
                        return _storage[i];
                }
                return default(KeyValuePair<TKey, TVal>);
            }
            set
            {
                for (int i = _storage.Count - 1; i >= 0; --i) // start from the end and work backwards.
                {
                    if (_storage[i].Key.Equals(key))
                    {
                        _storage[i] = value;
                        return;
                    }
                }
            }
        }
        public int Count { get { return _storage.Count; } }

        public Context()
        {
            _storage = new List<KeyValuePair<TKey, TVal>>();
        }

        public void PushBack(TKey key, TVal val)
        {
            _storage.Add(new KeyValuePair<TKey, TVal>(key, val));
        }

        public KeyValuePair<TKey, TVal> Pop()
        {
            var tmp = _storage[_storage.Count-1];
            _storage.RemoveAt(_storage.Count - 1);
            return tmp;
        }

        public void RemoveLast(int items)
        {
            for (int i = 0; i < items; ++i)
            {
                _storage.RemoveAt(_storage.Count-1);
            }
        }
    }
}
