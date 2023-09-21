using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace EnumsSourceGen
{
	public readonly struct EnumInfo
	{
		public readonly GenerationTypeInfo Type;
		public readonly GenerationTypeInfo ExtensionsType;
		public readonly GenerationTypeInfo HelperType;

		public readonly List<AttributeInfo> Attributes;

		public readonly bool HasFlagsDecorator;
		public readonly GenerationTypeInfo UnderlyingType;

		public readonly List<EnumValueInfo> Values;

		public EnumInfo(GenerationTypeInfo type, List<AttributeInfo> attributes, bool hasFlagsDecorator, GenerationTypeInfo underlyingType, List<EnumValueInfo> values)
		{
			Type = type;
			ExtensionsType = new GenerationTypeInfo(type.Namespace, type.Name+"Extensions");
			HelperType = new GenerationTypeInfo(type.Namespace, type.Name+"Helper");
			Attributes = attributes;
			HasFlagsDecorator = hasFlagsDecorator;
			UnderlyingType = underlyingType;
			Values = values;
		}
	}

	public class GenerationTypeInfo
	{
		public readonly string? Namespace;
		public readonly string Name;

		private string? _fullName;
		public string FullName
		{
			get
			{
				if (_fullName is not null)
					return _fullName;

				_fullName = string.IsNullOrEmpty(Namespace)
					? Name
					: Namespace + "." + Name;

				return _fullName;
			}
		}

		private string? _globalName;
		public string GlobalName
		{
			get
			{
				if (_globalName is not null)
					return _globalName;

				_globalName = string.IsNullOrEmpty(Namespace)
					? Name
					: $"global::{Namespace}.{Name}";

				return _globalName;
			}
		}
		public string? LiteralSuffix { get; }

		public bool HasLiteralSuffix { get; }

		public GenerationTypeInfo(string? @namespace, string name, string? literalSuffix = null)
		{
			Namespace = @namespace;
			Name = name;

			LiteralSuffix = literalSuffix;
			HasLiteralSuffix = string.IsNullOrEmpty(literalSuffix) == false;
		}
	}

	public sealed class GenerationTypeInfo<TType> : GenerationTypeInfo
	{
		private static Type _type = typeof(TType);
		public static GenerationTypeInfo<TType> Instance { get; } = new(_type.Namespace, _type.Name, TypesHelper.GetLiteralSuffixOf(_type));
		private GenerationTypeInfo(string? @namespace, string name, string? literalSuffix)
			: base(@namespace, name, literalSuffix)
		{
		}
	}

	public static class TypesHelper
	{
		public static GenerationTypeInfo GetUnderlyingTypeOfEnum(INamedTypeSymbol symbol)
		{
			if (symbol.EnumUnderlyingType == null)
				return GenerationTypeInfo<int>.Instance;

			return ParseAsIntegralNumericType(symbol.EnumUnderlyingType);
		}

		public static GenerationTypeInfo ParseAsIntegralNumericType(INamedTypeSymbol symbol)
		{
			var underlineType = symbol.ContainingNamespace == null
				? symbol.MetadataName
				: $"{symbol.ContainingNamespace}.{symbol.Name}";

			var type = Type.GetType(underlineType);
			if(type == null || type == typeof(int))
				return GenerationTypeInfo<int>.Instance;

			var result = CreateInfoByType(type);
			return result;
		}

		private const string UInt32Suffix = "U";
		private const string Int64Suffix  = "L";
		private const string UInt64Suffix = "UL";
		public static string? GetLiteralSuffixOf(Type type)
		{
			if (type == typeof(uint))
				return UInt32Suffix;

			if (type == typeof(long))
				return Int64Suffix;

			if (type == typeof(ulong))
				return UInt64Suffix;

			return null;
		}

		public static GenerationTypeInfo CreateInfoByType(Type type)
		{
			var literalSuffix = GetLiteralSuffixOf(type);
			return new GenerationTypeInfo(type.Namespace, type.Name, literalSuffix);
		}
	}
}