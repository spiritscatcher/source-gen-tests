using System.Text;

namespace EnumsSourceGen.Writers;

public static class ExtensionsClassWriter
{
	public static void AppendClass(StringBuilder sb, in EnumInfo enumInfo)
	{
		sb.Append(@"
	public static partial class ").Append(enumInfo.ExtensionsType.Name).Append(@"
	{");
		AppendMethod_ToName(sb, enumInfo);
		AppendMethod_ToValue(sb, enumInfo);
		AppendMethod_ToDigitString(sb, enumInfo);
		AppendMethod_ToDebugString(sb, enumInfo);
		AppendMethod_GetUnderlyingType(sb, enumInfo);
		sb.Append(@"
	}"
		);
	}

	public static void AppendMethod_ToName(StringBuilder sb, in EnumInfo enumInfo)
	{
		sb.Append(@"
		public static global::System.String ToName(this ").Append(enumInfo.Type.FullName).Append(@" value)
		{
			switch(value)
			{");
		foreach (var member in enumInfo.Values)
		{
			sb.Append(@"
				case ").Append(enumInfo.Type.FullName).Append('.').Append(member.Name).Append(@": return """).Append(member.Name).Append(@""";");
		}
		sb.Append(@"
				default: return value.ToString();
			}
		}"
		);
	}

	public static void AppendMethod_ToValue(StringBuilder sb, in EnumInfo enumInfo)
	{
		sb.Append(@"
		public static ").Append(enumInfo.UnderlyingType.GlobalName).Append(" ToValue(this ").Append(enumInfo.Type.FullName).Append(@" value)
		{
			switch(value)
			{");
		foreach (var member in enumInfo.Values)
		{
			sb.Append(@"
				case ").Append(enumInfo.Type.FullName).Append('.').Append(member.Name).Append(": return ").Append(member.ValueAsString);

			if(enumInfo.UnderlyingType.HasLiteralSuffix)
				sb.Append(enumInfo.UnderlyingType.LiteralSuffix);

			sb.Append(';');
		}
		sb.Append(@"
				default: return (").Append(enumInfo.UnderlyingType.GlobalName).Append(@")value;
			}
		}"
		);
	}

	public static void AppendMethod_ToDigitString(StringBuilder sb, in EnumInfo enumInfo)
	{
		sb.Append(@"
		public static global::System.String ToDigitString(this ").Append(enumInfo.Type.FullName).Append(@" value)
		{
			switch(value)
			{");
		foreach (var member in enumInfo.Values)
		{
			sb.Append(@"
				case ").Append(enumInfo.Type.FullName).Append('.').Append(member.Name).Append(@": return """).Append(member.ValueAsString).Append(@""";");
		}
		sb.Append(@"
				default: return value.ToString();
			}
		}"
		);
	}

	public static void AppendMethod_ToDebugString(StringBuilder sb, in EnumInfo enumInfo)
	{
		sb.Append(@"
		public static global::System.String ToDebugString(this ").Append(enumInfo.Type.FullName).Append(@" value)
		{
			switch(value)
			{");
		foreach (var member in enumInfo.Values)
		{
			sb.Append(@"
				case ").Append(enumInfo.Type.FullName).Append('.').Append(member.Name).Append(@": return """).Append(member.Name).Append(" (").Append(member.ValueAsString).Append(@")"";");
		}
		sb.Append(@"
				default: return value.ToString();
			}
		}"
		);
	}

	public static void AppendMethod_GetUnderlyingType(StringBuilder sb, in EnumInfo enumInfo)
	{
		sb.Append(@"
		public static global::System.Type GetUnderlyingType(this ").Append(enumInfo.Type.FullName).Append(@" value)
		{
			return typeof(").Append(enumInfo.UnderlyingType.GlobalName).Append(@");
		}"
		);
	}
}