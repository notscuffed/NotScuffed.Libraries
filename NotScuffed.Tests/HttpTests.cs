using System;
using System.Net.Http;
using System.Security.Authentication;
using FluentAssertions;
using NotScuffed.Http;
using NUnit.Framework;

namespace NotScuffed.Tests
{
    public class HttpTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestTimeout()
        {
            var httpClient = Requester.CreateClient("https://httpstat.us");

            Assert.Throws<TimeoutException>(() =>
            {
                Requester.Get("/200")
                    .SetTimeout(TimeSpan.FromMilliseconds(50))
                    .AddParam("sleep", 100)
                    .Request(httpClient).GetAwaiter().GetResult();
            }, "Request did not throw TimeoutException");
        }

        [Test]
        public void TestGoodResponse()
        {
            var httpClient = Requester.CreateClient("https://api.ipify.org");

            var response = Requester.Get("/")
                .AddParam("format", "json")
                .Request(httpClient).GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            Assert.IsTrue(content.Contains("\"ip\":"));
        }

        [Test]
        public void TestIfChecksForInvalidSSL()
        {
            Action action = () =>
            {
                var httpClient = Requester.CreateClient("https://expired.badssl.com");

                Requester.Get("/")
                    .Request(httpClient).GetAwaiter().GetResult();
            };

            action.Should()
                .Throw<HttpRequestException>()
                .WithInnerException<AuthenticationException>();
        }

        [Test]
        public void TestIgnoreInvalidSSL()
        {
            var httpClient = Requester.CreateClient("https://expired.badssl.com", false);

            var response = Requester.Get("/")
                .Request(httpClient).GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }
    }
}