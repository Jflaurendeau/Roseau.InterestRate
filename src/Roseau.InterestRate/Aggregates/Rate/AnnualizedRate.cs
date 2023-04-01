using Roseau.DateHelpers;
using Roseau.InterestRate.Common.Exceptions;
using Roseau.InterestRate.SeedWork;

namespace Roseau.InterestRate.Aggregates.Rate;

public class AnnualizedRate : ValueObject
{
	private decimal _annualizedRate;
	private decimal _numberOfYears;

	/// <summary>
	/// Constructor for the interest rate class
	/// </summary>
	/// <param name="annualizedRate">Annualized effective rate (must be greater than -1)</param>
	/// <param name="numberOfYears">Number of years the annualized rate is applyable (must be greater then 0)</param>
	public AnnualizedRate(decimal annualizedRate, decimal numberOfYears)
	{
		List<Exception> exceptions = new();
		if (annualizedRate <= -1)
			exceptions.Add(new ImpossibleAnnualizedRateException());
		if (numberOfYears <= 0)
			exceptions.Add(new UnusableNumberOfYearsException());
		if (exceptions.Any())
			throw new AggregateException(exceptions);
		Rate = annualizedRate;
		NumberOfYears = numberOfYears;
	}

	/// <summary>
	/// Annualized effective rate (must be greater than -1)
	/// </summary>
	public decimal Rate
	{
		get
		{
			return _annualizedRate;
		}
		private set
		{
			if (value <= -1)
				throw new ImpossibleAnnualizedRateException();
			_annualizedRate = value;
		}
	}
	/// <summary>
	/// Number of years the annualized rate is applyable (must be greater then 0)
	/// </summary>
	public decimal NumberOfYears
	{
		get
		{
			return _numberOfYears;
		}
		private set
		{
			if (value <= 0)
				throw new UnusableNumberOfYearsException();
			_numberOfYears = value;
		}
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Rate;
		yield return NumberOfYears;
	}
	/// <summary>
	/// Determine the date at which a following rate would start to be applied, after the number of period for which the actual rate can be applied.
	/// </summary>
	/// <param name="calculationDate">The calculation date</param>
	/// <returns>Date at which a fallowing rate would start to be applied.</returns>
	public DateOnly NextDateForFollowingRate(DateOnly calculationDate)
	{
		return calculationDate.AddYears(NumberOfYears);
	}
	/// <summary>
	/// Calculate the discount factor applicable at a unique payment date
	/// </summary>
	/// <param name="calculationDate"> The calculation date</param>
	/// <param name="paymentDate">The payment date</param>
	/// <returns></returns>
	public decimal DiscountFactor(DateOnly calculationDate, DateOnly paymentDate)
	{
		if (calculationDate > paymentDate)
			throw new ArgumentOutOfRangeException(nameof(paymentDate), $"The calculation date ({calculationDate}) must be a prior date then the payment date ({paymentDate}).");
		decimal power = Math.Min(NumberOfYears, calculationDate.AgeCalculator(paymentDate));
		return Mathematics.Mathematics.Pow(1 / (1 + Rate), power);
	}
	public decimal DiscountFactor(decimal numberOfYears)
	{
		if (numberOfYears > NumberOfYears)
            throw new ArgumentOutOfRangeException(nameof(numberOfYears), $"The number of years argument ({numberOfYears}) must be less then the number of years property ({NumberOfYears}).");
		return Mathematics.Mathematics.Pow(1 / (1 + Rate), numberOfYears);
	}
	public decimal DiscountFactorAtEndOfRatePeriod() => Mathematics.Mathematics.Pow(1 / (1 + Rate), NumberOfYears);

	public decimal AccumulationFactor(DateOnly calculationDate, DateOnly paymentDate)
	{
		if (calculationDate > paymentDate)
			throw new ArgumentOutOfRangeException(nameof(paymentDate), $"The calculation date ({calculationDate}) must be a prior date then the payment date ({paymentDate}).");
		decimal power = Math.Min(NumberOfYears, calculationDate.AgeCalculator(paymentDate));
		return Mathematics.Mathematics.Pow(1 + Rate, power);
	}
	public decimal AccumulationFactor(decimal numberOfYears)
	{
		if (numberOfYears > NumberOfYears)
			throw new ArgumentOutOfRangeException(nameof(numberOfYears), $"The number of years argument ({numberOfYears}) must be less then the number of years property ({NumberOfYears}).");
		return Mathematics.Mathematics.Pow(1 + Rate, numberOfYears);
	}
	public decimal AccumulationFactorAtEndOfRatePeriod() => Mathematics.Mathematics.Pow(1 + Rate, NumberOfYears);
}
