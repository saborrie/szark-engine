using System;

namespace Szark
{
    public struct Entity : IEquatable<Entity>
    {
        public readonly int id;
        public Entity(int id) => this.id = id;

        public bool Equals(Entity other) =>
            id == other.id;
    }
}