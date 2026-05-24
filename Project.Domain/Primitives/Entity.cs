using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Primitives
{
    public abstract class Entity : IEquatable<Entity>
    {
        public int Id { get; private set; }

        // Audit — set automatically by AuditInterceptor
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }     // ← soft delete, covers IsActive

        protected Entity(int id)
        {
            Id = id;
        }

        protected Entity()
        {
        } 

        public static bool operator ==(Entity? left, Entity? right)
            => left is not null && right is not null && left.Equals(right);

        public static bool operator !=(Entity? left, Entity? right)
            => !(left == right);

        public bool Equals(Entity? other)
            => other is not null && other.Id == Id;

        public override bool Equals(object? obj)
            => obj is Entity entity && Equals(entity);

        public override int GetHashCode()
            => Id.GetHashCode();
    }
}
