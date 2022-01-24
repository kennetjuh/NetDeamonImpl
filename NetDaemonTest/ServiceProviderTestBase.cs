using Moq;
using Microsoft.Extensions.DependencyInjection;
using NetDaemon.HassModel.Entities;

namespace NetDaemonTest
{
    public abstract class ServiceProviderTestBase
    {
        internal readonly Mock<IServiceProvider> serviceProviderMock = new(MockBehavior.Strict);
        internal readonly Mock<IServiceScope> serviceScopeMock = new(MockBehavior.Strict);
        internal readonly Mock<IServiceScopeFactory> serviceScopeFactoryMock = new(MockBehavior.Strict);
        internal readonly Mock<IHaContext> haContextMock = new(MockBehavior.Strict);
        internal readonly IEntities entities;

        public ServiceProviderTestBase()
        {
            entities = new Entities(haContextMock.Object);
            SetupMocks();
        }

        internal virtual void SetupMocks()
        {
            serviceProviderMock.Reset();
            serviceScopeMock.Reset();
            haContextMock.Reset();

            serviceProviderMock.Setup(x => x.GetService(typeof(IHaContext))).Returns(haContextMock.Object);
            //serviceScopeMock.Setup(x => x.Dispose());
            serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
            serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);
            serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactoryMock.Object);
        }

        internal virtual void VerifyAllMocks()
        {
            serviceProviderMock.VerifyAll();
            serviceScopeMock.VerifyAll();
            haContextMock.VerifyAll();
        }
    }
}
