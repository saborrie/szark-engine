using System;
using System.Collections.Generic;

namespace Szark
{
    public sealed class Scene
    {
        internal Dictionary<Entity, List<IComponent>> Entities { get; }
        internal static Random random = new Random();

        public Scene() =>
            Entities = new Dictionary<Entity, List<IComponent>>();

        public Entity Instantiate(params IComponent[] components)
        {
            int id = random.Next(int.MaxValue);
            while (Entities.ContainsKey(new Entity(id)))
                id = random.Next(int.MaxValue);

            Entity entity = new Entity(id);
            Entities.Add(entity, new List<IComponent>(components));
            return entity;
        }

        public void Destroy(Entity entity)
        {
            if (Entities.ContainsKey(entity))
                Entities.Remove(entity);
        }

        public void AddComponent(Entity entity, IComponent component)
        {
            if (Entities.ContainsKey(entity))
                Entities[entity].Add(component);
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            if (Entities.ContainsKey(entity))
                Entities[entity].RemoveAll(c => c is T);
        }

        public IComponent GetComponent<T>(Entity entity) where T : IComponent
        {
            if (Entities.ContainsKey(entity))
                foreach (var component in Entities[entity])
                    if (component is T) return (T)component;
            return null;
        }
    }
}