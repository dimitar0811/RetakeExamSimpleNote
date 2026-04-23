using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;
using The_SimpleNotes_System.Models;

namespace The_SimpleNotes_System
{
    [TestFixture]
    public class Tests
    {
        private RestClient client;
        private static string createdNoteId;

        private const string BaseUrl = "http://144.91.123.158:5005/api";
        private const string StaticToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKd3RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiI1MDg0Yjk0Yy1lMDBlLTQzNDYtODc5Mi1hZjBkYzA3MWNlNmUiLCJpYXQiOiIwNC8yMy8yMDI2IDA2OjIyOjE1IiwiVXNlcklkIjoiMjA4YjMzNGYtMDRiYy00OGVhLTliMWQtNDgzY2QyOWYyMjQwIiwiRW1haWwiOiJkaW1hMjFAZXhhbXBsZS5jb20iLCJVc2VyTmFtZSI6IkRpbWkwODExIiwiZXhwIjoxNzc2OTQ2OTM1LCJpc3MiOiJTaW1wbGVOb3Rlc19BcHBfU29mdFVuaSIsImF1ZCI6IlNpbXBsZU5vdGVzX1dlYkFQSV9Tb2Z0VW5pIn0.C8liry29aH5Fd-8rEh0hXPVZHtAlw8FYiqPKvlho5Bo";
        private const string Email = "dima21@example.com";
        private const string Password = "123456";

        [OneTimeSetUp]
        public void Setup()
        {
            var tempClient = new RestClient(BaseUrl);

            var request = new RestRequest("/User/Authorization", Method.Post);
            request.AddJsonBody(new
            {
                email = Email,
                password = Password
            });

            var response = tempClient.Execute(request);

            var json = JsonDocument.Parse(response.Content);
            var token = json.RootElement.GetProperty("accessToken").GetString();

            var options = new RestClientOptions(BaseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };

            client = new RestClient(options);
        }

        // 1.3
        [Test, Order(1)]
        public void CreateNoteWithoutRequiredFields_ShouldReturnBadRequest()
        {
            var request = new RestRequest("/Note/Create", Method.Post);

            request.AddJsonBody(new { });

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // 1.4
        [Test, Order(2)]
        public void CreateNote_ShouldSucceed()
        {
            var request = new RestRequest("/Note/Create", Method.Post);

            request.AddJsonBody(new
            {
                title = "Valid Title",
                description = "This is a valid description with more than thirty characters.",
                status = "New"
            });

            var response = client.Execute(request);
            var json = JsonDocument.Parse(response.Content);

            var msg = json.RootElement.GetProperty("msg").GetString();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(msg, Is.EqualTo("Note created successfully!"));
        }

        // 1.5
        [Test, Order(3)]
        public void GetAllNotes_ShouldReturnNotes()
        {
            var request = new RestRequest("/Note/AllNotes", Method.Get);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var notes = JsonSerializer.Deserialize<List<NoteDto>>(
                JsonDocument.Parse(response.Content)
                .RootElement.GetProperty("allNotes")
                .GetRawText()
            );

            Assert.That(notes.Count, Is.GreaterThan(0));

            createdNoteId = notes.Last().Id;

            Assert.That(createdNoteId, Is.Not.Null.And.Not.Empty);
        }

        // 1.6
        [Test, Order(4)]
        public void EditNote_ShouldSucceed()
        {
            var request = new RestRequest($"/Note/Edit/{createdNoteId}", Method.Put);

            request.AddJsonBody(new
            {
                title = "Edited Title",
                description = "Edited description with enough characters to pass validation.",
                status = "Done"
            });

            var response = client.Execute(request);
            var json = JsonDocument.Parse(response.Content);

            var msg = json.RootElement.GetProperty("msg").GetString();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(msg, Is.EqualTo("Note edited successfully!"));
        }

        // 1.7
        [Test, Order(5)]
        public void DeleteNote_ShouldSucceed()
        {
            var request = new RestRequest($"/Note/Delete/{createdNoteId}", Method.Delete);

            var response = client.Execute(request);
            var json = JsonDocument.Parse(response.Content);

            var msg = json.RootElement.GetProperty("msg").GetString();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(msg, Is.EqualTo("Note deleted successfully!"));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            this.client?.Dispose();
        }
    }

    
}