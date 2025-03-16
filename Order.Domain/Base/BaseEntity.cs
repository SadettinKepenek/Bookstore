namespace Order.Domain.Base;

public abstract class BaseEntity
{
    public Guid Id { get; protected init; }
 
    public DateTime CreatedAt { get;  set; }
    public DateTime UpdatedAt { get;  set; }

    
}