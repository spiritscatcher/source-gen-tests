using EnumsSourceGen.Attributes;

namespace SourceGenTests.Enums
{
	[EnumOptimized]
	public enum CompilationType
	{
		None,
		Compile,
		NotCompile,
	}
}