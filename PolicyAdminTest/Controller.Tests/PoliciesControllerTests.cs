using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PolicyAdmin.API.Controllers;
using PolicyAdmin.API.DTOs;
using PolicyAdmin.API.Repository;

namespace PolicyAdmin.Tests.Controllers
{
    [TestFixture]
    public class PoliciesControllerTests
    {
        private Mock<IpolicyRepository> _repo;
        private PoliciesController _controller;

        [SetUp]
        public void Setup()
        {
            _repo = new Mock<IpolicyRepository>();
            _controller = new PoliciesController(_repo.Object);
        }

        [Test]
        public async Task GetPolicies_ReturnsOk()
        {
            // Arrange
            _repo.Setup(x => x.GetAllPoliciesAsync(null))
                 .ReturnsAsync(new List<PolicyDto>());

            // Act
            var result = await _controller.GetPolicies(null);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetPolicyByNumber_ReturnsNotFound()
        {
            // Arrange
            _repo.Setup(x => x.GetPolicyByNumberAsync("INVALID"))
                 .ReturnsAsync((PolicyDto?)null);

            // Act
            var result = await _controller.GetPolicyByNumber("INVALID");

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task CreatePolicy_ReturnsCreated()
        {
            // Arrange
            var dto = new CreatePolicyDto();

            _repo.Setup(x => x.CreatePolicyAsync(It.IsAny<CreatePolicyDto>()))
                 .ReturnsAsync(new PolicyDto
                 {
                     PolicyNumber = "POL001"
                 });

            // Act
            var result = await _controller.CreatePolicy(dto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        }

        [Test]
        public async Task RenewPolicy_ReturnsOk()
        {
            // Arrange
            var dto = new RenewPolicyDto();

            _repo.Setup(x => x.RenewPolicyAsync("POL001", dto))
                 .ReturnsAsync(new PolicyDto());

            // Act
            var result = await _controller.RenewPolicy("POL001", dto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task CancelPolicy_ReturnsOk()
        {
            // Arrange
            var dto = new CancelPolicyDto();

            _repo.Setup(x => x.CancelPolicyAsync("POL001", dto))
                 .ReturnsAsync(new PolicyDto());

            // Act
            var result = await _controller.CancelPolicy("POL001", dto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }
    }
}