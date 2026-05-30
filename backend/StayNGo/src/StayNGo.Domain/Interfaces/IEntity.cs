namespace StayNGo.Domain.Interfaces;

public interface IEntity
{
    Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}