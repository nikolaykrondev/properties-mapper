using System;
using System.Linq.Expressions;

namespace PropertiesMapper
{
	public static class Extensions
	{
		public static bool IsSameMember<T>(this Expression<Func<T, object>> expr1, Expression<Func<T, object>> expr2)
		{
			var result1 = Helpers.GetMemberExpression(expr1);
			var result2 = Helpers.GetMemberExpression(expr2);

			return result1.IsSameMember(result2);
		}
		public static bool IsSameMember(this MemberExpression expr1, MemberExpression expr2)
		{
			if (expr1 == null || expr2 == null)
				return false;

			return expr1.Member == expr2.Member;
		}
	}
}