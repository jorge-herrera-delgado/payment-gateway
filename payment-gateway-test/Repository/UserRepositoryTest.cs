using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway_repository.Engine.Contract;
using payment_gateway_repository.Engine.Model;
using payment_gateway_repository.Model;
using payment_gateway_repository.Repository;

namespace payment_gateway_test.Repository
{
    [TestClass]
    public class UserRepositoryTest
    {
        private User _user;
        private List<User> _users;

        private Mock<INonSqlDataSource> _mockDataSource;

        [TestInitialize]
        public void Init()
        {
            _user = new User
            {
                FirstName = "Firstname",
                LastName = "Lastname",
                UserId = Guid.NewGuid(),
                UserLogin = new UserLogin
                {
                    Username = "TestUser",
                    Password = "123456"
                }
            };

            _users = new List<User> {_user};

            //Mock
            _mockDataSource = new Mock<INonSqlDataSource>();
            _mockDataSource.Setup(d => d.GetItem(It.IsAny<NonSqlSchema>(), It.IsAny<Expression<Func<User, bool>>>()))
                .Returns((NonSqlSchema s, Expression<Func<User, bool>> e) => e != null ? _user : null);
            _mockDataSource.Setup(d => d.GetItemAsync(It.IsAny<NonSqlSchema>(), It.IsAny<Expression<Func<User, bool>>>()))
                .Returns((NonSqlSchema s, Expression<Func<User, bool>> e) => Task.FromResult<User>(e != null ? _user : null));
            _mockDataSource.Setup(d => d.InsertAsync(It.IsAny<NonSqlSchema>(), It.IsAny<User>()))
                .Returns((NonSqlSchema s, User u) =>
                {
                    if (u == null)
                        throw new NullReferenceException("Test Null Exception for User");

                    return Task.FromResult(u.UserId != _user.UserId);
                });

            _mockDataSource.Setup(x =>
                x.UpdateAsync(It.IsAny<NonSqlSchema>(), It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Expression<Func<User, string>>>(), It.IsAny<string>())).ReturnsAsync(
                (NonSqlSchema s, Expression<Func<User, bool>> f, Expression<Func<User, string>> e, string v) =>
                {
                    if (v == null)
                        throw new NullReferenceException("Test Null Exception for User");

                    return v == true.ToString();
                });

        }

        [TestMethod]
        public void UserRepository_GetItem_Successful()
        {
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetItem(x => x.UserId == _user.UserId);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.UserId == _user.UserId);
        }

        [TestMethod]
        public void UserRepository_GetItem_Failed()
        {
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetItem(null);
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void UserRepository_GetItemAsync_Successful()
        {
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetItemAsync(x => x.UserId == _user.UserId).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.UserId == _user.UserId);
        }

        [TestMethod]
        public void UserRepository_GetItemAsync_Failed()
        {
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetItemAsync(null).Result;
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void UserRepository_GetMongoQueryable_Successful()
        {
            //Mock
            _mockDataSource.Setup(d => d.GetMongoQueryable<User>(It.IsAny<NonSqlSchema>())).Returns(_users.AsQueryable());
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetMongoQueryable();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.Any(x => x.UserId == _user.UserId));
        }

        [TestMethod]
        public void UserRepository_GetMongoQueryable_Failed()
        {
            //Mock
            _mockDataSource.Setup(d => d.GetMongoQueryable<User>(It.IsAny<NonSqlSchema>())).Returns<IQueryable<User>>(null);
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act
            var result = repo.GetMongoQueryable();
            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void UserRepository_AddItemAsync_User_No_Exists_Successful()
        {
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act
            //Simulates a Non Existing user in Storage
            var temp = new User();
            var result = repo.AddItemAsync(temp).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UserRepository_AddItemAsync_Already_Exists_Failed()
        {
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act
            var result = repo.AddItemAsync(_user).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRepository_AddItemAsync_NullReferenceException_Failed()
        {
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act and Assert
            Assert.ThrowsExceptionAsync<NullReferenceException>(() => repo.AddItemAsync(null), "Null Reference Exception. Parameter cannot be null.");
        }

        [TestMethod]
        public void UserRepository_UpdateItemAsync_User_No_Exists_Failed()
        {
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act
            //Simulates a Non Existing user in Storage
            var result = repo.UpdateItemAsync(_user, u => u.FirstName, false.ToString()).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserRepository_UpdateItemAsync_Already_Exists_Successful()
        {
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act
            var result = repo.UpdateItemAsync(_user, u => u.FirstName, true.ToString()).Result;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UserRepository_UpdateItemAsync_NullReferenceException_Failed()
        {
            //Arrange
            var repo = new UserRepository(_mockDataSource.Object);
            //Act and Assert
            Assert.ThrowsExceptionAsync<NullReferenceException>(() => repo.UpdateItemAsync(_user, u => u.FirstName, null), "Null Reference Exception. Parameter cannot be null.");
        }
    }
}
