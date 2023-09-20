using System;
using EnumsSourceGen.EnumGenerators;

namespace SourceGenTestsApp
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");
			Console.WriteLine(TestEnum.One.ToStringFast());
		}
	}
}