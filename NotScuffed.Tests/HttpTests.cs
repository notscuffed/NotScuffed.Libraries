using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NotScuffed.Http;
using NUnit.Framework;

namespace NotScuffed.Tests
{
    public class HttpTests
    {
        [Test]
        public void TestTimeout()
        {
            var httpClient = Requester.CreateClient("https://httpstat.us");

            Assert.Throws<TimeoutException>(async () =>
            {
                await Requester.Get("/200")
                    .SetTimeout(TimeSpan.FromMilliseconds(50))
                    .AddParam("sleep", 100)
                    .Request(httpClient);
            }, "Request did not throw TimeoutException");
        }

        [Test]
        public async Task TestGoodResponse()
        {
            var httpClient = Requester.CreateClient("https://api.ipify.org");

            var response = await Requester.Get("/")
                .AddParam("format", "json")
                .Request(httpClient);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(content.Contains("\"ip\":"));
        }

        [Test]
        public void TestIfChecksForInvalidSSL()
        {
            Func<Task> action = async () =>
            {
                var httpClient = Requester.CreateClient("https://expired.badssl.com");

                await Requester.Get("/")
                    .Request(httpClient);
            };

            action.Should()
                .Throw<HttpRequestException>()
                .WithInnerException<AuthenticationException>();
        }

        [Test]
        public async Task TestIgnoreInvalidSSL()
        {
            var httpClient = Requester.CreateClient("https://expired.badssl.com", false);

            var response = await Requester.Get("/")
                .Request(httpClient);

            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestPost()
        {
            var httpClient = Requester.CreateClient("https://postman-echo.com");

            var response = await Requester.Post("/post")
                .AddPost("test", "123")
                .Request(httpClient);

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<PostmanResponse>(content);
            
            Assert.AreEqual("123", result.Form["test"]);
            Assert.AreEqual("application/x-www-form-urlencoded", result.Headers["content-type"]);
        }
    }
}