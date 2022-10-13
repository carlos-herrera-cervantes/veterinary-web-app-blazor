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
using Veterinary.Services.AuthServices;
using Xunit;

namespace Veterinary.Tests.Services.AuthServices;

[Collection("AuthService")]
public class AuthServiceTests
{
    #region snippet_Properties

    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    private readonly Mock<ILogger<AuthService>> _mockLogger;

    private readonly Mock<JwtAuthenticationStateProvider> _mockAuthenticationStateProvider;

    #endregion

    #region snippet_Constructors

    public AuthServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLocalStorageService = new Mock<ILocalStorageService>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockAuthenticationStateProvider = new Mock<JwtAuthenticationStateProvider>
        (
            _mockLocalStorageService.Object
        );
    }

    #endregion

    #region snippet_Tests

    [Fact]
    public async Task SignInAsyncShouldThrowException()
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

        var authService = new AuthService
        (
            _mockHttpClientFactory.Object,
            _mockAuthenticationStateProvider.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var credentials = new Credentials
        {
            Email = "dummy@example.com",
            Password = "secret"
        };
        
        await Assert.ThrowsAsync<Exception>(async ()
            => await authService.SignInAsync(credentials));
    }

    [Fact]
    public async Task SignInAsyncShouldReturnHttpMessageResponse()
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
                Content = new StringContent(@"{""message"": ""dummy-jwt""}")
            })
            .Verifiable();

        var authService = new AuthService
        (
            _mockHttpClientFactory.Object,
            _mockAuthenticationStateProvider.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var credentials = new Credentials
        {
            Email = "dummy@example.com",
            Password = "secret"
        };
        var authResponse = await authService.SignInAsync(credentials);

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

        Assert.Equal("dummy-jwt", authResponse.Message);
    }

    [Fact]
    public async Task SignupEmployeeAsyncShouldThrowException()
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

        var authService = new AuthService
        (
            _mockHttpClientFactory.Object,
            _mockAuthenticationStateProvider.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var credentials = new Credentials
        {
            Email = "dummy@example.com",
            Password = "secret"
        };

        await Assert.ThrowsAsync<Exception>(async ()
            => await authService.SignupEmployeeAsync(credentials));
    }

    [Fact]
    public async Task SignupEmployeeAsyncShouldReturnHttpMessageResponse()
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
                Content = new StringContent(@"{""message"": ""success""}")
            })
            .Verifiable();

        var authService = new AuthService
        (
            _mockHttpClientFactory.Object,
            _mockAuthenticationStateProvider.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var credentials = new Credentials
        {
            Email = "dummy@example.com",
            Password = "secret"
        };
        var authResponse = await authService.SignupEmployeeAsync(credentials);

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

        Assert.Equal("success", authResponse.Message);
    }

    [Fact]
    public async Task SignOutAsyncShouldDestroySession()
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
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
            .Verifiable();

        var authService = new AuthService
        (
            _mockHttpClientFactory.Object,
            _mockAuthenticationStateProvider.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        await authService.SignOutAsync();

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
    }

    #endregion
}
