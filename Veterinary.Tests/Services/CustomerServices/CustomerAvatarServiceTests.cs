using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;
using Veterinary.Services.CustomerServices;
using Veterinary.Domain.Config;

namespace Veterinary.Tests.Services.CustomerServices;

[Collection(nameof(CustomerAvatarService))]
public class CustomerAvatarServiceTests
{
    #region snippet_Properties

    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    private readonly Mock<ILogger<CustomerAvatarService>> _mockLogger;

    #endregion

    #region snippet_Constructors

    public CustomerAvatarServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLocalStorageService = new Mock<ILocalStorageService>();
        _mockLogger = new Mock<ILogger<CustomerAvatarService>>();
    }

    #endregion

    #region snippet_Tests

    [Fact(DisplayName = "Should return default profile path when the request to the API fails")]
    public async Task GetByIdAsyncShouldReturnDefaultProfilePath()
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

        var customerAvatarService = new CustomerAvatarService(
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var avatar = await customerAvatarService.GetByIdAsync(id: "644e847ff71914f4af18b060");

        Assert.True(avatar.Path == AvatarConfig.NoProfilePicture);
    }

    [Fact(DisplayName = "Should return the correct path when process is successful")]
    public async Task GetByIdAsyncShouldReturnCorrectPath()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();

        var avatarPath = "http://localhost:4566/dummy-s3/myprofile.png";
        var dummyContent = @"{""path"": " + @"""" + avatarPath + @"""}";

        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
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

        var customerAvatarService = new CustomerAvatarService(
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var avatar = await customerAvatarService.GetByIdAsync(id: "644e847ff71914f4af18b060");

        Assert.True(avatar.Path == avatarPath);
    }

    #endregion
}
