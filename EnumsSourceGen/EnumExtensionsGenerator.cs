using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using EnumsSourceGen.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EnumsSourceGen;

[Generator]
public class EnumExtensionsGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		// Do a simple filter for enums
		IncrementalValuesProvider<EnumDeclarationSyntax> enumDeclarations = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
				transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx) // sect the enum with the [EnumOptimizedAttribute] attribute
			)
			.Where(static m => m is not null)!; // filter out attributed enums that we don't care about

		// Combine the selected enums with the `Compilation`
		IncrementalValueProvider<(Compilation, ImmutableArray<EnumDeclarationSyntax>)> compilationAndEnums = context.CompilationProvider.Combine(enumDeclarations.Collect());

		// Generate the source using the compilation and enums
		context.RegisterSourceOutput(compilationAndEnums, SourceOutputRegistration);
	}

	private static readonly string AttributeFullName = typeof(EnumOptimizedAttribute).FullName!;

	private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
	{
		var result = node is EnumDeclarationSyntax {AttributeLists.Count: > 0} enumDeclaration;
		return result;
	}

	private static EnumDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
	{
		// we know the node is a EnumDeclarationSyntax thanks to IsSyntaxTargetForGeneration
		var enumDeclarationSyntax = (EnumDeclarationSyntax) context.Node;

		// loop through all the attributes on the method
		for (var ial = 0; ial < enumDeclarationSyntax.AttributeLists.Count; ial++)
		{
			var attributeListSyntax = enumDeclarationSyntax.AttributeLists[ial];
			for (var index = 0; index < attributeListSyntax.Attributes.Count; index++)
			{
				var attributeSyntax = attributeListSyntax.Attributes[index];
				if (context.SemanticModel.GetSymbolInfo(attributeSyntax)
						.Symbol is not IMethodSymbol attributeSymbol)
				{
					// weird, we couldn't get the symbol, ignore it
					continue;
				}

				INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;

				string fullName = attributeContainingTypeSymbol.ToDisplayString();

				// Is the attribute the [EnumOptimizedAttribute] attribute?
				if (fullName == AttributeFullName)
				{
					// return the enum
					return enumDeclarationSyntax;
				}
			}
		}

		// we didn't find the attribute we were looking for
		return null;
	}

	private void SourceOutputRegistration(SourceProductionContext spc, (Compilation, ImmutableArray<EnumDeclarationSyntax>) source)
	{
		Execute(source.Item1, source.Item2, spc);
	}

	private void Execute(Compilation compilation, ImmutableArray<EnumDeclarationSyntax> enums, SourceProductionContext context)
	{
		if (enums.IsDefaultOrEmpty)
		{
			// nothing to do yet
			return;
		}

		// I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
		IEnumerable<EnumDeclarationSyntax> distinctEnums = enums.Distinct();

		// Convert each EnumDeclarationSyntax to an EnumToGenerate
		List<EnumToGenerate> enumsToGenerate = GetTypesToGenerate(compilation, distinctEnums, context.CancellationToken);

		// If there were errors in the EnumDeclarationSyntax, we won't create an
		// EnumToGenerate for it, so make sure we have something to generate
		if (enumsToGenerate.Count > 0)
		{
			// generate the source code and add it to the output
			string result = GenerateExtensionClass(enumsToGenerate);
			context.AddSource("EnumExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
		}
	}

	private static List<EnumToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<EnumDeclarationSyntax> enums, CancellationToken ct)
	{
		// Create a list to hold our output
		var enumsToGenerate = new List<EnumToGenerate>();

		// Get the semantic representation of our marker attribute
		INamedTypeSymbol? enumAttribute = compilation.GetTypeByMetadataName(AttributeFullName);

		if (enumAttribute == null)
		{
			// If this is null, the compilation couldn't find the marker attribute type
			// which suggests there's something very wrong! Bail out..
			return enumsToGenerate;
		}

		foreach (EnumDeclarationSyntax enumDeclarationSyntax in enums)
		{
			// stop if we're asked to
			ct.ThrowIfCancellationRequested();

			// Get the semantic representation of the enum syntax
			SemanticModel semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
			if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
			{
				// something went wrong, bail out
				continue;
			}

			// Get the full type name of the enum e.g. Colour, or OuterClass<T>.Colour if it was nested in a generic type (for example)
			string enumName = enumSymbol.ToString();

			// Get all the members in the enum
			ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
			var members = new List<string>(enumMembers.Length);

			// Get all the fields from the enum, and add their name to the list
			foreach (ISymbol member in enumMembers)
			{
				if (member is IFieldSymbol { ConstantValue: not null })
				{
					members.Add(member.Name);
				}
			}

			// Create an EnumToGenerate for use in the generation phase
			enumsToGenerate.Add(new EnumToGenerate(enumName, members));
		}

		return enumsToGenerate;
	}

	private static string GenerateExtensionClass(List<EnumToGenerate> enumsToGenerate)
	{
		var sb = new StringBuilder();
		sb.Append(@"
namespace EnumsSourceGen.EnumGenerators
{
    public static partial class EnumExtensions
    {");
		foreach(var enumToGenerate in enumsToGenerate)
		{
			sb.Append(@"
                public static string ToStringFast(this ").Append(enumToGenerate.Name).Append(@" value)
                    => value switch
                    {");
			foreach (var member in enumToGenerate.Values)
			{
				sb.Append(@"
                ").Append(enumToGenerate.Name).Append('.').Append(member)
					.Append(" => nameof(")
					.Append(enumToGenerate.Name).Append('.').Append(member).Append("),");
			}

			sb.Append(@"
                    _ => value.ToString(),
                };
");
		}
    
		sb.Append(@"
    }
}");

		return sb.ToString();
	}
}

public readonly struct EnumToGenerate
{
	public readonly string Name;
	public readonly List<string> Values;

	public EnumToGenerate(string name, List<string> values)
	{
		Name   = name;
		Values = values;
	}
}