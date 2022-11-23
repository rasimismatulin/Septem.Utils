using System;
using FluentValidation;
using FluentValidation.Results;
using Septem.Utils.Helpers.Extensions;

namespace Septem.Utils.Domain.FluentValidation;

public class AlreadyExistsValidationFailure : ValidationFailure
{
    public AlreadyExistsValidationFailure(IValidationContext context, string type, Guid uid) :
        base("UID", ValidationExtensions.GetMessage(context, nameof(ValidationMessages.AlreadyExists), type, uid))
    {
        ErrorCode = "Controversy";
    }
    public AlreadyExistsValidationFailure(IValidationContext context, string type, string prm) :
        base("UID", ValidationExtensions.GetMessage(context, nameof(ValidationMessages.AlreadyExists), type, prm))
    {
        ErrorCode = "Controversy";
    }

    public AlreadyExistsValidationFailure(IValidationContext context, string type, Guid uid,
        object attemptedValue) : base("UID", ValidationExtensions.GetMessage(context, nameof(ValidationMessages.AlreadyExists), type, uid.Encode()), attemptedValue)
    {
        ErrorCode = "Controversy";
    }
}



public class RepeatTimeLimitValidationFailure : ValidationFailure
{
    public RepeatTimeLimitValidationFailure(IValidationContext context, int seconds) :
        base("Username", ValidationExtensions.GetMessage(context, nameof(ValidationMessages.RepeatTimeLimit), seconds))
    {
        ErrorCode = "Controversy";
    }
}