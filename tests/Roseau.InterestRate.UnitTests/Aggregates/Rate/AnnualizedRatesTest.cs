using Roseau.DateHelpers;
using Roseau.InterestRate.Aggregates.Rate;

namespace Roseau.InterestRate.UnitTests.Aggregates.Rate;

[TestClass]
public class AnnualizedRatesTest
{
	[TestMethod]
	public void AnnualizedRates_PassingIEnumerableWithNull_ThrowsArgumentNullException()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				default!,
				new(0.07m, 1m),
				new(0.11m, 14m)
		};

		// Act

		// Assert
		Assert.ThrowsException<ArgumentNullException>(() => new AnnualizedRates(calculationDate, rates));

	}
	[TestMethod]
	public void Rates_PropertyReturnsSameRate_ReturnTrue()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		DateOnly[] paymentDates = new DateOnly[]
		{
				calculationDate.AddYears(0.3m),
				calculationDate.AddYears(0.5m),
				calculationDate.AddYears(0.6m),
				calculationDate.AddYears(15m),
		};
		OrderedDates dates = new(paymentDates);
		AnnualizedRates annualizedRates = new(calculationDate, rates);

		// Act
		var rates2 = annualizedRates.Rates;

		// Assert
		for (int i = 0; i < rates.Length; i++)
		{
			Assert.AreEqual(rates[i], rates2.ElementAt(i));
		}

	}
	[TestMethod]
	public void DiscountFactorArray_RatesWithPeriodsShorterAndLongerThenPaymentDateInterval_ReturnsTrue()
	{
		// Arrange
		decimal[] expectedDiscountRateArray = new decimal[4];
		decimal[] calculatedDiscountRateArray;
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		DateOnly[] paymentDates = new DateOnly[]
		{
				calculationDate.AddYears(9),
				calculationDate.AddYears(12),
				calculationDate.AddYears(13),
				calculationDate.AddYears(45),
		};
		OrderedDates dates = new(paymentDates);
		AnnualizedRates annualizedRates = new(calculationDate, rates);
		/*
		 * For the determination of the power, using the NumberOfYears :  additionning the number of years:
		 * expected-1: 0.9143398242399131201457665940M
		 * expected-2: 0.7639854290932546905805390548M
		 * expected-3: 0.6882751613452744960185036530M
		 * expected-4: 0.1958835174675591391235347413M
		 * 
		 * For the determination of the power, using the AgeCalculation : calculating the age between calculation date and payment dates.
		 * expected-1: 0.9143398242399131201457665940M
		 * expected-2: 0.7639399227253657227902505878M
		 * expected-3: 0.6882341646174465971083338629M
		 * expected-4: 0.1958835174675591391235347413M
		*/
		// Act
		expectedDiscountRateArray[0] = Mathematics.Mathematics.Pow(1 / (1 + rates[0].Rate), 9);
		expectedDiscountRateArray[1] = 0.7639854290932546905805390548M;
		expectedDiscountRateArray[2] = 0.6882751613452744960185036530M;
		expectedDiscountRateArray[3] = Mathematics.Mathematics.Pow(1 / (1 + rates[0].Rate), 10) * Mathematics.Mathematics.Pow(1 / (1 + rates[1].Rate), 1 / 24m) * Mathematics.Mathematics.Pow(1 / (1 + rates[2].Rate), 1m) * Mathematics.Mathematics.Pow(1 / (1 + rates[3].Rate), 14m);
		calculatedDiscountRateArray = annualizedRates.DiscountFactors(dates);
		decimal difference = 0;
		for (int i = 0; i < expectedDiscountRateArray.Length; i++)
			difference += Math.Abs(expectedDiscountRateArray[i] - calculatedDiscountRateArray[i]);
		var differenceIsInMarginOrError = difference <= 2 * Mathematics.Mathematics.Epsilon;

		// Assert
		Assert.IsTrue(differenceIsInMarginOrError);
	}
	[TestMethod]
	public void DatesAtWhichEachRatePeriodEnd_TestAgainstManuallyCalculatedDates_DatesOfNewRate_ReturnTrue()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		DateOnly[] expectedDates = new DateOnly[]
		{
				new DateOnly(2032, 5, 5),
				new DateOnly(2032, 5, 20),
				new DateOnly(2033, 5, 20),
				new DateOnly(2047, 5, 20),
		};
		AnnualizedRates annualizedRates = new(calculationDate, rates);

		// Act
		DateOnly[] dates = annualizedRates.DatesAtWhichEachRatePeriodEnd();
		bool test = expectedDates[0] == dates[0];
		test = test && expectedDates[1] == dates[1];
		test = test && expectedDates[2] == dates[2];
		test = test && expectedDates[3] == dates[3];

		// Assert
		Assert.IsTrue(test);
	}
	[TestMethod]
	public void DiscountFactor_EqualsDiscountFactorsMethod_ReturnTrue()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		DateOnly[] paymentDates = new DateOnly[]
		{
				calculationDate.AddYears(0.3m),
				calculationDate.AddYears(0.5m),
				calculationDate.AddYears(0.6m),
				calculationDate.AddYears(15m),
		};
		OrderedDates dates = new(paymentDates);
		AnnualizedRates annualizedRates = new(calculationDate, rates);
		bool differenceIsInMarginOrError;
		decimal difference;

		// Act
		var expectedFactor = annualizedRates.DiscountFactors(dates);

		decimal[] factor = new decimal[expectedFactor.Length];
		for (int i = 0; i < factor.Length; i++)
			factor[i] = annualizedRates.DiscountFactor(dates[i]);
		
		// Assert
		for (int i = 0; i < expectedFactor.Length; i++)
		{
			difference = factor[i] - expectedFactor[i];
			differenceIsInMarginOrError = difference <= 2 * Mathematics.Mathematics.Epsilon;
			Assert.IsTrue(differenceIsInMarginOrError);
		}

	}
	[TestMethod]
	public void DiscountFactor_PaymentDateIsBeforeCalculationDate_ThrowsArgumentOutOfRange()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		DateOnly[] paymentDates = new DateOnly[]
		{
				calculationDate.AddYears(-0.3m),
				calculationDate.AddYears(0.5m),
				calculationDate.AddYears(0.6m),
				calculationDate.AddYears(15m),
		};
		OrderedDates dates = new(paymentDates);
		AnnualizedRates annualizedRates = new(calculationDate, rates);

		// Act

		// Assert
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => annualizedRates.DiscountFactor(dates[0]));

	}
	[TestMethod]
	public void DiscountFactors_PaymentDateIsBeforeCalculationDate_ThrowsArgumentOutOfRange()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		DateOnly[] paymentDates = new DateOnly[]
		{
				calculationDate.AddYears(-0.3m),
				calculationDate.AddYears(0.5m),
				calculationDate.AddYears(0.6m),
				calculationDate.AddYears(15m),
		};
		OrderedDates dates = new(paymentDates);
		AnnualizedRates annualizedRates = new(calculationDate, rates);

		// Act

		// Assert
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => annualizedRates.DiscountFactors(dates));

	}
	[TestMethod]
	public void AccumulationFactor_EqualsAccumulationFactorsMethod_ReturnTrue()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		DateOnly[] paymentDates = new DateOnly[]
		{
				calculationDate.AddYears(0.3m),
				calculationDate.AddYears(0.5m),
				calculationDate.AddYears(0.6m),
				calculationDate.AddYears(15m),
		};
		OrderedDates dates = new(paymentDates);
		AnnualizedRates annualizedRates = new(calculationDate, rates);
		bool differenceIsInMarginOrError;
		decimal difference;

		// Act
		var expectedFactor = annualizedRates.AccumulationFactors(dates);

		decimal[] factor = new decimal[expectedFactor.Length];
		for (int i = 0; i < factor.Length; i++)
			factor[i] = annualizedRates.AccumulationFactor(dates[i]);

		// Assert
		for (int i = 0; i < expectedFactor.Length; i++)
		{
			difference = factor[i] - expectedFactor[i];
			differenceIsInMarginOrError = difference <= 2 * Mathematics.Mathematics.Epsilon;
			Assert.IsTrue(differenceIsInMarginOrError);
		}

	}
	[TestMethod]
	public async Task DiscountFactorsAsync_EqualsNonAsyncMethod_ReturnTrue()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		DateOnly[] paymentDates = new DateOnly[]
		{
				calculationDate.AddYears(0.3m),
				calculationDate.AddYears(0.5m),
				calculationDate.AddYears(0.6m),
				calculationDate.AddYears(15m),
		};
		OrderedDates dates = new(paymentDates);
		AnnualizedRates annualizedRates = new(calculationDate, rates);
		// Act
		var expectedFactor = annualizedRates.DiscountFactors(dates);
		var factor = await annualizedRates.DiscountFactorsAsync(dates);
		// Assert
		for(int i = 0; i < expectedFactor.Length; i++)
			Assert.AreEqual(expectedFactor[i], factor[i]);
	}
	[TestMethod]
	public void AccumulationFactors_EqualsInverseOfDiscountFactors_ReturnTrue()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		DateOnly[] paymentDates = new DateOnly[]
		{
				calculationDate.AddYears(0.3m),
				calculationDate.AddYears(0.5m),
				calculationDate.AddYears(0.6m),
				calculationDate.AddYears(15m),
		};
		OrderedDates dates = new(paymentDates);
		AnnualizedRates annualizedRates = new(calculationDate, rates);
		decimal difference;
		bool differenceIsInMarginOrError;

		// Act
		var expectedFactor = annualizedRates.AccumulationFactors(dates);
		var factor = annualizedRates.DiscountFactors(dates);
		for (int i = 0; i < factor.Length; i++)
		{
			factor[i] = 1 / factor[i];
		}

		// Assert
		for (int i = 0; i < expectedFactor.Length; i++)
		{
			difference = factor[i] - expectedFactor[i];
			differenceIsInMarginOrError = difference <= 10 * Mathematics.Mathematics.Epsilon;
			Assert.IsTrue(differenceIsInMarginOrError);
		}
	}
	[TestMethod]
	public async Task AccmulationFactorsAsync_EqualsNonAsyncMethod_ReturnTrue()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 5, 5);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		DateOnly[] paymentDates = new DateOnly[]
		{
				calculationDate.AddYears(0.3m),
				calculationDate.AddYears(0.5m),
				calculationDate.AddYears(0.6m),
				calculationDate.AddYears(15m),
		};
		OrderedDates dates = new(paymentDates);
		AnnualizedRates annualizedRates = new(calculationDate, rates);
		// Act
		var expectedFactor = annualizedRates.AccumulationFactors(dates);
		var factor = await annualizedRates.AccumulationFactorsAsync(dates);
		// Assert
		for (int i = 0; i < expectedFactor.Length; i++)
			Assert.AreEqual(expectedFactor[i], factor[i]);
	}
}
