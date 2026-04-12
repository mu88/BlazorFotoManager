using FluentAssertions;
using FotoManager;
using Microsoft.Extensions.Localization;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Unit;

[Category("Unit")]
public class TranslatorTests
{
    [Test]
    public void Translate_ReturnsLocalizedString()
    {
        // Arrange
        var localizer = Substitute.For<IStringLocalizer<Translator>>();
        localizer["Hello"].Returns(new LocalizedString("Hello", "Hallo"));
        var testee = new Translator(localizer);

        // Act
        var result = testee.Translate("Hello");

        // Assert
        result.Should().Be("Hallo");
    }
}
