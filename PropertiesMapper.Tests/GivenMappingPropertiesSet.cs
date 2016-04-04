using System;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;
using PropertiesMapper.Tests.Entities;

namespace PropertiesMapper.Tests
{
	[TestFixture]
	public class GivenMappingPropertiesSet
	{
		public class WhenForMember
		{
			[Test]
			public void ShouldMap()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid, p => p.Id);

				set.Should().NotBeNull();
				set.Should().BeOfType<MappingPropertiesSet<Person, PersonDTO>>();
			}

			[Test]
			public void ShouldThrowForDuplicateMap()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid, p => p.Id);

				Action action = () => set.ForMember(dto => dto.new_name, p => p.Id);

				action.ShouldThrow<Exception>()
					.WithMessage("Key already added");
			}

			[Test]
			public void ShouldThrowForMethodBinaryExpression()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid, p => p.Id);

				Action action = () => set.ForMember(dto => dto.new_name, p => p.Id + p.Name);

				action.ShouldThrow<Exception>()
					.WithMessage("Expression 'source => (Convert(source.Id) + source.Name)' can't be cast to MemberExpression or UnaryExpression.");
			}

			[Test]
			public void ShouldReturnMapUsingSubEntities()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid, p => p.Id)
					.ForMember(dto => dto.new_address_id, p => p.CustomAddress.AddressId);

				set.Should().NotBeNull();
			}
		}

		public class WhenResolveNameWithExpression
		{
			[Test]
			public void ShouldReturnNullForNotMappedProperty()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>();
				Action action = () => set.ResolveName(p => p.Id);

				action.ShouldThrow<Exception>()
					.WithMessage("Mapping for source.Id was not found");
			}

			[Test]
			public void ShouldReturnName()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid, p => p.Id);
				
				var name = set.ResolveName(p => p.Id);

				name.Should().Be("new_entityid");
			}

			[Test]
			public void ShouldReturnNameForField()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_collection, p => p.SomeCollection);
				
				var name = set.ResolveName(p => p.SomeCollection);

				name.Should().Be("new_collection");
			}

			[Test]
			public void ShouldResolveNameForSubEntity()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid, p => p.Id)
					.ForMember(dto => dto.new_address_id, p => p.CustomAddress.AddressId);

				var name = set.ResolveName(p => p.CustomAddress.AddressId);

				name.Should().Be("new_address_id");
			}

			[Test]
			public void ShouldThrowForMethodBinaryExpression()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid + dto.new_name, p => p.Id);

				Action action = () => set.ResolveName(p => p.Id);

				action.ShouldThrow<Exception>()
					.WithMessage("Expression 'dto => (dto.new_entityid + dto.new_name)' can't be cast to MemberExpression.");
			}
		}

		public class WhenResolveNameWithMemberInfo
		{
			[Test]
			public void ShouldReturnNullForNotMappedProperty()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>();

				Expression<Func<Person, object>> expression = p => p.Id;

				var member = ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member;
				Action action = () => set.ResolveName(member, null);

				action.ShouldThrow<Exception>()
					.WithMessage("Mapping for source.Id was not found");
			}

			[Test]
			public void ShouldReturnName()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid, p => p.Id);

				Expression<Func<Person, object>> expression = p => p.Id;

				var member = ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member;

				var name = set.ResolveName(member, null);

				name.Should().Be("new_entityid");
			}

			[Test]
			public void ShouldReturnNameForField()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_collection, p => p.SomeCollection);

				Expression<Func<Person, object>> expression = p => p.SomeCollection;

				var member = ((MemberExpression)expression.Body).Member;

				var name = set.ResolveName(member, null);

				name.Should().Be("new_collection");
			}

			[Test]
			public void ShouldResolveNameForSubEntity()
			{
				var set = new MappingPropertiesSet<Person, PersonDTO>()
					.ForMember(dto => dto.new_entityid, p => p.Id)
					.ForMember(dto => dto.new_address_id, p => p.CustomAddress.AddressId);

				Expression<Func<Person, object>> parent = p => p.CustomAddress;
				Expression<Func<Person, object>> expression = p => p.CustomAddress.AddressId;

				var member = ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member;

				var name = set.ResolveName(member, ((MemberExpression)parent.Body).Member.Name);

				name.Should().Be("new_address_id");
			}
		}
	}
}