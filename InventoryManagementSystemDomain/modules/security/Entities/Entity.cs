using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationCore.Models
{
    public abstract class Entity<TId> where TId : notnull
    {
        public TId Id { get; protected set; }

        protected Entity(TId id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return Id.Equals(other.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();
        public static bool operator ==(Entity<TId>? a, Entity<TId>? b) => a?.Equals(b) ?? b is null;
        public static bool operator !=(Entity<TId>? a, Entity<TId>? b) => !(a == b);
    }
}
