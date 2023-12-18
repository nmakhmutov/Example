namespace Domain.Abstractions;

public interface IEntity;

public interface IEntity<out TKey> : IEntity
    where TKey : struct
{
    public TKey Id { get; }
}
