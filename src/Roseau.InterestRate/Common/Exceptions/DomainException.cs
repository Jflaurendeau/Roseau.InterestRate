using System.Runtime.Serialization;

namespace Roseau.InterestRate.Common.Exceptions;
[Serializable]
public class DomainException : Exception
{
	protected DomainException() : base() { }
	protected DomainException(string? message) : base(message) { }
	protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context) { }

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
