using FluentAssertions;
using NetArchTest.Rules;

namespace CleanArch.ArchitectureTests;

public class LayerDependencyTests
{
    private const string DomainNamespace = "CleanArch.Domain";
    private const string ApplicationNamespace = "CleanArch.Application";
    private const string InfrastructureNamespace = "CleanArch.Infrastructure";
    private const string WebApiNamespace = "CleanArch.WebApi";

    [Fact]
    public void Domain_ShouldNotDependOnOuterLayers()
    {
        var result = Types.InAssembly(typeof(Domain.Entities.Product).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, WebApiNamespace)
            .GetResult();

        AssertRule(result);
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructureOrWebApi()
    {
        var result = Types.InAssembly(typeof(Application.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, WebApiNamespace)
            .GetResult();

        AssertRule(result);
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnWebApi()
    {
        var result = Types.InAssembly(typeof(Infrastructure.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn(WebApiNamespace)
            .GetResult();

        AssertRule(result);
    }

    private static void AssertRule(TestResult result)
    {
        result.IsSuccessful.Should().BeTrue(
            "the following types violate the layer dependency rule: {0}",
            string.Join(", ", result.FailingTypeNames ?? []));
    }
}
