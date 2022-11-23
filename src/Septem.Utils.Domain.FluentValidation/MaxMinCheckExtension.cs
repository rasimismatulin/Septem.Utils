using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;

namespace Septem.Utils.Domain.FluentValidation
{
    public static class MaxMinCheckExtension
    {
        public static void RuleForMaxMinOrEquals<T>(this AbstractValidator<T> validator,
            Expression<Func<T, int>> min,
            Expression<Func<T, int>> max)
        {
            var funcMin = min.Compile();
            var funcMax = max.Compile();
            validator.RuleFor(x => new { Min = funcMin(x), Max = funcMax(x) })
                .Must(x => x.Min <= x.Max)
                .WithLocaleMessage(nameof(ValidationMessages.MaxMinError), GetProperty(min).Name, GetProperty(max).Name);
        }

        public static void RuleForMaxMinOrEquals<T>(this AbstractValidator<T> validator,
            Expression<Func<T, int?>> min,
            Expression<Func<T, int?>> max)
        {
            var funcMin = min.Compile();
            var funcMax = max.Compile();
            validator.When(x => funcMin.Invoke(x).HasValue && funcMax(x).HasValue, () =>
            {
                validator.RuleFor(x => new { Min = funcMin(x).Value, Max = funcMax(x).Value })
                    .Must(x => x.Min <= x.Max)
                    .WithLocaleMessage(nameof(ValidationMessages.MaxMinError), GetProperty(min).Name, GetProperty(max).Name);
            });
        }

        public static PropertyInfo GetProperty<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            MemberExpression memberExpression = null;

            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", "expression");
            }

            return memberExpression.Member as PropertyInfo;
        }
    }
}
