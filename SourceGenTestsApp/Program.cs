using System;
using System.Linq;
using EnumsSourceGen.Attributes;

namespace SourceGenTestsApp
{
	using Enums;

	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");
			Console.WriteLine($"{nameof(TestEnum)}: {TestEnum.Two.ToName()} = {TestEnum.Two.ToValue()}");
			Console.WriteLine($"{nameof(TestEnum)} [{TestEnum.Two.GetUnderlyingType()}]: {TestEnum.Two.ToName()} = {TestEnum.Two.ToValue()}; {TestEnum.Two.ToDebugString()}");
			Console.WriteLine($"{nameof(HelloHelper)} [{HelloHelper.UnderlyingType}]: {string.Join(", ", HelloHelper.GetValues().Select(x => x.ToValue()))}; {string.Join(", ", HelloHelper.GetNames())}");
		}
	}
}

namespace SourceGenTestsApp.Enums
{
	[Flags]
	[EnumOptimized]
	public enum Hello : long
	{
		Test = 1,
		T = 2,
		Other = Test | T,
		Another = 8,
	}
}