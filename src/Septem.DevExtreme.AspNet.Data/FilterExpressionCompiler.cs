﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Septem.DevExtreme.AspNet.Data.Helpers;

namespace Septem.DevExtreme.AspNet.Data
{

    class FilterExpressionCompiler : ExpressionCompiler
    {
        const string
            CONTAINS = "contains",
            NOT_CONTAINS = "notcontains",
            STARTS_WITH = "startswith",
            ENDS_WITH = "endswith";

        bool _stringToLower;

        readonly bool _supportsEqualsMethod;
        private readonly bool _useNpgILikeFunction;

        public FilterExpressionCompiler(Type itemType, bool guardNulls, bool stringToLower = false, bool supportsEqualsMethod = true, bool useNpgILikeFunction = true)
            : base(itemType, guardNulls)
        {
            _stringToLower = stringToLower;
            _supportsEqualsMethod = supportsEqualsMethod;
            _useNpgILikeFunction = useNpgILikeFunction;
        }

        public LambdaExpression Compile(IList criteriaJson)
        {
            var dataItemExpr = CreateItemParam();
            return Expression.Lambda(CompileCore(dataItemExpr, criteriaJson), dataItemExpr);
        }

        Expression CompileCore(ParameterExpression dataItemExpr, IList criteriaJson)
        {
            if (IsCriteria(criteriaJson[0]))
                return CompileGroup(dataItemExpr, criteriaJson);

            if (IsUnary(criteriaJson))
            {
                return CompileUnary(dataItemExpr, criteriaJson);
            }

            return CompileBinary(dataItemExpr, criteriaJson);
        }

        Expression CompileBinary(ParameterExpression dataItemExpr, IList criteriaJson)
        {
            var hasExplicitOperation = criteriaJson.Count > 2;

            var clientAccessor = Convert.ToString(criteriaJson[0]);
            var clientOperation = hasExplicitOperation ? Convert.ToString(criteriaJson[1]).ToLower() : "=";
            var clientValue = Utils.UnwrapNewtonsoftValue(criteriaJson[hasExplicitOperation ? 2 : 1]);
            var isStringOperation = clientOperation == CONTAINS || clientOperation == NOT_CONTAINS || clientOperation == STARTS_WITH || clientOperation == ENDS_WITH;

            if (CustomFilterCompilers.Binary.CompilerFuncs.Count > 0)
            {
                var customResult = CustomFilterCompilers.Binary.TryCompile(new BinaryExpressionInfo
                {
                    DataItemExpression = dataItemExpr,
                    AccessorText = clientAccessor,
                    Operation = clientOperation,
                    Value = clientValue,
                    StringToLower = _stringToLower
                });

                if (customResult != null)
                    return customResult;
            }

            var accessorExpr = CompileAccessorExpression(dataItemExpr, clientAccessor, progression => {
                if (isStringOperation)
                    ForceToString(progression);

                if (_stringToLower)
                    AddToLower(progression);
            });

            if (isStringOperation)
            {
                return CompileStringFunction(accessorExpr, clientOperation, Convert.ToString(clientValue));

            }
            else
            {
                var useDynamicBinding = accessorExpr.Type == typeof(Object);
                var expressionType = TranslateBinaryOperation(clientOperation);

                if (!useDynamicBinding)
                {
                    try
                    {
                        clientValue = Utils.ConvertClientValue(clientValue, accessorExpr.Type);
                    }
                    catch
                    {
                        return Expression.Constant(false);
                    }
                }

                if (clientValue == null && !Utils.CanAssignNull(accessorExpr.Type))
                {
                    switch (expressionType)
                    {
                        case ExpressionType.GreaterThan:
                        case ExpressionType.GreaterThanOrEqual:
                        case ExpressionType.LessThan:
                        case ExpressionType.LessThanOrEqual:
                            return Expression.Constant(false);

                        case ExpressionType.Equal:
                        case ExpressionType.NotEqual:
                            accessorExpr = Expression.Convert(accessorExpr, Utils.MakeNullable(accessorExpr.Type));
                            break;
                    }
                }

                if (_stringToLower && clientValue is String)
                    clientValue = ((string)clientValue).ToLower();

                Expression valueExpr = Expression.Constant(clientValue, accessorExpr.Type);

                if (useDynamicBinding)
                {
                    var compareMethod = typeof(Utils).GetMethod(nameof(Utils.DynamicCompare));
                    return Expression.MakeBinary(
                        expressionType,
                        Expression.Call(compareMethod, accessorExpr, valueExpr, Expression.Constant(_stringToLower)),
                        Expression.Constant(0)
                    );
                }

                if (expressionType == ExpressionType.Equal || expressionType == ExpressionType.NotEqual)
                {
                    var type = Utils.StripNullableType(accessorExpr.Type);
                    if (_supportsEqualsMethod && !HasEqualityOperator(type))
                    {
                        if (type.IsValueType)
                        {
                            accessorExpr = Expression.Convert(accessorExpr, typeof(Object));
                            valueExpr = Expression.Convert(valueExpr, typeof(Object));
                        }
                        Expression result = Expression.Call(typeof(Object), "Equals", Type.EmptyTypes, accessorExpr, valueExpr);
                        if (expressionType == ExpressionType.NotEqual)
                            result = Expression.Not(result);
                        return result;
                    }
                }

                if (IsInequality(expressionType))
                {
                    var type = Utils.StripNullableType(accessorExpr.Type);
                    if (!HasComparisonOperator(type))
                    {
                        if (type.IsValueType)
                        {
                            var compareToMethod = type.GetMethod("CompareTo", new[] { type }) ?? type.GetMethod("CompareTo", new[] { typeof(object) });
                            if (compareToMethod != null && !compareToMethod.IsStatic && compareToMethod.ReturnType == typeof(int))
                                return CompileCompareToCall(accessorExpr, expressionType, clientValue, compareToMethod);
                        }

                        var compareMethod = type.GetMethod("Compare", new[] { type, type });
                        if (compareMethod != null && compareMethod.IsStatic && compareMethod.ReturnType == typeof(int))
                        {
                            return Expression.MakeBinary(
                                expressionType,
                                Expression.Call(compareMethod, accessorExpr, valueExpr),
                                Expression.Constant(0)
                            );
                        }

                        // Comparer<T>.Default fallback?
                    }
                }

                return Expression.MakeBinary(expressionType, accessorExpr, valueExpr);
            }

        }

