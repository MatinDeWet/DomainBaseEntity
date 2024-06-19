namespace DomainBaseEntity
{
    public abstract class BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateDeleted { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
