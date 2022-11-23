using System;
using FluentValidation;
using FluentValidation.Results;
using Septem.Utils.Helpers.Extensions;

namespace Septem.Utils.Domain.FluentValidation;

public class NotFoundValidationFailure : ValidationFailure
{
    public NotFoundValidationFailure(IValidationContext context, string type, Guid uid) :
        base("UID", ValidationExtensions.GetMessage(context, nameof(ValidationMessages.NotFound), type, uid.Encode()))
    {
        ErrorCode = "Controversy";
    }
    public NotFoundValidationFailure(IValidationContext context, string type, string prm) :
        base("UID", ValidationExtensions.GetMessage(context, nameof(ValidationMessages.NotFound), type, prm))
    {
        ErrorCode = "Controversy";
    }

    public NotFoundValidationFailure(IValidationContext context, string type, Guid uid,
        object attemptedValue) : base("UID", ValidationExtensions.GetMessage(context, nameof(ValidationMessages.NotFound), type, uid.Encode()), attemptedValue)
    {
        ErrorCode = "Controversy";
    }
}