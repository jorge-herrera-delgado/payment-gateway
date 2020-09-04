using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using payment_gateway.AssemblyRegister;
using payment_gateway.Model;
using payment_gateway.Services;
using payment_gateway.Services.Action.User;

namespace payment_gateway_test.ServiceTest
{
    [TestClass]
    public class ActionServiceTest
    {
        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void ActionService_Successful()
        {
            ////Mock

            ////var mockAction = new Mock<IAction>();
            ////mockAction.Setup(a => a.ProcessAction(It.IsAny<object>())).Returns(Task.FromResult((object)new RepoModel.User()));
            //var sc = new ServiceCollection();
            ////var mock = new Mock<IActionService>();
            //sc.RegisterActions();
            //sc.RegisterRepositories(new AppSettings{ConnectionString = string.Empty});
            //sc.RegisterValidation();
            //var sp = sc.BuildServiceProvider();
            ////Arrange
            //var service = new ActionService(sp);
            ////Act
            //var result = service.ProcessAction<LoginAction>(null).Result;
            ////Assert
            //Assert.IsNotNull(result);
        }
    }
}
