using System;
using System.Collections.Generic;

namespace EnumsSourceGen
{
	public readonly struct EnumValueInfo
	{
		public readonly string Name;

		public readonly List<AttributeInfo> Attributes;

		public readonly object? Value;

		public EnumValueInfo(string name, List<AttributeInfo> attributes, object? value)
		{
			Name       = name;
			Attributes = attributes;
			Value      = value;
		}
	}

	public readonly struct AttributeInfo
	{
		public readonly string FullName;
		public readonly List<AttributeParameterInfo> Parameters;

		public AttributeInfo(string fullName, List<AttributeParameterInfo> parameters)
		{
			FullName   = fullName;
			Parameters = parameters;
		}
	}

	public readonly struct AttributeParameterInfo
	{
		public readonly string Name;
		public readonly string Value;

		public AttributeParameterInfo(string name, string value)
		{
			Name  = name;
			Value = value;
		}
	}
}