using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using payment_gateway.Services;
using payment_gateway.Services.Action.Engine;

namespace payment_gateway_test.ServiceTest
{
    [TestClass]
    public class ActionServiceTest
    {
        [TestMethod]
        public void ActionService_AggregateException_Failed()
        {
            //Mock
            var mockAction = new Mock<IAction>();
            mockAction.Setup(c => c.ProcessAction(string.Empty)).Returns(Task.FromResult(default(object)));
            //Arrange
            var sc = new ServiceCollection();
            sc.AddSingleton(provider => mockAction.Object);
            var sp = sc.BuildServiceProvider();
            var service = new ActionService(sp);
            //Act and Assert
            Assert.ThrowsExceptionAsync<AggregateException>(() => service.ProcessAction<IAction>(string.Empty), "Not found or not implemented.");
        }
    }
}
