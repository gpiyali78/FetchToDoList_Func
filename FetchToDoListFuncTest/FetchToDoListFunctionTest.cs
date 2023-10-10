using FetchToDoListFunc;
using FetchToDoListFunc.Model;
using FetchToDoListFunc.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchToDoListFuncTest
{
    internal class FetchToDoListFunctionTest
    {
        private FetchToDoListFunction _fetchToDoListFunc;
        private Mock<IFetchToDoList> _fetchRepoMock;
        private Mock<ILogger> _loggerMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<IConfiguration> _config;

        [SetUp]
        public void Setup()
        {
            _fetchRepoMock = new Mock<IFetchToDoList>();
            _loggerMock = new Mock<ILogger>();
            _httpRequestMock = new Mock<HttpRequest>();

            _fetchToDoListFunc = new FetchToDoListFunction(_fetchRepoMock.Object, _config.Object);
        }

        [Test]
        public async Task GetAllTask_ValidJWT_ReturnsOkResultWithTaskList()
        {
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6InBpeWFsaSIsInJvbGUiOiJhZG1pbiJ9.13uvoezlooecySJYjOnmOhp_hjGnxDXwnpAgld_-2xs" } });


            // Mock the behavior of IFetchToDoList
            var fakeTaskList = new List<TaskList>() { /* Initialize your fake data here */ };
            _fetchRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(fakeTaskList);

            // Act
            var result = await _fetchToDoListFunc.GetAllTask(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetAllTask_InvalidJWT_ReturnsUnauthorizedResult()
        {
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "InvalidToken" } });


            // Act
            var result = await _fetchToDoListFunc.GetAllTask(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            var okResult = result.Result as  UnauthorizedResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task GetAllTask_ExceptionThrown_ReturnsBadRequestResult()
        {
            // Arrange
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6InBpeWFsaSIsInJvbGUiOiJhZG1pbiJ9.13uvoezlooecySJYjOnmOhp_hjGnxDXwnpAgld_-2xs" } });


            // Mock the behavior of IFetchToDoList to throw an exception
            _fetchRepoMock.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Fake exception"));

            // Act
            var result = await _fetchToDoListFunc.GetAllTask(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            var exception = result.Result as BadRequestObjectResult;
            Assert.That(exception?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task CreateAsync_ValidJWT_ReturnsOkResultWithTaskList()
        {
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6InBpeWFsaSIsInJvbGUiOiJhZG1pbiJ9.13uvoezlooecySJYjOnmOhp_hjGnxDXwnpAgld_-2xs" } });

            TaskList objTaskList = new()
            {
                TaskName = "Test Task",
                Description = "Test Description",
                TaskStartDate = DateTime.Now,
                TaskEndDate = DateTime.Now.AddDays(1),
                TaskStatus = "In Progress",
                TotalEffortRequired = 10
            };

            var jsonBody = JsonConvert.SerializeObject(objTaskList);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(jsonBody);
            writer.Flush();
            stream.Position = 0;
            _httpRequestMock.Setup(req => req.Body).Returns(stream);

            // Mock the behavior of IFetchToDoList
            _fetchRepoMock.Setup(repo => repo.CreateAsync(objTaskList));

            // Act
            var result = await _fetchToDoListFunc.CreateAsync(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task CreateAsync_InvalidJWT_ReturnsUnauthorizedResult()
        {
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "InvalidToken" } });


            // Act
            var result = await _fetchToDoListFunc.CreateAsync(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            var okResult = result.Result as UnauthorizedResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task CreateAsync_ExceptionThrown_ReturnsBadRequestResult()
        {
            // Arrange
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6InBpeWFsaSIsInJvbGUiOiJhZG1pbiJ9.13uvoezlooecySJYjOnmOhp_hjGnxDXwnpAgld_-2xs" } });


            // Mock the behavior of IFetchToDoList to throw an exception
            _fetchRepoMock.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Fake exception"));

            // Act
            var result = await _fetchToDoListFunc.GetAllTask(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            var exception = result.Result as BadRequestObjectResult;
            Assert.That(exception?.StatusCode, Is.EqualTo(400));
        }
    }
}

