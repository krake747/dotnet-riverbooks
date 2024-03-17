using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Xunit.Abstractions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace RiverBooks.OrderProcessing.Tests;

public sealed class InfrastructureDependencyTests(ITestOutputHelper outputHelper)
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(typeof(IOrderProcessingModuleMarker).Assembly)
        .Build();

    [Fact]
    public void DomainTypes_ShouldNotReferenceInfrastructure()
    {
        // Arrange
        var domainTypes = Types().That()
            .ResideInNamespace("RiverBooks.OrderProcessing.Domain.*", true)
            .As("OrderProcessing Domain Types");

        var infrastructureTypes = Types().That()
            .ResideInNamespace("RiverBooks.OrderProcessing.Infrastructure.*", true)
            .As("OrderProcessing Infrastructure Types");

        // Act
        PrintTypes(domainTypes, infrastructureTypes);

        // Assert
        domainTypes.Should().NotDependOnAny(infrastructureTypes).Check(Architecture);
    }

    /// <summary>
    ///     Used for debugging purposes
    /// </summary>
    /// <param name="domainTypes"></param>
    /// <param name="infrastructureTypes"></param>
    private void PrintTypes(GivenTypesConjunctionWithDescription domainTypes,
        GivenTypesConjunctionWithDescription infrastructureTypes)
    {
        // Debugging - Inspect classes and their dependencies
        foreach (var domainClass in domainTypes.GetObjects(Architecture))
        {
            outputHelper.WriteLine($"Domain Type: {domainClass.FullName}");
            foreach (var dependency in domainClass.Dependencies)
            {
                var targetType = dependency.Target;
                if (infrastructureTypes.GetObjects(Architecture).Any(infraClass => infraClass.Equals(targetType)))
                {
                    outputHelper.WriteLine($"  Depends on Infrastructure: {targetType.FullName}");
                }
            }
        }

        foreach (var infrastructureType in infrastructureTypes.GetObjects(Architecture))
        {
            outputHelper.WriteLine($"Infrastructure Types: {infrastructureType.FullName}");
        }
    }
}