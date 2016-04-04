using System;
using System.Linq.Expressions;
using NUnit.Framework;
using FluentAssertions;
using PropertiesMapper.Tests.Entities;

namespace PropertiesMapper.Tests
{
	[TestFixture]
	public class GivenPropertiesMapper
	{
		public class WhenAddMap
		{
			[Test]
			public void ShouldReturnMap()
			{
				var mapper = new PropertiesMapper();
				var map = mapper.AddMap<Person, PersonDTO>();

				map.Should().NotBeNull();
			}

			[Test]
			public void ShouldThrowForDuplicateMap()
			{
				var mapper = new PropertiesMapper();
				mapper.AddMap<Person, PersonDTO>();

				Action action = () => mapper.AddMap<Person, PersonDTO>();
				action.ShouldThrow<Exception>()
					.WithMessage("Duplicate mapping");
			}
		}

		public class WhenResolveNameWithExpression
		{
			[Test]
			public void ShouldReturnNullForNotMappedProperty()
			{
				var mapper = new PropertiesMapper();
				var name = mapper.ResolveName<PersonDTO, Person>(x => x.Id);

				name.Should().BeNull();
			}

			[Test]
			public void ShouldReturnName()
			{
				var mapper = new PropertiesMapper();
				mapper.AddMap<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid, p => p.Id);

				var name = mapper.ResolveName<PersonDTO, Person>(x => x.Id);

				name.Should().Be("new_entityid");
			}
		}

		public class WhenResolveNameWithMemberInfo
		{
			[Test]
			public void ShouldReturnNullForNotMappedProperty()
			{
				var mapper = new PropertiesMapper();

				Expression<Func<Person, object>> expression = p => p.Id;

				var member = ((MemberExpression) ((UnaryExpression)expression.Body).Operand).Member;
				var name = mapper.ResolveName<PersonDTO, Person>(member, null);

				name.Should().BeNull();
			}

			[Test]
			public void ShouldReturnName()
			{
				var mapper = new PropertiesMapper();
				mapper.AddMap<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid, p => p.Id);

				Expression<Func<Person, object>> expression = p => p.Id;

				var member = ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member;

				var name = mapper.ResolveName<PersonDTO, Person>(member, null);

				name.Should().Be("new_entityid");
			}
		}
	}
}