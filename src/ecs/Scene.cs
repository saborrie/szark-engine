using System;
using System.Collections.Generic;

namespace Szark
{
    public sealed class Scene
    {
        internal Dictionary<int, List<IComponent>> Entities { get; }

        public Scene() =>
            Entities = new Dictionary<int, List<IComponent>>();

        public Entity Instantiate(params IComponent[] components)
        {
            int GenerateID()
            {
                Random random = new Random();
                int id = random.Next(int.MaxValue);
                while (Entities.ContainsKey(id))
                    id = random.Next(int.MaxValue);
                return id;
            }

            int newID = GenerateID();
            Entities.Add(newID, new List<IComponent>(components));
            return new Entity(newID);
        }

        public void Destroy(Entity entity)
        {
            if (Entities.ContainsKey(entity.id))
                Entities.Remove(entity.id);
        }

        public void AddComponent(Entity entity, IComponent component)
        {
            if (Entities.ContainsKey(entity.id))
                Entities[entity.id].Add(component);
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            if (Entities.ContainsKey(entity.id))
                Entities[entity.id].RemoveAll(c => c is T);
        }

        public IComponent GetComponent<T>(Entity entity) where T : IComponent
        {
            if (Entities.ContainsKey(entity.id))
                foreach (var component in Entities[entity.id])
                    if (component is T) return (T)component;
            return null;
        }
    }
}