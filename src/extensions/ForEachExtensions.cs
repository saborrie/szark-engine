using System;
using System.Collections.Generic;

namespace Szark
{
    public static partial class EntityManager
    {
        public static void ForEach<T>(Action<T> action) 
            where T : IComponent
        {
            foreach (var entity in Active.Entities)
            {
                int i0 = entity.Value.IndexOfType<T>();
                if (i0 == -1) continue;
                action((T)entity.Value[i0]);
            }
        }

        public static void ForEach<T, TJ>(Action<T, TJ> action) 
            where T : IComponent where TJ : IComponent
        {
            foreach (var entity in Active.Entities)
            {
                int i0 = entity.Value.IndexOfType<T>();
                int i1 = entity.Value.IndexOfType<TJ>();

                if (i0 == -1 || i1 == -1) continue;
                action((T)entity.Value[i0], (TJ)entity.Value[i1]);
            }
        }

        public static void ForEach<T, TJ, TK>(Action<T, TJ, TK> action) 
            where T : IComponent where TJ : IComponent where TK : IComponent
        {
            foreach (var entity in Active.Entities)
            {
                int i0 = entity.Value.IndexOfType<T>();
                int i1 = entity.Value.IndexOfType<TJ>();
                int i2 = entity.Value.IndexOfType<TK>();

                if (i0 == -1 || i1 == -1 || i2 == -1) continue;
                action((T)entity.Value[i0], (TJ)entity.Value[i1],
                    (TK)entity.Value[i2]);
            }
        }

        public static void ForEach<T, TJ, TK, TL>(Action<T, TJ, TK, TL> action)
            where T : IComponent where TJ : IComponent
            where TK : IComponent where TL : IComponent
        {
            foreach (var entity in Active.Entities)
            {
                int i0 = entity.Value.IndexOfType<T>();
                int i1 = entity.Value.IndexOfType<TJ>();
                int i2 = entity.Value.IndexOfType<TK>();
                int i3 = entity.Value.IndexOfType<TL>();

                if (i0 == -1 || i1 == -1 || i2 == -1 || i3 == -1) continue;

                action((T)entity.Value[i0], (TJ)entity.Value[i1],
                    (TK)entity.Value[i2], (TL)entity.Value[i3]);
            }
        }

        private static int IndexOfType<T>(this List<IComponent> list) where T : IComponent
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i] is T) return i;
            return -1;
        }
    }
}