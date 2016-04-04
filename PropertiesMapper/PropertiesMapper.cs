using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace PropertiesMapper
{
	public class PropertiesMapper : IPropertiesMapper
	{
		private readonly Dictionary<Tuple<Type, Type>, object> _map = new Dictionary<Tuple<Type, Type>, object>();

		public string ResolveName<TDestination, TSource>(MemberInfo memberInfo, string parentName)
		{
			var key = new Tuple<Type, Type>(typeof(TSource), typeof(TDestination));
			if (_map.ContainsKey(key))
			{
				var set = _map[key] as IMappingPropertiesSet<TSource, TDestination>;
				var name = set?.ResolveName(memberInfo, parentName);
				return name;
			}
			return null;
		}

		public string ResolveName<TDestination, TSource>(Expression<Func<TSource, object>> fun)
		{
			var key = new Tuple<Type, Type>(typeof(TSource), typeof(TDestination));
			if (_map.ContainsKey(key))
			{
				var set = _map[key] as IMappingPropertiesSet<TSource, TDestination>;
				var name = set?.ResolveName(fun);
				return name;
			}
			return null;
		}

		public IMappingPropertiesSet<TSource, TDestination> AddMap<TSource, TDestination>()
		{
			var key = new Tuple<Type, Type>(typeof(TSource), typeof(TDestination));
			if (_map.ContainsKey(key))
			{
				throw new Exception("Duplicate mapping");
			}
			var mappingSet = new MappingPropertiesSet<TSource, TDestination>();

			_map.Add(new Tuple<Type, Type>(typeof(TSource), typeof(TDestination)), mappingSet);
			return mappingSet;
		}
	}
}