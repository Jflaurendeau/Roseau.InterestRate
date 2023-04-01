namespace Roseau.InterestRate.SeedWork;

public interface IRepository<T> where T : IAggregateRoot
{
	IUnitOfWork UnitOfWork { get; }

	Task<T> AddAsync(T aggregateRoot);
	void Update(T aggregateRoot);
	Task<T?> GetByIdAsync(Guid id);
}
