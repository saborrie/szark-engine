using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Szark
{
    public static partial class EntityManager
    {
        public static Scene Active { get; set; }

        private static List<ISystem> updateSystems;
        private static List<ISystem> renderSystems;

        internal static void Init()
        {
            Active = new Scene();
            updateSystems = new List<ISystem>();
            renderSystems = new List<ISystem>();

            var foundSystems = GetAllTypesOf<ISystem>();

            foreach (var type in foundSystems)
            {
                var system = (ISystem)Activator.CreateInstance(type);
                bool isRender = type.GetCustomAttribute<ExcecuteInRender>() != null;

                if (isRender) renderSystems.Add(system);
                else updateSystems.Add(system);
            }
        }

        static List<Type> GetAllTypesOf<T>()
        {
            List<Type> result = new List<Type>();
            Type[] allTypes = Assembly.GetEntryAssembly().GetTypes();
        
            foreach (var type in allTypes)
                if (typeof(T).IsAssignableFrom(type) && !type.IsInterface)
                    result.Add(type);

            return result;
        }

        internal static void Update(float deltaTime) =>
            updateSystems.ForEach(s => s.Execute(deltaTime));

        internal static void Render(float deltaTime) =>
            renderSystems.ForEach(s => s.Execute(deltaTime));
    }
}