/* using Blog.Application.Helpers;
using FluentAssertions;
using Xunit;

namespace Blog.Tests.Helpers;

public class SlugHelperTests
{
    [Theory]
    [InlineData("Merhaba Dünya", "merhaba-dunya")]
    [InlineData("Clean Architecture Nedir?", "clean-architecture-nedir")]
    [InlineData("  C#  in .NET 6  ", "c-in-net-6")]
    [InlineData("ÇĞIÖŞÜ", "cgiosu")]
    public void Generate_ShouldProduceCorrectSlug(string input, string expected)
    {
        var result = SlugHelper.Generate(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Generate_WithEmptyInput_ShouldReturnEmpty(string? input)
    {
        var result = SlugHelper.Generate(input!);
        result.Should().BeEmpty();
    }
} */