using System;

namespace Szark
{
    public struct Option<T>
    {
        private T Content;

        public bool HasValue { get; private set; }

        private Option(T content)
        {
            Content = content;
            HasValue = true;
        }

        public static Option<T> Some(T content) => 
            new Option<T>(content);

        public static Option<T> None() => 
            new Option<T>();

        public void Assign(T value)
        {
            Content = value;
            HasValue = true;
        }

        public T Unwrap()
        {
            if (!HasValue)
                throw new Exception("Option is None!");
            else
                return Content;
        }

        public void Map(Action<T> Some, Action None)
        {
            if (!HasValue) None?.Invoke();
            else Some?.Invoke(Content);
        }

        public T Or(T value) => 
            !HasValue ? value : Content;
    }
}
