using CommunityToolkit.Diagnostics;
using Roseau.DateHelpers;
using Roseau.InterestRate.SeedWork;

namespace Roseau.InterestRate.Aggregates.Rate;

public class AnnualizedRates : Entity, IAggregateRoot
{
	private readonly AnnualizedRate[] _rates;

	public AnnualizedRates(DateOnly calculationDate, IEnumerable<AnnualizedRate> annualizedRates)
	{
		GuardAgainstInvalidState(calculationDate, annualizedRates);
		CalculationDate = calculationDate;
		_rates = annualizedRates.ToArray();
	}
	private static void GuardAgainstInvalidState(DateOnly calculationDate, IEnumerable<AnnualizedRate> annualizedRates)
	{
		Guard.IsNotNull(calculationDate);
		Guard.IsNotNull(annualizedRates);
		int numberOfNull = annualizedRates.Count(_ => _ is null);
		if (numberOfNull > 0)
			throw new ArgumentNullException(nameof(annualizedRates), $"The rates (IEnumerable<AnnualizedRate>) contains {numberOfNull} non-initialized elements and all element must be initialized");
	}
	
	public DateOnly CalculationDate { get; private set; }
	public IReadOnlyCollection<AnnualizedRate> Rates => _rates;

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
	public decimal DiscountFactor(DateOnly paymentDate)
	{
		if (CalculationDate > paymentDate)
			throw new ArgumentOutOfRangeException(nameof(paymentDate), $"The calculation date ({CalculationDate}) must be a prior date then the payment date ({paymentDate}).");

		var numberOfYears = CalculationDate.AgeCalculator(paymentDate);
		int index = 0;
		decimal discountFactor = 1m;
		decimal numberOfYearsOfRates = _rates[index].NumberOfYears;
		while (index < _rates.Length - 1 &&
			numberOfYears >= numberOfYearsOfRates)
		{
			discountFactor *= _rates[index].DiscountFactor(numberOfYearsOfRates);
			numberOfYears -= numberOfYearsOfRates;
			index++;
			numberOfYearsOfRates = _rates[index].NumberOfYears;
		}
		discountFactor *= _rates[index].DiscountFactor(Math.Min(numberOfYears, numberOfYearsOfRates));
		return discountFactor;
	}
	public decimal[] DiscountFactors(OrderedDates dates)
	{
		Guard.IsNotNull(dates);
		if (dates[0] < CalculationDate)
			throw new ArgumentOutOfRangeException($"The {nameof(CalculationDate)} ({CalculationDate}) must precedes {dates[0]} and every other date in the list");

		int numberOfDates = dates.Count;
		decimal[] discountFactorArray = new decimal[numberOfDates];


		int paymentDateIndex = 0;
		decimal totalNumberOfYears = CalculationDate.AgeCalculator(dates[paymentDateIndex]);
		decimal totalPayedNumberOfYears = 0m;

		int rateIndex = 0;
		decimal rateNumberOfYears = _rates[rateIndex].NumberOfYears;

		decimal paymentDateNumberOfYears;
		decimal intermediatNumberOfYears;
		decimal discountFactor = 1m;
		while (paymentDateIndex < numberOfDates
			&& rateIndex < _rates.Length)
		{
			paymentDateNumberOfYears = totalNumberOfYears - totalPayedNumberOfYears;
			intermediatNumberOfYears = Math.Min(paymentDateNumberOfYears, rateNumberOfYears);

			discountFactor *= _rates[rateIndex].DiscountFactor(intermediatNumberOfYears);
			discountFactorArray[paymentDateIndex] = discountFactor;

			totalPayedNumberOfYears += intermediatNumberOfYears;
			if (totalNumberOfYears <= totalPayedNumberOfYears
			&& ++paymentDateIndex < numberOfDates)
				totalNumberOfYears = CalculationDate.AgeCalculator(dates[paymentDateIndex]);

			rateNumberOfYears -= intermediatNumberOfYears;
			if (rateNumberOfYears <= Decimal.Zero
			&& ++rateIndex < _rates.Length)
				rateNumberOfYears = _rates[rateIndex].NumberOfYears;
		}
		while (paymentDateIndex < numberOfDates)
		{
			discountFactorArray[paymentDateIndex] = discountFactor;
			paymentDateIndex++;
		}
		return discountFactorArray;
	}
	public decimal AccumulationFactor(DateOnly paymentDate)
	{
		if (CalculationDate > paymentDate)
			throw new ArgumentOutOfRangeException(nameof(paymentDate), $"The calculation date ({CalculationDate}) must be a prior date then the payment date ({paymentDate}).");

		var numberOfYears = CalculationDate.AgeCalculator(paymentDate);
		int index = 0;
		decimal accumulationFactor = 1m;
		decimal numberOfYearsOfRates = _rates[index].NumberOfYears;
		while (index < _rates.Length - 1 &&
			numberOfYears >= numberOfYearsOfRates)
		{
			accumulationFactor *= _rates[index].AccumulationFactor(numberOfYearsOfRates);
			numberOfYears -= numberOfYearsOfRates;
			index++;
			numberOfYearsOfRates = _rates[index].NumberOfYears;
		}
		accumulationFactor *= _rates[index].AccumulationFactor(Math.Min(numberOfYears, numberOfYearsOfRates));
		return accumulationFactor;
	}
	public decimal[] AccumulationFactors(OrderedDates dates)
	{
		Guard.IsNotNull(dates);
		if (dates[0] < CalculationDate)
			throw new ArgumentOutOfRangeException($"The {nameof(CalculationDate)} ({CalculationDate}) must precedes {dates[0]} and every other date in the list");
		int numberOfDates = dates.Count;
		decimal[] discountFactorArray = new decimal[numberOfDates];


		int paymentDateIndex = 0;
		decimal totalNumberOfYears = CalculationDate.AgeCalculator(dates[paymentDateIndex]);
		decimal totalPayedNumberOfYears = 0m;

		int rateIndex = 0;
		decimal rateNumberOfYears = _rates[rateIndex].NumberOfYears;

		decimal paymentDateNumberOfYears;
		decimal intermediatNumberOfYears;
		decimal discountFactor = 1m;
		while (paymentDateIndex < numberOfDates
			&& rateIndex < _rates.Length)
		{
			paymentDateNumberOfYears = totalNumberOfYears - totalPayedNumberOfYears;
			intermediatNumberOfYears = Math.Min(paymentDateNumberOfYears, rateNumberOfYears);

			discountFactor *= _rates[rateIndex].AccumulationFactor(intermediatNumberOfYears);
			discountFactorArray[paymentDateIndex] = discountFactor;

			totalPayedNumberOfYears += intermediatNumberOfYears;
			if (totalNumberOfYears <= totalPayedNumberOfYears
			&& ++paymentDateIndex < numberOfDates)
				totalNumberOfYears = CalculationDate.AgeCalculator(dates[paymentDateIndex]);

			rateNumberOfYears -= intermediatNumberOfYears;
			if (rateNumberOfYears <= Decimal.Zero
			&& ++rateIndex < _rates.Length)
				rateNumberOfYears = _rates[rateIndex].NumberOfYears;
		}
		while (paymentDateIndex < numberOfDates)
		{
			discountFactorArray[paymentDateIndex] = discountFactor;
			paymentDateIndex++;
		}
		return discountFactorArray;
	}

	public Task<decimal[]> DiscountFactorsAsync(OrderedDates dates) => Task.Run(() => DiscountFactors(dates));
	public Task<decimal[]> AccumulationFactorsAsync(OrderedDates dates) => Task.Run(() => AccumulationFactors(dates));

}
