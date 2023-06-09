﻿using System.Runtime.Serialization;

namespace Roseau.InterestRate.Common.Exceptions;
[Serializable]
public class DomainException : Exception
{
	protected DomainException(string? message) : base(message) { }
	protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
