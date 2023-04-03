namespace Roseau.InterestRate.SeedWork;

public abstract class Entity
{
	private Guid _Id;


	public virtual Guid Id
	{
		get
		{
			return _Id;
		}
		protected set
		{
			_Id = value;
		}
	}
	public bool IsTransient()
	{
		return this.Id == Guid.Empty;
	}
	public override bool Equals(object? obj)
	{
		if (obj is not Entity)
			return false;
		if (Object.ReferenceEquals(this, obj))
			return true;
		if (this.GetType() != obj.GetType())
			return false;
		Entity item = (Entity)obj;
		if (item.IsTransient() || this.IsTransient())
			return false;
		else
			return item.Id == this.Id;
	}
	public override int GetHashCode()
	{
		if (!IsTransient())
			return this.Id.GetHashCode() ^ 31;
		// XOR for random distribution. See: 
		// https://docs.microsoft.com/archive/blogs/ericlippert/guidelines-and-rules-for-gethashcode
		return 0;
	}
}
