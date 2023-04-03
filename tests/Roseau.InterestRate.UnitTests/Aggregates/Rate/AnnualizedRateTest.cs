using Roseau.InterestRate.Aggregates.Rate;
using Roseau.InterestRate.Common.Exceptions;
using Roseau.Mathematics;

namespace Roseau.InterestRate.UnitTests.Aggregates.Rate;

[TestClass]
public class AnnualizedRateTest
{
	[TestMethod]
	[DataRow(-2)]
	[DataRow(-1)]
	public void AnnualizedRate_RateLessOrEqualToMinusOne_ThrowException(int n)
	{
		Assert.ThrowsException<ImpossibleAnnualizedRateException>(() => new AnnualizedRate(n, 10m));
	}
	[TestMethod]
	[DataRow(-2)]
	[DataRow(0)]
	public void NumberOfYears_YearsLessOrEqualTo0_ThrowException(int n)
	{
		Assert.ThrowsException<UnusableNumberOfYearsException>(() => new AnnualizedRate(0m, n));
	}
	[TestMethod]
	public void DiscountFactor_PaymentDateIsBeforeCalculationDate_ThrowException()
	{
		// Arrange
		AnnualizedRate rate = new(0m, 10m);
		DateOnly paymentDate = new(2002, 1, 1);
		DateOnly calculationDate = paymentDate.AddDays(1);

		// Assert
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => rate.DiscountFactor(calculationDate, paymentDate));
	}
	[TestMethod]
	public void AccumulationFactor_PaymentDateIsBeforeCalculationDate_ThrowException()
	{
		// Arrange
		AnnualizedRate rate = new(0m, 10m);
		DateOnly paymentDate = new(2002, 1, 1);
		DateOnly calculationDate = paymentDate.AddDays(1);

		// Assert
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => rate.AccumulationFactor(calculationDate, paymentDate));
	}
	[TestMethod]
	public void DiscountFactor_TenYearsOfDiscountRate_ReturnsGoodResult()
	{
		// Arrange
		decimal rate = 0.01m;
		decimal years = 10;
		AnnualizedRate annualizedRate = new AnnualizedRate(rate, years);
		DateOnly calculationDate = new(2000, 1, 1);
		DateOnly paymentDate = calculationDate.AddYears((int)years);
		decimal exactDiscountFactor = Decimal.One;

		// Act
		for (int i = 0; i < 10; i++)
		{
			exactDiscountFactor *= 1 / (1 + rate);
		}
		decimal testDiscountFactor = annualizedRate.DiscountFactor(calculationDate, paymentDate);

		// Assert
		Assert.AreEqual(exactDiscountFactor, testDiscountFactor);
	}
	[TestMethod]
	public void GetEqualityComponent_TwoAnnualizedRatesAreEqual_ReturnsTrue()
	{
		// Arrange
		decimal rate = 0.01m;
		decimal years = 10;
		AnnualizedRate annualizedRate = new AnnualizedRate(rate, years);
		AnnualizedRate annualizedRate2 = new AnnualizedRate(rate, years);

		// Act

		// Assert
		Assert.IsTrue(annualizedRate.Equals(annualizedRate2));
	}
	[TestMethod]
	public void GetEqualityComponent_TwoAnnualizedRatesAreNotEqual_ReturnsFalse()
	{
		// Arrange
		decimal rate = 0.01m;
		decimal years = 10;
		AnnualizedRate annualizedRate = new AnnualizedRate(rate, years);
		AnnualizedRate annualizedRate2 = new AnnualizedRate(rate, years+1);

		// Act

		// Assert
		Assert.IsFalse(annualizedRate.Equals(annualizedRate2));
	}
	[TestMethod]
	public void DiscountFactor_HalfAYearWithLowDiscountRateIsExact_ReturnsTrue()
	{
		// Arrange
		decimal rate = 0.01m;
		decimal years = 0.5m;
		AnnualizedRate annualizedRate = new(rate, years);
		DateOnly calculationDate = new(2000, 1, 1);
		DateOnly paymentDate = calculationDate.AddYears(1);
		decimal exactDiscountFactor;
		decimal testDiscountFactor;
		decimal difference;
		bool differenceIsInMarginOrError;

		// Act
		exactDiscountFactor = 0.995037190209989135665273753738571899697m; // ... = 1.01^-0.5 sur Wolfram
		testDiscountFactor = annualizedRate.DiscountFactor(calculationDate, paymentDate);
		difference = Math.Abs(exactDiscountFactor - testDiscountFactor); // Différence peut être causé par les arrondis machines
		differenceIsInMarginOrError = difference <= 2 * Mathematics.Mathematics.Epsilon;

		// Assert
		Assert.IsTrue(differenceIsInMarginOrError);
	}
	[TestMethod]
	public void DiscountFactor_HalfAYearWithHighDiscountRateIsExact_ReturnsTrue()
	{
		// Arrange
		decimal rate = 0.1m;
		decimal years = 0.5m;
		AnnualizedRate annualizedRate = new(rate, years);
		DateOnly calculationDate = new(2000, 1, 1);
		DateOnly paymentDate = calculationDate.AddYears(1);
		decimal exactDiscountFactor;
		decimal testDiscountFactor;
		decimal difference;
		bool differenceIsInMarginOrError;

		// Act
		exactDiscountFactor = 0.953462589245592315446775921527215998613883506m; // ... = 1.1^-0.5 sur Wolfram
		testDiscountFactor = annualizedRate.DiscountFactor(calculationDate, paymentDate);
		difference = Math.Abs(exactDiscountFactor - testDiscountFactor); // Différence peut être causé par les arrondis machines
		differenceIsInMarginOrError = difference <= 2 * Mathematics.Mathematics.Epsilon;

		// Assert
		Assert.IsTrue(differenceIsInMarginOrError);
	}
	[TestMethod]
	public void DiscountFactor_QuaterOfAYearWithLowDiscountRateIsExact_ReturnsTrue()
	{
		// Arrange
		decimal rate = 0.01m;
		decimal years = 0.25m;
		AnnualizedRate rateDefault = new(rate, years);
		DateOnly calculationDate = new(2000, 1, 1);
		DateOnly paymentDate = calculationDate.AddYears(1);
		decimal exactDiscountFactor;
		decimal testDiscountFactor;
		decimal difference;
		bool differenceIsInMarginOrError;

		// Act
		exactDiscountFactor = 0.99751550875662536521326534126147377214732017172763m; // ... = 1.01^-0.25 sur Wolfram
		testDiscountFactor = rateDefault.DiscountFactor(calculationDate, paymentDate);
		difference = Math.Abs(exactDiscountFactor - testDiscountFactor); // Différence peut être causé par les arrondis machines
		differenceIsInMarginOrError = difference <= 2 * Mathematics.Mathematics.Epsilon;

		// Assert
		Assert.IsTrue(differenceIsInMarginOrError);
	}
	[TestMethod]
	public void DiscountFactor_QuaterOfAYearWithHighDiscountRateIsExact_ReturnsTrue()
	{
		// Arrange
		decimal rate = 0.1m;
		decimal years = 0.25m;
		AnnualizedRate rateDefault = new(rate, years);
		DateOnly calculationDate = new(2000, 1, 1);
		DateOnly paymentDate = calculationDate.AddYears(1);
		decimal exactDiscountFactor;
		decimal testDiscountFactor;
		decimal difference;
		bool differenceIsInMarginOrError;

		// Act
		exactDiscountFactor = 0.976454089676310544893104527925023643152860997m; // ... = 1.1^-0.25 sur Wolfram
		testDiscountFactor = rateDefault.DiscountFactor(calculationDate, paymentDate);
		difference = Math.Abs(exactDiscountFactor - testDiscountFactor); // Différence peut être causé par les arrondis machines
		differenceIsInMarginOrError = difference <= 2 * Mathematics.Mathematics.Epsilon;

		// Assert
		Assert.IsTrue(differenceIsInMarginOrError);
	}
	[TestMethod]
	public void DiscountFactor_TwelfthOfAYearWithLowDiscountRateIsExact_ReturnsTrue()
	{
		// Arrange
		decimal rate = 0.01m;
		decimal years = 1 / 12m;
		AnnualizedRate rateDefault = new(rate, years);
		DateOnly calculationDate = new(2000, 1, 1);
		DateOnly paymentDate = calculationDate.AddYears(1);
		decimal exactDiscountFactor;
		decimal testDiscountFactor;
		decimal difference;
		bool differenceIsInMarginOrError;

		// Act
		exactDiscountFactor = 0.999171149448777100086916245948279920437642033m; // ... = 1.01^-(1/12) sur Wolfram
		testDiscountFactor = rateDefault.DiscountFactor(calculationDate, paymentDate);
		difference = Math.Abs(exactDiscountFactor - testDiscountFactor); // Différence peut être causé par les arrondis machines
		differenceIsInMarginOrError = difference <= 2 * Mathematics.Mathematics.Epsilon;

		// Assert
		Assert.IsTrue(differenceIsInMarginOrError);
	}
	[TestMethod]
	public void DiscountFactor_TwelfthOfAYearWithHighDiscountRateIsExact_ReturnsTrue()
	{
		// Arrange
		decimal rate = 0.1m;
		decimal years = 1 / 12m;
		AnnualizedRate rateDefault = new(rate, years);
		DateOnly calculationDate = new(2000, 1, 1);
		DateOnly paymentDate = calculationDate.AddYears(1);
		decimal exactDiscountFactor;
		decimal testDiscountFactor;
		decimal difference;
		bool differenceIsInMarginOrError;

		// Act
		exactDiscountFactor = 0.99208894344699095225333725606900033855667734863m; // ... = 1.1^-(1/12) sur Wolfram
		testDiscountFactor = rateDefault.DiscountFactor(calculationDate, paymentDate);
		difference = Math.Abs(exactDiscountFactor - testDiscountFactor); // Différence peut être causé par les arrondis machines
		differenceIsInMarginOrError = difference <= 2 * Mathematics.Mathematics.Epsilon;

		// Assert
		Assert.IsTrue(differenceIsInMarginOrError);
	}
	[TestMethod]
	[DataRow(0.1)]
	[DataRow(1)]
	[DataRow(10)]
	[DataRow(100)]
	public void DiscountFactorAtEndOfRatePeriod_MultiplePeriods_ReturnsTrue(double years)
	{
		// Arrange
		decimal rate = 0.01m;
		AnnualizedRate annualizedRate = new AnnualizedRate(rate, Convert.ToDecimal(years));

		// Act
		decimal expected = Mathematics.Mathematics.Pow(1 / (1 + rate), Convert.ToDecimal(years));
		decimal actual = annualizedRate.DiscountFactorAtEndOfRatePeriod();

		// Assert
		Assert.AreEqual(expected, actual);
	}
	[TestMethod]
	[DataRow(0.1)]
	[DataRow(1)]
	[DataRow(10)]
	[DataRow(100)]
	public void AccumulationFactorAtEndOfRatePeriod_MultiplePeriods_ReturnsTrue(double years)
	{
		// Arrange
		decimal rate = 0.01m;
		AnnualizedRate annualizedRate = new AnnualizedRate(rate, Convert.ToDecimal(years));

		// Act
		decimal expected = Mathematics.Mathematics.Pow(1 + rate, Convert.ToDecimal(years));
		decimal actual = annualizedRate.AccumulationFactorAtEndOfRatePeriod();

		// Assert
		Assert.AreEqual(expected, actual);
	}
}