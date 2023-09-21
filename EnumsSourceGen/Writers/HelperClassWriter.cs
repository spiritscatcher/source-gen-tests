using System.Text;

namespace EnumsSourceGen.Writers;

public static class HelperClassWriter
{
	public static void AppendClass(StringBuilder sb, in EnumInfo enumInfo)
	{
		sb.Append(@"
	public static partial class ").Append(enumInfo.HelperType.Name).Append(@"
	{");
		AppendProperty_UnderlyingType(sb, enumInfo);
		AppendMethod_GetValues(sb, enumInfo);
		AppendMethod_GetNames(sb, enumInfo);
		sb.Append(@"
	}"
		);
	}

	public static void AppendProperty_UnderlyingType(StringBuilder sb, in EnumInfo enumInfo)
	{
		sb.Append(@"
		public static global::System.Type UnderlyingType { get; } = typeof(").Append(enumInfo.UnderlyingType.GlobalName).Append(@");
"
		);
	}

	private static void AppendMethod_GetValues(StringBuilder sb, in EnumInfo enumInfo)
	{
		sb.Append(@"
		public static global::System.Collections.Generic.IEnumerable<").Append(enumInfo.Type.FullName).Append(@"> GetValues()
		{"
		);
		foreach (var member in enumInfo.Values)
		{
			sb.Append(@"
			yield return ").Append(enumInfo.Type.FullName).Append('.').Append(member.Name).Append(";");
		}
		sb.Append(@"
		}"
		);
	}

	private static void AppendMethod_GetNames(StringBuilder sb, in EnumInfo enumInfo)
	{
		sb.Append(@"
		public static global::System.Collections.Generic.IEnumerable<global::System.String> GetNames()
		{"
		);
		foreach (var member in enumInfo.Values)
		{
			sb.Append(@"
			yield return """).Append(member.Name).Append(@""";");
		}
		sb.Append(@"
		}"
		);
	}
}