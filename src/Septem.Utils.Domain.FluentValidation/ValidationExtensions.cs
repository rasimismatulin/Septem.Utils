using System;
using System.Globalization;
using FluentValidation;
using Septem.Utils.Helpers.ActionInvoke;

namespace Septem.Utils.Domain.FluentValidation;

public static class ValidationExtensions
{
    private const string DefaultInvalidLocalizationString = "Not localized validation error";

    public static string GetMessage(IValidationContext ctx, string message, params object[] arguments)
    {
        var instance = ctx?.InstanceToValidate;
        var locale = "en";
        if (instance is Request request && !string.IsNullOrWhiteSpace(request.Language))
            locale = request.Language;

        var msg = ValidationMessages.ResourceManager.GetString(message, new CultureInfo(locale)) ?? DefaultInvalidLocalizationString;
        return string.Format(msg, arguments);
    }


    public static IRuleBuilderOptions<T, TProperty> WithLocaleMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string message, params object[] formatParameters)
    {
        return rule.Configure(config => config.Current.SetErrorMessage((context, property) =>
        {
            var instance = context.InstanceToValidate;
            var locale = "en";
            if (instance is Request request && !string.IsNullOrWhiteSpace(request.Language))
                locale = request.Language;
            var msg = ValidationMessages.ResourceManager.GetString(message, new CultureInfo(locale)) ?? DefaultInvalidLocalizationString;
        
            var format = new object[formatParameters.Length + 2];
            format[0] = config.PropertyName;
            format[1] = context.DisplayName;
            for (var i = 2; i < format.Length; i++)
                format[i] = formatParameters[i - 2];

            return string.Format(msg, format);
        }));
    }

    public static IRuleBuilderOptions<T, TProperty> NotNullWithLocaleMessage<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return ruleBuilder.NotNull().WithLocaleMessage(nameof(ValidationMessages.NotNull));
    }

    public static IRuleBuilderOptions<T, TProperty> NotEmptyWithLocaleMessage<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return ruleBuilder.NotEmpty().WithLocaleMessage(nameof(ValidationMessages.NotEmpty));
    }

    #region Lenght

    public static IRuleBuilderOptions<T, string> LengthWithLocaleMessage<T>(this IRuleBuilder<T, string> ruleBuilder, int exactLength)
    {
        return ruleBuilder.Length(exactLength).WithLocaleMessage(nameof(ValidationMessages.ExactLength), exactLength);
    }
    public static IRuleBuilderOptions<T, string> MaxLengthWithLocaleMessage<T>(this IRuleBuilder<T, string> ruleBuilder, int maxLength)
    {
        return ruleBuilder.MaximumLength(maxLength).WithLocaleMessage(nameof(ValidationMessages.MaximumLength), maxLength);
    }
    public static IRuleBuilderOptions<T, string> MinLengthWithLocaleMessage<T>(this IRuleBuilder<T, string> ruleBuilder, int minLength)
    {
        return ruleBuilder.MinimumLength(minLength).WithLocaleMessage(nameof(ValidationMessages.MinimumLength), minLength);
    }

    #endregion

    #region GreaterThan

    public static IRuleBuilderOptions<T, TProperty?> GreaterThanWithLocaleMessage<T, TProperty>(this IRuleBuilder<T, TProperty?> ruleBuilder, TProperty valueToCompare)
        where TProperty : struct, IComparable<TProperty>, IComparable
    {
        return ruleBuilder.GreaterThan(valueToCompare)
            .WithLocaleMessage(nameof(ValidationMessages.GreaterThan), valueToCompare);
    }

    public static IRuleBuilderOptions<T, TProperty> GreaterThanWithLocaleMessage<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
        return ruleBuilder.GreaterThan(valueToCompare)
            .WithLocaleMessage(nameof(ValidationMessages.GreaterThan), valueToCompare);
    }

    public static IRuleBuilderOptions<T, TProperty> GreaterThanOrEqualsWithLocaleMessage<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
        return ruleBuilder.GreaterThanOrEqualTo(valueToCompare)
            .WithLocaleMessage(nameof(ValidationMessages.GreaterThanOrEquals), valueToCompare);
    }

    public static IRuleBuilderOptions<T, TProperty?> GreaterThanOrEqualsWithLocaleMessage<T, TProperty>(this IRuleBuilder<T, TProperty?> ruleBuilder, TProperty valueToCompare)
        where TProperty : struct, IComparable<TProperty>, IComparable
    {
        return ruleBuilder.GreaterThanOrEqualTo(valueToCompare)
            .WithLocaleMessage(nameof(ValidationMessages.GreaterThanOrEquals), valueToCompare);
    }

    #endregion

    #region LessThan

    public static IRuleBuilderOptions<T, TProperty?> LessThanWithLocaleMessage<T, TProperty>(this IRuleBuilder<T, TProperty?> ruleBuilder, TProperty valueToCompare)
        where TProperty : struct, IComparable<TProperty>, IComparable
    {
        return ruleBuilder.LessThan(valueToCompare)
            .WithLocaleMessage(nameof(ValidationMessages.LessThan), valueToCompare);
    }

    public static IRuleBuilderOptions<T, TProperty?> LessThanOrEqualsWithLocaleMessage<T, TProperty>(this IRuleBuilder<T, TProperty?> ruleBuilder, TProperty valueToCompare)
        where TProperty : struct, IComparable<TProperty>, IComparable
    {
        return ruleBuilder.LessThanOrEqualTo(valueToCompare)
            .WithLocaleMessage(nameof(ValidationMessages.LessThanOrEquals), valueToCompare);
    }


    public static IRuleBuilderOptions<T, TProperty> LessThanWithLocaleMessage<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
        return ruleBuilder.LessThan(valueToCompare)
            .WithLocaleMessage(nameof(ValidationMessages.LessThan), valueToCompare);
    }

    public static IRuleBuilderOptions<T, TProperty> LessThanOrEqualsWithLocaleMessage<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
        return ruleBuilder.LessThanOrEqualTo(valueToCompare)
            .WithLocaleMessage(nameof(ValidationMessages.LessThanOrEquals), valueToCompare);
    }

    #endregion

    public static IRuleBuilderOptions<T, DateOnly> DateOnlyGranterThanOrEqualsWithLocaleMessage<T>(this IRuleBuilder<T, DateOnly> ruleBuilder, DateTime valueToCompare)
    {
        return ruleBuilder.Must(fromDate => fromDate >= DateOnly.FromDateTime(valueToCompare))
            .WithLocaleMessage(nameof(ValidationMessages.GreaterThan), valueToCompare);
    }

    public static IRuleBuilderOptions<T, string> MatchesWithLocaleMessage<T>(this IRuleBuilder<T, string> ruleBuilder, string regex)
    {
        return ruleBuilder.Matches(regex).WithLocaleMessage(nameof(ValidationMessages.FormatError), regex);
    }
}

