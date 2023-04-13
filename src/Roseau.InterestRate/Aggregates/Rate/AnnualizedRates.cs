using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Roseau.DateHelpers;
using Roseau.InterestRate.SeedWork;

namespace Roseau.InterestRate.Aggregates.Rate;

public class AnnualizedRates : Entity, IAggregateRoot
{
	private readonly AnnualizedRate[] _rates;
	private readonly IMemoryCache _cache;

	#region Constructors
	public AnnualizedRates(DateOnly calculationDate, IEnumerable<AnnualizedRate> annualizedRates, IMemoryCache? memoryCache = default) : this(Guid.NewGuid(), calculationDate, annualizedRates, memoryCache) { }
	public AnnualizedRates(Guid guid, DateOnly calculationDate, IEnumerable<AnnualizedRate> annualizedRates, IMemoryCache? memoryCache = default)
	{
		GuardAgainstInvalidState(calculationDate, annualizedRates);
		Id = guid;
		CalculationDate = calculationDate;
		_rates = annualizedRates.ToArray();
		_cache = memoryCache ?? new MemoryCache(new MemoryCacheOptions() { SizeLimit = 16, });
	}
	#endregion
	#region Properties
	public DateOnly CalculationDate { get; private set; }
	public IReadOnlyCollection<AnnualizedRate> Rates => _rates;
	#endregion

	public DateOnly[] DatesAtWhichEachRatePeriodEnd()
	{
		DateOnly[] datesEachRatePeriodEnd = new DateOnly[_rates.Length];

		datesEachRatePeriodEnd[0] = _rates[0].NextDateForFollowingRate(CalculationDate);
		for (int i = 1; i < datesEachRatePeriodEnd.Length; i++)
		{
			datesEachRatePeriodEnd[i] = _rates[i].NextDateForFollowingRate(datesEachRatePeriodEnd[i - 1]);
		}
		return datesEachRatePeriodEnd;
	}
	#region Discount factors
	public decimal DiscountFactor(DateOnly paymentDate)
		=> Factor(paymentDate, GetDiscountFactor);
	public ReadOnlySpan<decimal> DiscountFactors(OrderedDates dates)
		=> new(GetFactors(dates, GetDiscountFactor));
	public ReadOnlySpan<decimal> DiscountFactors(OrderedDates dates, int start)
		=> new(GetFactors(dates, GetDiscountFactor), start, dates.Count - start);
	public ReadOnlySpan<decimal> DiscountFactors(OrderedDates dates, int start, int length)
		=> new(GetFactors(dates, GetDiscountFactor), start, length);
	#endregion
	#region Accumulation factors
	public decimal AccumulationFactor(DateOnly paymentDate)
		=> Factor(paymentDate, GetAccumulationFactor);
	public ReadOnlySpan<decimal> AccumulationFactors(OrderedDates dates)
		=> new(GetFactors(dates, GetAccumulationFactor));
	public ReadOnlySpan<decimal> AccumulationFactors(OrderedDates dates, int start)
		=> new(GetFactors(dates, GetAccumulationFactor), start, dates.Count - start);
	public ReadOnlySpan<decimal> AccumulationFactors(OrderedDates dates, int start, int length)
		=> new(GetFactors(dates, GetAccumulationFactor), start, length);
	#endregion
	#region Discount and Accumulation Func
	private static decimal GetDiscountFactor(AnnualizedRate annualizedRate, decimal numberOfYears)
		=> annualizedRate.DiscountFactor(numberOfYears);
	private static decimal GetAccumulationFactor(AnnualizedRate annualizedRate, decimal numberOfYears)
		=> annualizedRate.AccumulationFactor(numberOfYears);
	#endregion


