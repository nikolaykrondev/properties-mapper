using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PropertiesMapper
{
	public class MappingPropertiesSet<TSource, TDestination> : IMappingPropertiesSet<TSource, TDestination>
	{
		private static readonly string _predicateSourceName = "source";

		private readonly Dictionary<MemberExpression, Expression<Func<TDestination, object>>> _set =
			new Dictionary<MemberExpression, Expression<Func<TDestination, object>>>(new CustomEqualityComparer());

		private static Expression<Func<TSource, object>> Rewrite(Expression<Func<TSource, object>> exp, string newParamName)
		{
			var param = Expression.Parameter(exp.Parameters[0].Type, newParamName);
			var newExpression = new PredicateRewriterVisitor(param).Visit(exp);

			return (Expression<Func<TSource, object>>)newExpression;
		}

		private class PredicateRewriterVisitor : ExpressionVisitor
		{
			private readonly ParameterExpression _parameterExpression;

			public PredicateRewriterVisitor(ParameterExpression parameterExpression)
			{
				_parameterExpression = parameterExpression;
			}

			protected override Expression VisitParameter(ParameterExpression node)
			{
				return _parameterExpression;
			}
		}

		public IMappingPropertiesSet<TSource, TDestination> ForMember(Expression<Func<TDestination, object>> dest, Expression<Func<TSource, object>> source)
		{
			source = Rewrite(source, _predicateSourceName);
			var key = Helpers.GetMemberExpression(source);
			
			if (_set.ContainsKey(key))
			{
				throw new Exception("Key already added");
			}
			_set.Add(key, dest);

			return this;
		}

		public string ResolveName(Expression<Func<TSource, object>> fun)
		{
			fun = Rewrite(fun, _predicateSourceName);
			var key = Helpers.GetMemberExpression(fun);
			
			if (!_set.ContainsKey(key))
			{
				throw new Exception($"Mapping for {key} was not found");
			}
			var name = GetKey(_set[key]);
			return name;
		}

		public string ResolveName(MemberInfo memberInfo, string parentName)
		{
			var key = GenerateMemberExpression(memberInfo.Name, parentName);
			if (!_set.ContainsKey(key))
			{
				throw new Exception($"Mapping for {key} was not found");
			}
			var name = GetKey(_set[key]);
			return name;
		}

		private static MemberExpression GenerateMemberExpression(string propertyName, string parentName)
		{
			var entityParam = Expression.Parameter(typeof(TSource), _predicateSourceName);
			if (!string.IsNullOrEmpty(parentName))
			{
				var parent = typeof (TSource).GetProperty(parentName);
				var propertyInfo = parent.PropertyType.GetProperty(propertyName);
				var columnExprParent = Expression.Property(entityParam, parent);
				var columnExpr = Expression.MakeMemberAccess(columnExprParent, propertyInfo);
				return columnExpr;
			}
			else
			{
				var member = typeof (TSource).GetMember(propertyName);
				if (!member.Any())
				{
					throw new MemberAccessException($"Member with name {propertyName} has not found");
				}
				var columnExpr = Expression.MakeMemberAccess(entityParam, member.First());
				return columnExpr;
			}
		}

		private static string GetKey(Expression<Func<TDestination, object>> propertyLambda)
		{
			var member = propertyLambda.Body as MemberExpression;

			if (member == null)
			{
				throw new ArgumentException($"Expression '{propertyLambda}' can't be cast to MemberExpression.");
			}

			return member.Member.Name;
		}
	}

	internal class CustomEqualityComparer : IEqualityComparer<MemberExpression>
	{
		public bool Equals(MemberExpression x, MemberExpression y)
		{
			return x.IsSameMember(y);
		}

		public int GetHashCode(MemberExpression obj)
		{
			return obj.Member.GetHashCode();
		}
	}
}
