using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using NotScuffed.Http;
using NUnit.Framework;

namespace NotScuffed.Tests
{
    [TestFixture]
    public class HttpTests
    {
        [Test]
        public void TestTimeout()
        {
            var httpClient = Requester.CreateClient("https://httpstat.us");

            Assert.Throws<TimeoutException>(() =>
            {
                Requester.Get("/200")
                    .SetTimeout(TimeSpan.FromMilliseconds(50))
                    .AddQuery("sleep", 100)
                    .Request(httpClient).GetAwaiter().GetResult();
            }, "Request did not throw TimeoutException");
        }
        
        [Test]
        public void TestOperationCanceled()
        {
            var httpClient = Requester.CreateClient("https://httpstat.us");

            Assert.Throws<TaskCanceledException>(() =>
            {
                using var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(TimeSpan.FromMilliseconds(50));
                
                Requester.Get("/200")
                    .SetTimeout(TimeSpan.FromSeconds(1))
                    .AddQuery("sleep", 100)
                    .Request(httpClient, tokenSource.Token).GetAwaiter().GetResult();
                
            }, "Request did not throw TimeoutException");
        }

        [Test]
        public async Task TestGoodResponse()
        {
            var httpClient = Requester.CreateClient("https://api.ipify.org");

            var response = await Requester.Get("/")
                .AddQuery("format", "json")
                .Request(httpClient);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(content.Contains("\"ip\":"));
        }

        [Test]
        public async Task TestGoodResponseAbsolute()
        {
            var httpClient = Requester.CreateClient();

            var response = await Requester.Get("https://api.ipify.org")
                .AddQuery("format", "json")
                .Request(httpClient);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

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

        [Test]
        public async Task TestJsonPost()
        {
            var httpClient = Requester.CreateClient("https://postman-echo.com");

            var response = await Requester.Post("/post")
                .SetJsonBody("{\"key\":\"value\"}")
                .Request(httpClient);

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<PostmanResponse>(content);

            Assert.AreEqual("application/json", result.Headers["content-type"]);
            Assert.AreEqual("value", result.Data["key"].ToString());
        }
    }
}