﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SearchableComboBox.Samples.Extensions
{
    public static class MvxPropertyNameExtensions
    {
        private const string WrongExpressionMessage =
            "Wrong expression\nshould be called with expression like\n() => PropertyName";

        private const string WrongUnaryExpressionMessage =
            "Wrong unary expression\nshould be called with expression like\n() => PropertyName";

        public static string GetPropertyNameFromExpression<T>(
            this object target,
            Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var memberExpression = FindMemberExpression(expression);

            if (memberExpression == null)
            {
                throw new ArgumentException(WrongExpressionMessage, nameof(expression));
            }

            var member = memberExpression.Member as PropertyInfo;
            if (member == null)
            {
                throw new ArgumentException(WrongExpressionMessage, nameof(expression));
            }

            if (member.DeclaringType == null)
            {
                throw new ArgumentException(WrongExpressionMessage, nameof(expression));
            }

            if (target != null && !member.DeclaringType.IsInstanceOfType(target))
            {
                throw new ArgumentException(WrongExpressionMessage, nameof(expression));
            }

            if (member.GetGetMethod(true).IsStatic)
            {
                throw new ArgumentException(WrongExpressionMessage, nameof(expression));
            }

            return member.Name;
        }

        private static MemberExpression FindMemberExpression<T>(Expression<Func<T>> expression)
        {
            var body = expression.Body as UnaryExpression;
            if (body != null)
            {
                var unary = body;
                var member = unary.Operand as MemberExpression;
                if (member == null)
                    throw new ArgumentException(WrongUnaryExpressionMessage, nameof(expression));
                return member;
            }

            return expression.Body as MemberExpression;
        }
    }
}
