using FetchToDoListFunc.Model;
using FetchToDoListFunc.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchToDoListFuncTest.Repository
{
    internal class FetchToDoListTest
    {
        private Mock<IMongoCollection<TaskList>> _mockCollection;
        private Mock<IMongoDatabase> _mockDatabase;
        private FetchToDoList _fetchToDoList;

        [SetUp]
        public void Setup()
        {
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockCollection = new Mock<IMongoCollection<TaskList>>();

            // Configure the mock database to return the mock collection
            _mockDatabase.Setup(db => db.GetCollection<TaskList>("todolist", null))
                         .Returns(_mockCollection.Object);
            _fetchToDoList = new FetchToDoList(_mockDatabase.Object);
        }

        [Test]
        public async Task CreateAsync_ValidTaskList_ReturnsOkResult()
        {
            // Arrange
            var taskList = new TaskList { /* Initialize with valid data */ };

            _mockCollection.Setup(m => m.InsertOneAsync(It.IsAny<TaskList>(), null, default));

            // Act
            var result = await _fetchToDoList.CreateAsync(taskList);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.EqualTo("Task Added Successfully"));
        }

        [Test]
        public async Task CreateAsync_ValidTaskList_ThrowsException()
        {
            // Arrange
            var taskList = new TaskList { /* Initialize with valid data */ };

            _mockCollection.Setup(m => m.InsertOneAsync(It.IsAny<TaskList>(), null, default)).ThrowsAsync(new Exception("Some error message")); ;

            // Act
            var result = await _fetchToDoList.CreateAsync(taskList);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(badRequestResult.Value, Is.EqualTo("Some error message"));
        }


    }
}