        bool IsInequality(ExpressionType type)
        {
            return type == ExpressionType.LessThan || type == ExpressionType.LessThanOrEqual || type == ExpressionType.GreaterThanOrEqual || type == ExpressionType.GreaterThan;
        }

        bool HasEqualityOperator(Type type)
        {
            if (type.IsEnum || (int)Type.GetTypeCode(type) > 2)
                return true;

            if (type == typeof(Guid) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan))
                return true;

            var operatorMethod = type.GetMethod("op_Equality", new[] { type, type });
            return operatorMethod != null && operatorMethod.ReturnType == typeof(bool);
        }

        bool HasComparisonOperator(Type type)
        {
            if (type.IsEnum)
                return false;

            var code = (int)Type.GetTypeCode(type);
            if (code > 4 && code < 18)
                return true;

            if (type == typeof(DateTimeOffset) || type == typeof(TimeSpan))
                return true;

            return type.GetMethod("op_GreaterThan", new[] { type, type }) != null;
        }

        Expression CompileCompareToCall(Expression accessorExpr, ExpressionType expressionType, object clientValue, MethodInfo compareToMethod)
        {
            if (clientValue == null)
                return Expression.Constant(false);

            var result = Expression.MakeBinary(
                expressionType,
                Expression.Call(
                    Utils.IsNullable(accessorExpr.Type) ? Expression.Property(accessorExpr, "Value") : accessorExpr,
                    compareToMethod,
                    Expression.Constant(clientValue, compareToMethod.GetParameters()[0].ParameterType)
                ),
                Expression.Constant(0)
            );

            if (GuardNulls)
            {
                return Expression.Condition(
                    Expression.MakeBinary(ExpressionType.Equal, accessorExpr, Expression.Constant(null)),
                    Expression.Constant(false),
                    result
                );
            }

            return result;
        }

