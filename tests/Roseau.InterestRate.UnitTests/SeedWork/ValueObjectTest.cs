using Moq;
using Roseau.InterestRate.Aggregates.Rate;
using Roseau.InterestRate.SeedWork;

namespace Roseau.InterestRate.UnitTests.SeedWork;

[TestClass]
public class ValueObjectTest
{
	[TestMethod]
	public void EqualOperator_BothAreNull_ReturnTrue()
	{
		// Arrange

		// Act

		//Assert
		Assert.IsTrue(ValueObject.EqualOperator(null!, null!));
	}
	[TestMethod]
	public void EqualOperator_RightIsNull_ReturnFalse()
	{
		// Arrange
		var valueObject1Mocked = new Mock<ValueObject>();
		var valueObject2Mocked = new Mock<ValueObject>();

		// Act
		var valueObject1 = valueObject1Mocked.Object;
		var valueObject2 = valueObject2Mocked.Object;

		//Assert
		Assert.IsFalse(ValueObject.EqualOperator(valueObject1, null!));
	}
	[TestMethod]
	public void EqualOperator_LeftIsNull_ReturnFalse()
	{
		// Arrange
		var valueObject1Mocked = new Mock<ValueObject>();
		var valueObject2Mocked = new Mock<ValueObject>();

		// Act
		var valueObject1 = valueObject1Mocked.Object;
		var valueObject2 = valueObject2Mocked.Object;

		//Assert
		Assert.IsFalse(ValueObject.EqualOperator(null!, valueObject2));
	}
	[TestMethod]
	public void EqualOperator_BothAreEqual_ReturnTrue()
	{
		// Arrange
		var valueObject1Mocked = new Mock<ValueObject>();
		var valueObject2Mocked = new Mock<ValueObject>();
		valueObject1Mocked.Setup(m => m.Equals(valueObject2Mocked.Object)).Returns(true);

		// Act

		//Assert
		Assert.IsTrue(ValueObject.EqualOperator(valueObject1Mocked.Object, valueObject2Mocked.Object));
	}
	[TestMethod]
	public void EqualOperator_BothAreNotEqual_ReturnFalse()
	{
		// Arrange
		var valueObject1Mocked = new Mock<ValueObject>();
		var valueObject2Mocked = new Mock<ValueObject>();
		valueObject1Mocked.Setup(m => m.Equals(valueObject2Mocked.Object)).Returns(false);

		// Act

		//Assert
		Assert.IsFalse(ValueObject.EqualOperator(valueObject1Mocked.Object, valueObject2Mocked.Object));
	}
	[TestMethod]
	public void NotEqualOperator_BothAreNull_ReturnFalse()
	{
		// Arrange

		// Act

		//Assert
		Assert.AreEqual(!ValueObject.EqualOperator(null!, null!), ValueObject.NotEqualOperator(null!, null!));
	}
	[TestMethod]
	public void NotEqualOperator_RightIsNull_ReturnTrue()
	{
		// Arrange
		var valueObject1Mocked = new Mock<ValueObject>();
		var valueObject2Mocked = new Mock<ValueObject>();

		// Act
		var valueObject1 = valueObject1Mocked.Object;
		var valueObject2 = valueObject2Mocked.Object;

		//Assert
		Assert.AreEqual(!ValueObject.EqualOperator(valueObject1, null!), ValueObject.NotEqualOperator(valueObject1, null!));
	}
	[TestMethod]
	public void NotEqualOperator_LeftIsNull_ReturnTrue()
	{
		// Arrange
		var valueObject1Mocked = new Mock<ValueObject>();
		var valueObject2Mocked = new Mock<ValueObject>();

		// Act
		var valueObject1 = valueObject1Mocked.Object;
		var valueObject2 = valueObject2Mocked.Object;

		//Assert
		Assert.AreEqual(!ValueObject.EqualOperator(null!, valueObject2), ValueObject.NotEqualOperator(null!, valueObject2));
	}
	[TestMethod]
	public void NotEqualOperator_BothAreEqual_ReturnFalse()
	{
		// Arrange
		var valueObject1Mocked = new Mock<ValueObject>();
		var valueObject2Mocked = new Mock<ValueObject>();
		valueObject1Mocked.Setup(m => m.Equals(valueObject2Mocked.Object)).Returns(true);

		// Act

		//Assert
		Assert.AreEqual(!ValueObject.EqualOperator(valueObject1Mocked.Object, valueObject2Mocked.Object), ValueObject.NotEqualOperator(valueObject1Mocked.Object, valueObject2Mocked.Object));
	}
	[TestMethod]
	public void NotEqualOperator_BothAreNotEqual_ReturnTrue()
	{
		// Arrange
		var valueObject1Mocked = new Mock<ValueObject>();
		var valueObject2Mocked = new Mock<ValueObject>();
		valueObject1Mocked.Setup(m => m.Equals(valueObject2Mocked.Object)).Returns(false);

		// Act

		//Assert
		Assert.AreEqual(!ValueObject.EqualOperator(valueObject1Mocked.Object, valueObject2Mocked.Object), ValueObject.NotEqualOperator(valueObject1Mocked.Object, valueObject2Mocked.Object));
	}
	[TestMethod]
	public void Equals_VersusNullObject_ReturnFalse()
	{
		// Arrange
		var valueObject1Mocked = new Mock<ValueObject>();
		var valueObject2Mocked = new Mock<ValueObject>();
		valueObject1Mocked.CallBase = true;
		var valueObject = valueObject1Mocked.Object;

		// Act

		//Assert
		Assert.IsFalse(valueObject.Equals(null));
	}
	[TestMethod]
	public void Equals_VersusOtherTypeObject_ReturnFalse()
	{
		// Arrange
		var valueObject1Mocked = new Mock<ValueObject>();
		valueObject1Mocked.CallBase = true;
		var valueObject = valueObject1Mocked.Object;

		// Act

		//Assert
		Assert.IsFalse(valueObject.Equals("Hello"));
	}
	[TestMethod]
	public void Equals_SameTypeObject_ReturnTrue()
	{
		// Arrange
		ValueObject valueObject1 = new AnnualizedRate(0.01m, 10m);
		ValueObject valueObject2 = new AnnualizedRate(0.01m, 10m);

		// Act

		//Assert
		Assert.IsTrue(valueObject1.Equals(valueObject2));
	}
	[TestMethod]
	public void GetHashCode_SameTypeObject_ReturnTrue()
	{
		// Arrange
		ValueObject valueObject1 = new AnnualizedRate(0.01m, 10m);
		ValueObject valueObject2 = new AnnualizedRate(0.01m, 10m);

		// Act

		//Assert
		Assert.AreEqual(valueObject1.GetHashCode(), valueObject2.GetHashCode());
	}
	[TestMethod]
	public void GetHashCode_NotSameObject_ReturnTrue()
	{
		// Arrange
		ValueObject valueObject1 = new AnnualizedRate(0.02m, 10m);
		ValueObject valueObject2 = new AnnualizedRate(0.01m, 10m);

		// Act

		//Assert
		Assert.AreNotEqual(valueObject1.GetHashCode(), valueObject2.GetHashCode());
	}
}
