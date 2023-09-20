using System;
using System.Collections.Generic;

namespace EnumsSourceGen
{
	public readonly struct EnumInfo
	{
		public readonly string Namespace;

		public readonly string Name;

		public readonly string FullName;

		public readonly List<AttributeInfo> Attributes;

		public readonly bool HasFlagsDecorator;
		public readonly string UnderlineTypeFullName;

		public readonly List<EnumValueInfo> Values;

		public EnumInfo(string @namespace, string name, List<AttributeInfo> attributes, bool hasFlagsDecorator, string underlineTypeFullName, List<EnumValueInfo> values)
		{
			Namespace = @namespace;
			Name = name;
			FullName = $"{@namespace}.{name}";
			Attributes = attributes;
			HasFlagsDecorator = hasFlagsDecorator;
			UnderlineTypeFullName = underlineTypeFullName;
			Values = values;
		}
	}
}