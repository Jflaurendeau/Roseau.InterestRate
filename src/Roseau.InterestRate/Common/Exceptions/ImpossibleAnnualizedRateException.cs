using System.Runtime.Serialization;

namespace Roseau.InterestRate.Common.Exceptions;

[Serializable]
public class ImpossibleAnnualizedRateException : DomainException
{
	public ImpossibleAnnualizedRateException() : base("An annualized rate must be greater than -1") { }
	protected ImpossibleAnnualizedRateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);
}
