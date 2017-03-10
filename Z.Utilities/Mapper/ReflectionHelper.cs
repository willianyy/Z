// -----------------------------------------------------------------------
// <copyright file="ReflectionHelper.cs" company="HuiBing">
//     Copyright HB All rights reserved.
// </copyright>
// <author>JiangWeiPeng</author>
// <date>2016-04-13</date>
//-----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;

namespace Z.Utilities.Mapper
{
    /// <summary>
    /// Copy from Automapper source code
    /// </summary>
    public static class ReflectionHelper
    {
        public static MemberInfo FindProperty(LambdaExpression lambdaExpression)
        {
            Expression expressionToCheck = lambdaExpression;

            bool done = false;

            while (!done)
            {
                switch (expressionToCheck.NodeType)
                {
                    case ExpressionType.Convert:
                        expressionToCheck = ((UnaryExpression)expressionToCheck).Operand;
                        break;
                    case ExpressionType.Lambda:
                        expressionToCheck = ((LambdaExpression)expressionToCheck).Body;
                        break;
                    case ExpressionType.MemberAccess:
                        var memberExpression = (MemberExpression)expressionToCheck;

                        if (memberExpression.Expression.NodeType != ExpressionType.Parameter &&
                            memberExpression.Expression.NodeType != ExpressionType.Convert)
                        {
                            throw new ArgumentException(string.Format("Expression '{0}' must resolve to top-level member and not any child object's properties. Use a custom resolver on the child type or the AfterMap option instead.", lambdaExpression), "lambdaExpression");
                        }

                        MemberInfo member = memberExpression.Member;

                        return member;
                    default:
                        done = true;
                        break;
                }
            }

            throw new AutoMapperConfigurationException("Custom configuration for members is only supported for top-level individual members on a type.");
        }
    }
}
