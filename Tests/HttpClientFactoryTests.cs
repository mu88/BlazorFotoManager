using FluentAssertions;
using FotoManagerLogic.Business;
using NUnit.Framework;

namespace Tests;

[Category("Unit")]
public class HttpClientFactoryTests
{
    [Test]
    public void CreateClient()
    {
        var testee = new HttpClientFactory();

        testee.CreateClient().Should().NotBeNull();
    }
}