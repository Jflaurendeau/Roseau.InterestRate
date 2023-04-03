﻿using Roseau.InterestRate.Common.Exceptions;

namespace Roseau.InterestRate.UnitTests.Common.Exceptions;

[TestClass]
public class UnusableNumberOfYearsExceptionTest
{
    [TestMethod]
    public void Constructor_Serialized_ReturnTrue()
    {
        var exception = new UnusableNumberOfYearsException();
        string json = System.Text.Json.JsonSerializer.Serialize(exception);

        var deserializedException =
            System.Text.Json.JsonSerializer.Deserialize<UnusableNumberOfYearsException>(json);
        Assert.AreEqual(exception.HelpLink, deserializedException?.HelpLink);
        Assert.AreEqual(exception.HResult, deserializedException?.HResult);
        Assert.AreEqual(exception.Message, deserializedException?.Message);
        Assert.AreEqual(exception.Source, deserializedException?.Source);
        Assert.AreEqual(exception.StackTrace, deserializedException?.StackTrace);
        Assert.AreEqual(exception.TargetSite, deserializedException?.TargetSite);
    }
}