	private static void GuardAgainstInvalidState(DateOnly calculationDate, IEnumerable<AnnualizedRate> annualizedRates)
	{
		Guard.IsNotNull(calculationDate);
		Guard.IsNotNull(annualizedRates);
		int numberOfNull = annualizedRates.Count(_ => _ is null);
		if (numberOfNull > 0)
			throw new ArgumentNullException(nameof(annualizedRates), $"The rates (IEnumerable<AnnualizedRate>) contains {numberOfNull} non-initialized elements and all element must be initialized");
	}
	private decimal[] GetFactors(OrderedDates dates, Func<AnnualizedRate, decimal, decimal> accumulationOrDiscountFactor)
	{
		int key = HashCode.Combine(dates, GetDiscountFactor);
		if (!_cache.TryGetValue(key, out decimal[]? result))
		{
			result = Factors(dates.AsSpan(), accumulationOrDiscountFactor);
			_cache.Set(key, 
				result, 
				new MemoryCacheEntryOptions().SetSize(1)
											 .SetSlidingExpiration(TimeSpan.FromSeconds(10))
											 .SetPriority(CacheItemPriority.Low));
		}
		return result!;
	}
	private decimal Factor(DateOnly paymentDate, Func<AnnualizedRate, decimal, decimal> accumulationOrDiscountFactor)
	{
		if (CalculationDate > paymentDate)
			throw new ArgumentOutOfRangeException(nameof(paymentDate), $"The calculation date ({CalculationDate}) must be a prior date then the payment date ({paymentDate}).");

		var numberOfYears = CalculationDate.AgeCalculator(paymentDate);
		int index = 0;
		decimal factor = 1m;
		decimal numberOfYearsOfRates = _rates[index].NumberOfYears;
		while (index < _rates.Length - 1 &&
			numberOfYears >= numberOfYearsOfRates)
		{
			factor *= accumulationOrDiscountFactor(_rates[index],numberOfYearsOfRates);
			numberOfYears -= numberOfYearsOfRates;
			index++;
			numberOfYearsOfRates = _rates[index].NumberOfYears;
		}
		factor *= accumulationOrDiscountFactor(_rates[index], Math.Min(numberOfYears, numberOfYearsOfRates));
		return factor;
	}
	private decimal[] Factors(ReadOnlySpan<DateOnly> dates, Func<AnnualizedRate, decimal, decimal> AccumulationOrDiscountFactor)
	{
		if (dates == Span<DateOnly>.Empty)
			throw new ArgumentNullException(nameof(dates));
		if (dates[0] < CalculationDate)
			throw new ArgumentOutOfRangeException($"The {nameof(CalculationDate)} ({CalculationDate}) must precedes {dates[0]} and every other date in the list");

		int numberOfDates = dates.Length;
		decimal[] factorArray = new decimal[numberOfDates];


		int paymentDateIndex = 0;
		decimal totalNumberOfYears = CalculationDate.AgeCalculator(dates[paymentDateIndex]);
		decimal totalPayedNumberOfYears = 0m;

		int rateIndex = 0;
		decimal rateNumberOfYears = _rates[rateIndex].NumberOfYears;

		decimal paymentDateNumberOfYears;
		decimal intermediatNumberOfYears;
		decimal factor = 1m;
		int ratesLength = _rates.Length;
		while (paymentDateIndex < numberOfDates
			&& rateIndex < ratesLength)
		{
			paymentDateNumberOfYears = totalNumberOfYears - totalPayedNumberOfYears;
			intermediatNumberOfYears = Math.Min(paymentDateNumberOfYears, rateNumberOfYears);

			factor *= AccumulationOrDiscountFactor(_rates[rateIndex], intermediatNumberOfYears);
			factorArray[paymentDateIndex] = factor;

			totalPayedNumberOfYears += intermediatNumberOfYears;
			if (totalNumberOfYears <= totalPayedNumberOfYears
			&& ++paymentDateIndex < numberOfDates)
				totalNumberOfYears = CalculationDate.AgeCalculator(dates[paymentDateIndex]);

			rateNumberOfYears -= intermediatNumberOfYears;
			if (rateNumberOfYears <= Decimal.Zero
			&& ++rateIndex < ratesLength)
				rateNumberOfYears = _rates[rateIndex].NumberOfYears;
		}
		while (paymentDateIndex < numberOfDates)
		{
			factorArray[paymentDateIndex] = factor;
			paymentDateIndex++;
		}
		return factorArray;
	}
}
