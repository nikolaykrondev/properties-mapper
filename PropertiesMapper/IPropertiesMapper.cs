using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PropertiesMapper
{
	public interface IPropertiesMapper
	{
		IMappingPropertiesSet<TSource, TDestination> AddMap<TSource, TDestination>();
		string ResolveName<TDestination, TSource>(Expression<Func<TSource, object>> func);
		string ResolveName<TDestination, TSource>(MemberInfo memberInfo, string parentName);
	}
}