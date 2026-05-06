namespace CasaDaRosa.Domain.Abstractions;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; protected set; }

    protected void SetUpdatedAtUtc()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
