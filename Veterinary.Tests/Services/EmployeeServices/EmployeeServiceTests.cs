using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Veterinary.Domain.Models;
using Veterinary.Services.EmployeeServices;
using Xunit;

namespace Veterinary.Tests.Services.EmployeeServices;

[Collection("EmployeeService")]
public class EmployeeServiceTests
{
    #region snippet_Properties

    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    private readonly Mock<ILogger<EmployeeService>> _mockLogger;

    #endregion

    #region snippet_Constructors

    public EmployeeServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLocalStorageService = new Mock<ILocalStorageService>();
        _mockLogger = new Mock<ILogger<EmployeeService>>();
    }

    #endregion

    #region snippet_Tests

    [Fact]
    public async Task GetAllAsyncShouldThrowException()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();
        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>
            (
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError))
            .Verifiable();

        var employeeService = new EmployeeService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );

        await Assert.ThrowsAsync<Exception>(async ()
            => await employeeService.GetAllAsync(offset: 0, limit: 10));
    }

    [Fact]
    public async Task GetAllAsyncShouldReturn200()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();
        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>
            (
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{""next"": 0, ""previous"": 0, ""total"": 1, ""data"": []}")
            })
            .Verifiable();

        var employeeService = new EmployeeService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var employees = await employeeService.GetAllAsync(offset: 0, limit: 10);

        _mockHttpClientFactory.Verify(x => x.CreateClient(It.IsAny<string>()));
        mockDelegatingHandler
            .Protected()
            .Verify
            (
                "SendAsync",
                Times.Exactly(1),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );

        Assert.Equal(0, employees.Next);
        Assert.Equal(0, employees.Previous);
        Assert.Equal(1, employees.Total);
        Assert.Empty(employees.Data);
    }

    [Fact]
    public async Task GetByIdAsyncShouldThrowException()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();
        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>
            (
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError))
            .Verifiable();

        var employeeService = new EmployeeService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        
        await Assert.ThrowsAsync<Exception>(async ()
            => await employeeService.GetByIdAsync(id: "dummyid"));
    }

    [Fact]
    public async Task GetByIdAsyncShouldReturn200()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        var dummyContent = @"{""employee_id"": ""dummyid"", ""email"": ""dummy@example.com""," +
            @"""name"": ""dummy"", ""last_name"": ""dummy last name""}";

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();
        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>
            (
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(dummyContent)
            })
            .Verifiable();

        var employeeService = new EmployeeService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var employee = await employeeService.GetByIdAsync(id: "dummyid");

        _mockHttpClientFactory.Verify(x => x.CreateClient(It.IsAny<string>()));
        mockDelegatingHandler
            .Protected()
            .Verify
            (
                "SendAsync",
                Times.Exactly(1),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );

        Assert.Equal("dummyid", employee.EmployeeId);
        Assert.Equal("dummy@example.com", employee.Email);
        Assert.Equal("dummy", employee.Name);
        Assert.Equal("dummy last name", employee.LastName);
    }

    [Fact]
    public async Task GetAsyncShouldReturnNull()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();
        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>
            (
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError))
            .Verifiable();

        var employeeService = new EmployeeService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var employee = await employeeService.GetAsync();

        _mockHttpClientFactory.Verify(x => x.CreateClient(It.IsAny<string>()));
        mockDelegatingHandler
            .Protected()
            .Verify
            (
                "SendAsync",
                Times.Exactly(1),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );

        Assert.Null(employee);
    }

    [Fact]
    public async Task GetAsyncShouldReturnProfile()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        var dummyContent = @"{""employee_id"": ""dummyid"", ""email"": ""dummy@example.com""," +
            @"""name"": ""dummy"", ""last_name"": ""dummy last name""}";

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();
        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>
            (
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(dummyContent)
            })
            .Verifiable();

        var employeeService = new EmployeeService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var employee = await employeeService.GetAsync();

        _mockHttpClientFactory.Verify(x => x.CreateClient(It.IsAny<string>()));
        mockDelegatingHandler
            .Protected()
            .Verify
            (
                "SendAsync",
                Times.Exactly(1),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );

        Assert.Equal("dummyid", employee.EmployeeId);
        Assert.Equal("dummy@example.com", employee.Email);
        Assert.Equal("dummy", employee.Name);
        Assert.Equal("dummy last name", employee.LastName);
    }

    [Fact]
    public async Task UpdateByIdAsyncShouldThrowException()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();
        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>
            (
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError))
            .Verifiable();

        var employeeService = new EmployeeService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );

        await Assert.ThrowsAsync<Exception>(async () =>
        {
            var employee = new UpdateEmployeeProfileDto
            {
                Name = "dummy name",
                LastName = "dummy last name"
            };
            await employeeService.UpdateByIdAsync(id: "", employee);
        });
    }

    [Fact]
    public async Task UpdateByIdAsyncShouldReturn200()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        var dummyContent = @"{""employee_id"": ""dummyid"", ""email"": ""dummy@example.com""," +
            @"""name"": ""dummy"", ""last_name"": ""dummy last name""}";

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();
        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>
            (
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(dummyContent)
            })
            .Verifiable();

        var employeeService = new EmployeeService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var employee = new UpdateEmployeeProfileDto
        {
            Name = "dummy name",
            LastName = "dummy last name"
        };
        var updateResult = await employeeService.UpdateByIdAsync(id: "", employee);

        _mockHttpClientFactory.Verify(x => x.CreateClient(It.IsAny<string>()));
        mockDelegatingHandler
            .Protected()
            .Verify
            (
                "SendAsync",
                Times.Exactly(1),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );

        Assert.Equal("dummy name", employee.Name);
        Assert.Equal("dummy last name", employee.LastName);
    }

    #endregion
}
