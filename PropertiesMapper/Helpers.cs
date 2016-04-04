using System;
using System.Linq.Expressions;

namespace PropertiesMapper
{
	public static class Helpers
	{
		public static MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> expr)
		{
			var member = expr.Body as MemberExpression;
			var unary = expr.Body as UnaryExpression;
			if (member == null && unary == null)
			{
				throw new Exception($"Expression '{expr}' can't be cast to MemberExpression or UnaryExpression.");
			}
			return member ?? unary.Operand as MemberExpression;
		}
	}
}