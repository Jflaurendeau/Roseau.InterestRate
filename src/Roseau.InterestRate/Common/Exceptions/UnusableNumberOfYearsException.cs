using System.Runtime.Serialization;

namespace Roseau.InterestRate.Common.Exceptions;

[Serializable]
public class UnusableNumberOfYearsException : DomainException
{
	public UnusableNumberOfYearsException() : base("The number of years a rate is applicable must be greater than 0.") { }
	protected UnusableNumberOfYearsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
