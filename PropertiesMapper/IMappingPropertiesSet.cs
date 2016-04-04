using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PropertiesMapper
{
	public interface IMappingPropertiesSet<TSource, TDestination>
	{
		IMappingPropertiesSet<TSource, TDestination> ForMember(Expression<Func<TDestination, object>> dest, Expression<Func<TSource, object>> source);
		string ResolveName(Expression<Func<TSource, object>> fun);
		string ResolveName(MemberInfo memberInfo, string parentName);
	}
}