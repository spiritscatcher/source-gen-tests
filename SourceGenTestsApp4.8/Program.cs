using System;
using System.Linq;

namespace SourceGenTestsApp4._8
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// Console.WriteLine($"{nameof(TestEnum)}: {TestEnum.Two.ToName()} = {TestEnum.Two.ToValue()}");
			// Console.WriteLine($"{nameof(TestEnumUShort)} [{TestEnumUShort.Two.GetUnderlyingType()}]: {TestEnumUShort.Two.ToName()} = {TestEnumUShort.Two.ToValue()}; {TestEnumUShort.Two.ToDebugString()}");
			// Console.WriteLine($"{nameof(TestEnumUShortHelper)} [{TestEnumUShortHelper.UnderlyingType}]: {string.Join(", ", TestEnumUShortHelper.GetValues().Select(x => x.ToValue()))}; {string.Join(", ", TestEnumUShortHelper.GetNames())}");
		}
	}
}