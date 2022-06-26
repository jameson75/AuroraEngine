using System;
using CipherPark.Aurora.Core.Services;
using NUnit.Framework;
using Moq;
using FluentAssertions;

namespace Aurora.Core.UnitTests
{
    public class ServiceTableUnitTests
    {
        [Test]
        public void When_NewServiceRegistered_Then_ServiceIsAccessible()
        {         
            // Arrange
            ServiceTable sut = new ServiceTable();
            var mockService = Mock.Of<IGameStateService>();

            // Act
            sut.RegisterService(mockService);
            var result = sut.GetService<IGameStateService>();

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void When_ExistingServiceUnregistered_Then_ServiceIsInaccessible()
        {
            // Arrange
            ServiceTable sut = new ServiceTable();
            var mockService = Mock.Of<IGameStateService>();
            sut.RegisterService(mockService);

            // Act
            sut.UnregisterService<IGameStateService>();
            var result = sut.GetService<IGameStateService>();

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void When_AccessUnregisteredService_Then_ResultIsNull()
        {
            // Arrange
            ServiceTable sut = new ServiceTable();
            
            // Act
            var result = sut.GetService<IGameStateService>();

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void When_ExistingServiceRegistered_Then_ExceptionIsThrown()
        {
            // Arrange
            ServiceTable sut = new ServiceTable();
            var mockService1 = Mock.Of<IGameStateService>();
            var mockService2 = Mock.Of<IGameStateService>();
            sut.RegisterService(mockService1);

            // Act            
            Action action = () => sut.RegisterService(mockService2);

            // Assert
            action.Should().Throw<Exception>();
        }
    }
}