using FetchToDoListFunc;
using FetchToDoListFunc.Model;
using FetchToDoListFunc.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
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
       // private IConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _fetchRepoMock = new Mock<IFetchToDoList>();
            _loggerMock = new Mock<ILogger>();
            _httpRequestMock = new Mock<HttpRequest>();

            _fetchToDoListFunc = new FetchToDoListFunction(_fetchRepoMock.Object);
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
        public async Task GetTaskDetails_ValidJWT_ReturnsOkResultWithTaskList()
        {
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6InBpeWFsaSIsInJvbGUiOiJhZG1pbiJ9.13uvoezlooecySJYjOnmOhp_hjGnxDXwnpAgld_-2xs" } });


            // Mock the behavior of IFetchToDoList
            var fakeTaskList = new TaskList() { /* Initialize your fake data here */ };
            _fetchRepoMock.Setup(repo => repo.GetTaskDetailsByTaskNameAsync("Task")).ReturnsAsync(fakeTaskList);

            // Act
            var result = await _fetchToDoListFunc.GetTaskDetails(_httpRequestMock.Object, _loggerMock.Object, "Task");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetTaskDetails_InvalidJWT_ReturnsUnauthorizedResult()
        {
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "InvalidToken" } });


            // Act
            var result = await _fetchToDoListFunc.GetTaskDetails(_httpRequestMock.Object, _loggerMock.Object, "Task");

            // Assert
            var okResult = result.Result as UnauthorizedResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task GetTaskDetails_ExceptionThrown_ReturnsBadRequestResult()
        {
            // Arrange
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6InBpeWFsaSIsInJvbGUiOiJhZG1pbiJ9.13uvoezlooecySJYjOnmOhp_hjGnxDXwnpAgld_-2xs" } });


            // Mock the behavior of IFetchToDoList to throw an exception
            _fetchRepoMock.Setup(repo => repo.GetTaskDetailsByTaskNameAsync("Task")).ThrowsAsync(new Exception("Fake exception"));

            // Act
            var result = await _fetchToDoListFunc.GetTaskDetails(_httpRequestMock.Object, _loggerMock.Object,"Task");

            // Assert
            var exception = result.Result as BadRequestObjectResult;
            Assert.That(exception?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task UpdateTaskStatus_ValidJWT_ReturnsOkResultWithTaskList()
        {
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6InBpeWFsaSIsInJvbGUiOiJhZG1pbiJ9.13uvoezlooecySJYjOnmOhp_hjGnxDXwnpAgld_-2xs" } });


            // Mock the behavior of IFetchToDoList
            string objectIdString = "5f68c3ee1c8b4d09d48f1344";
            ObjectId objectId = ObjectId.Parse(objectIdString);
            _fetchRepoMock.Setup(repo => repo.UpdateTaskStatusAsync(objectId));

            // Act
            var result = await _fetchToDoListFunc.UpdateTaskStatus(_httpRequestMock.Object, _loggerMock.Object, objectIdString);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task UpdateTaskStatus_InvalidJWT_ReturnsUnauthorizedResult()
        {
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "InvalidToken" } });


            // Act
            var result = await _fetchToDoListFunc.UpdateTaskStatus(_httpRequestMock.Object, _loggerMock.Object, "Task");

            // Assert
            var okResult = result.Result as UnauthorizedResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task UpdateTaskStatus_ExceptionThrown_ReturnsBadRequestResult()
        {
            // Arrange
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6InBpeWFsaSIsInJvbGUiOiJhZG1pbiJ9.13uvoezlooecySJYjOnmOhp_hjGnxDXwnpAgld_-2xs" } });


            string objectIdString = "5f68c3ee1c8b4d09d48f1344";
            ObjectId objectId = ObjectId.Parse(objectIdString);
            _fetchRepoMock.Setup(repo => repo.UpdateTaskStatusAsync(objectId)).ThrowsAsync(new Exception("Fake exception")); ;

            // Act
            var result = await _fetchToDoListFunc.UpdateTaskStatus(_httpRequestMock.Object, _loggerMock.Object, objectIdString);

            // Assert
            var exception = result.Result as BadRequestObjectResult;
            Assert.That(exception?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task DeleteTask_ValidJWT_ReturnsOkResultWithTaskList()
        {
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6InBpeWFsaSIsInJvbGUiOiJhZG1pbiJ9.13uvoezlooecySJYjOnmOhp_hjGnxDXwnpAgld_-2xs" } });


            // Mock the behavior of IFetchToDoList
            string objectIdString = "5f68c3ee1c8b4d09d48f1344";
            ObjectId objectId = ObjectId.Parse(objectIdString);
            _fetchRepoMock.Setup(repo => repo.DeleteTaskAsync(objectId));

            // Act
            var result = await _fetchToDoListFunc.DeleteTask(_httpRequestMock.Object, _loggerMock.Object, objectIdString);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task DeleteTask_InvalidJWT_ReturnsUnauthorizedResult()
        {
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "InvalidToken" } });


            // Act
            var result = await _fetchToDoListFunc.DeleteTask(_httpRequestMock.Object, _loggerMock.Object, "Task");

            // Assert
            var okResult = result as UnauthorizedResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task DeleteTask_ExceptionThrown_ReturnsBadRequestResult()
        {
            // Arrange
            // Arrange
            _httpRequestMock.Setup(req => req.Headers).Returns(new HeaderDictionary { { "Authorization", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6InBpeWFsaSIsInJvbGUiOiJhZG1pbiJ9.13uvoezlooecySJYjOnmOhp_hjGnxDXwnpAgld_-2xs" } });


            string objectIdString = "5f68c3ee1c8b4d09d48f1344";
            ObjectId objectId = ObjectId.Parse(objectIdString);
            _fetchRepoMock.Setup(repo => repo.DeleteTaskAsync(objectId)).ThrowsAsync(new Exception("Fake exception")); ;

            // Act
            var result = await _fetchToDoListFunc.DeleteTask(_httpRequestMock.Object, _loggerMock.Object, objectIdString);

            // Assert
            var exception = result as BadRequestObjectResult;
            Assert.That(exception?.StatusCode, Is.EqualTo(400));
        }
    }
}

