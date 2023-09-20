using System;
using EnumsSourceGen.Attributes;

namespace SourceGenTestsApp4._8
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine($"{TestEnum.Two.ToName()} = {TestEnum.Two.ToValue()}");
		}
	}
	[EnumOptimized]
	public enum TestEnum
	{
		One = 1,
		Two = 2,
	}
}