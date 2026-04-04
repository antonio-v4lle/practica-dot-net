namespace FamilyFinance.Domain.Common;

public abstract class AggregateRoot : Entity
{
    public int Version { get; private set; }
    public void IncrementVersion() => Version++;
}
