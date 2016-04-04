using System;
using System.Collections.Generic;

namespace PropertiesMapper.Tests.Entities
{
	public class Person
	{
		public Guid? Id { get; set; }
		public string Name { get; set; }
		public List<string> SomeCollection = new List<string>();
		public Address CustomAddress { get; set; }
	}
}