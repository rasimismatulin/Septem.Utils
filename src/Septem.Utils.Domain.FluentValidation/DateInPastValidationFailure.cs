using System;
using FluentValidation;
using FluentValidation.Results;

namespace Septem.Utils.Domain.FluentValidation;

public class DateInPastValidationFailure : ValidationFailure
{
    public DateInPastValidationFailure(IValidationContext context, string type, DateTime date) :
        base(type, ValidationExtensions.GetMessage(context, nameof(ValidationMessages.DateInPast), type, date))
    {
        ErrorCode = "Controversy";
    }
}