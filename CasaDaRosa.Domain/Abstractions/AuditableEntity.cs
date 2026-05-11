namespace CasaDaRosa.Domain.Abstractions;

public abstract class AuditableEntity : Entity
{
    protected AuditableEntity(Guid id) : base(id) { }

    public DateTime CreatedAtUtc { get; protected set; }
    public DateTime? UpdatedAtUtc { get; protected set; }

    protected virtual void SetUpdatedAtUtc()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }

    protected virtual void SetCreatedAtUtc(DateTime createdAtUtc)
    {
        CreatedAtUtc = createdAtUtc;
    }

    public virtual void Touch()
    {
        this.UpdatedAtUtc = DateTime.UtcNow;
    }
}
