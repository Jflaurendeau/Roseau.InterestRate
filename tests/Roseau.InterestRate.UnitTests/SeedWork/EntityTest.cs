using Roseau.InterestRate.Aggregates.Rate;
using Roseau.InterestRate.SeedWork;

namespace Roseau.InterestRate.UnitTests.SeedWork;

[TestClass]
public class EntityTest
{
	[TestMethod]
	public void Id_IdReturnIsGoodOne_ReturnTrue()
	{
		// Arrange
		Guid id = Guid.NewGuid();
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(id, calculationDate, rates);

		// Act

		// Assert
		Assert.AreEqual(id, entity.Id);
	}
	[TestMethod]
	public void IsTransient_GuidIsNotEmpty_ReturnFalse()
	{
		// Arrange
		Guid id = Guid.NewGuid();
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(id, calculationDate, rates);

		// Act

		// Assert
		Assert.IsFalse(entity.IsTransient());
	}
	[TestMethod]
	public void IsTransient_GuidIsEmpty_ReturnTrue()
	{
		// Arrange
		Guid id = Guid.Empty;
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(id, calculationDate, rates);

		// Act

		// Assert
		Assert.IsTrue(entity.IsTransient());
	}
	[TestMethod]
	public void Equals_ObjectIsNotEntity_ReturnFalse()
	{
		// Arrange
		Guid id = Guid.Empty;
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(id, calculationDate, rates);

		// Act

		// Assert
		Assert.IsFalse(entity.Equals(0.02m));
	}
	[TestMethod]
	public void Equals_ObjectsHaveSameReference_ReturnTrue()
	{
		// Arrange
		Guid id = Guid.Empty;
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(id, calculationDate, rates);
		Entity entity2 = entity;

		// Act

		// Assert
		Assert.IsTrue(entity.Equals(entity2));
	}
	[TestMethod]
	public void Equals_FirstItemIsTransient_ReturnFalse()
	{
		// Arrange
		Guid id = Guid.Empty;
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(id, calculationDate, rates);
		AnnualizedRates entity2 = new(id, calculationDate, rates);

		// Act

		// Assert
		Assert.IsFalse(entity.Equals(entity2));
	}
	[TestMethod]
	public void Equals_OnlySecondItemIsTransient_ReturnFalse()
	{
		// Arrange
		Guid id = Guid.Empty;
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(Guid.NewGuid(), calculationDate, rates);
		AnnualizedRates entity2 = new(id, calculationDate, rates);

		// Act

		// Assert
		Assert.IsFalse(entity.Equals(entity2));
	}
	[TestMethod]
	public void Equals_BothHaveDifferentId_ReturnFalse()
	{
		// Arrange
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(Guid.NewGuid(), calculationDate, rates);
		AnnualizedRates entity2 = new(Guid.NewGuid(), calculationDate, rates);

		// Act

		// Assert
		Assert.IsFalse(entity.Equals(entity2));
	}
	[TestMethod]
	public void Equals_BothHaveSameId_ReturnTrue()
	{
		// Arrange
		Guid id = Guid.NewGuid();
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(id, calculationDate, rates);
		AnnualizedRates entity2 = new(id, calculationDate, rates);

		// Act

		// Assert
		Assert.IsTrue(entity.Equals(entity2));
	}
	[TestMethod]
	public void GetHashCode_IsNotTransient_EqualsToBitwiseLogicalOfIdHashCodeAnd31()
	{
		// Arrange
		Guid id = Guid.NewGuid();
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(id, calculationDate, rates);

		// Act

		// Assert
		Assert.AreEqual(entity.GetHashCode(), entity.Id.GetHashCode() ^ 31);
	}
	[TestMethod]
	public void GetHashCode_IsTransient_EqualsToZero()
	{
		// Arrange
		Guid id = Guid.Empty;
		DateOnly calculationDate = new(2022, 05, 15);
		AnnualizedRate[] rates = new AnnualizedRate[]
		{
				new(0.01m, 10m),
				new(0.05m, 1 / 24m),
				new(0.07m, 1m),
				new(0.11m, 14m)
		};
		Entity entity = new AnnualizedRates(id, calculationDate, rates);

		// Act
		
		// Assert
		Assert.AreEqual(0, entity.GetHashCode());
	}
}
