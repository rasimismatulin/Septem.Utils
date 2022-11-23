using System;
using FluentValidation;
using FluentValidation.Results;
using Septem.Utils.Helpers.Extensions;

namespace Septem.Utils.Domain.FluentValidation;

public class InvalidEntityRelationValidationFailure : ValidationFailure
{
    public InvalidEntityRelationValidationFailure(IValidationContext context, string type, Guid uid, Guid relation) :
        base("UID", ValidationExtensions.GetMessage(context, nameof(ValidationMessages.InvalidEntityRelation), type, uid.Encode(), relation.Encode()))
    {
        ErrorCode = "Controversy";
    }

    public InvalidEntityRelationValidationFailure(IValidationContext context, string type, Guid uid, Guid relation,
        object attemptedValue) : base("UID", ValidationExtensions.GetMessage(context, nameof(ValidationMessages.InvalidEntityRelation), type, uid.Encode(), relation.Encode()),
        attemptedValue)
    {
        ErrorCode = "Controversy";
    }
}