        Expression CompileStringFunction(Expression accessorExpr, string clientOperation, string value)
        {
            var invert = false;
            value = WebUtility.HtmlDecode(value);
            if (clientOperation == NOT_CONTAINS)
            {
                clientOperation = CONTAINS;
                invert = true;
            }

            if (GuardNulls)
                accessorExpr = Expression.Coalesce(accessorExpr, Expression.Constant(""));

            var operationMethod = typeof(String).GetMethod(GetStringOperationMethodName(clientOperation), new[] { typeof(String) });

            Expression valueConstant = clientOperation switch
            {
                STARTS_WITH => Expression.Constant($"{value}%"),
                ENDS_WITH => Expression.Constant($"%{value}"),
                CONTAINS => Expression.Constant($"%{value}%"),
                NOT_CONTAINS => Expression.Constant($"%{value}%"),
                _ => throw new ArgumentOutOfRangeException(nameof(clientOperation), clientOperation, null)
            };

            Expression result;
            if (_useNpgILikeFunction)
            {
                result = Expression.Call(
                    typeof(NpgsqlDbFunctionsExtensions),
                    nameof(NpgsqlDbFunctionsExtensions.ILike),
                    Type.EmptyTypes,
                    Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                    accessorExpr,
                    valueConstant
                );
            }
            else
            {
                result = Expression.Call(accessorExpr, operationMethod, Expression.Constant(value));
            }

            if (invert)
                result = Expression.Not(result);

            return result;
        }

        Expression CompileGroup(ParameterExpression dataItemExpr, IList criteriaJson)
        {
            var operands = new List<Expression>();
            var isAnd = true;
            var nextIsAnd = true;

            foreach (var item in criteriaJson)
            {
                var operandJson = item as IList;

                if (IsCriteria(operandJson))
                {
                    if (operands.Count > 1 && isAnd != nextIsAnd)
                        throw new ArgumentException("Mixing of and/or is not allowed inside a single group");

                    isAnd = nextIsAnd;
                    operands.Add(CompileCore(dataItemExpr, operandJson));
                    nextIsAnd = true;
                }
                else
                {
                    nextIsAnd = Regex.IsMatch(Convert.ToString(item), "and|&", RegexOptions.IgnoreCase);
                }
            }

            Expression result = null;
            var op = isAnd ? ExpressionType.AndAlso : ExpressionType.OrElse;

            foreach (var operand in operands)
            {
                if (result == null)
                    result = operand;
                else
                    result = Expression.MakeBinary(op, result, operand);
            }

            return result;
        }

        Expression CompileUnary(ParameterExpression dataItemExpr, IList criteriaJson)
        {
            return Expression.Not(CompileCore(dataItemExpr, (IList)criteriaJson[1]));
        }

        ExpressionType TranslateBinaryOperation(string clientOperation)
        {
            switch (clientOperation)
            {
                case "=":
                    return ExpressionType.Equal;

                case "<>":
                    return ExpressionType.NotEqual;

                case ">":
                    return ExpressionType.GreaterThan;

                case ">=":
                    return ExpressionType.GreaterThanOrEqual;

                case "<":
                    return ExpressionType.LessThan;

                case "<=":
                    return ExpressionType.LessThanOrEqual;
            }

            throw new NotSupportedException();
        }

        bool IsCriteria(object item)
        {
            return item is IList && !(item is String);
        }

        internal bool IsUnary(IList criteriaJson)
        {
            return Convert.ToString(criteriaJson[0]) == "!";
        }

        string GetStringOperationMethodName(string clientOperation)
        {
            if (clientOperation == STARTS_WITH)
                return nameof(String.StartsWith);

            if (clientOperation == ENDS_WITH)
                return nameof(String.EndsWith);

            return nameof(String.Contains);
        }

        static void AddToLower(List<Expression> progression)
        {
            var last = progression.Last();

            if (last.Type != typeof(String))
                return;

            var toLowerMethod = typeof(String).GetMethod(nameof(String.ToLower), Type.EmptyTypes);
            var toLowerCall = Expression.Call(last, toLowerMethod);

            if (last is MethodCallExpression lastCall && lastCall.Method.Name == nameof(ToString))
                progression.RemoveAt(progression.Count - 1);

            progression.Add(toLowerCall);
        }

        class BinaryExpressionInfo : IBinaryExpressionInfo
        {
            public Expression DataItemExpression { get; set; }
            public string AccessorText { get; set; }
            public string Operation { get; set; }
            public object Value { get; set; }
            public bool StringToLower { get; set; }
        }
    }

}
