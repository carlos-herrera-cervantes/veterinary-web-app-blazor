using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Moq;
using Moq.Protected;
using Veterinary.Services.AuthServices;
using Xunit;

namespace Veterinary.Tests.Services.AuthServices;

[Collection("AvatarService")]
public class AvatarServiceTests
{
    #region snippet_Properties

    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    #endregion

    #region snippet_Constructors

    public AvatarServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLocalStorageService = new Mock<ILocalStorageService>();
    }

    #endregion

    #region snippet_Tests

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

        var avatarService = new AvatarService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object
        );
        var avatar = await avatarService.GetAsync();

        Assert.Null(avatar);
    }

    [Fact]
    public async Task GetAsyncShouldReturnAvatar()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        var avatarPath = "http://localhost:4566/dummy-s3/myprofile.png";
        var dummyContent = @"{""path"": " + @"""" + avatarPath + @"""}";

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

        var avatarService = new AvatarService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object
        );
        var avatar = await avatarService.GetAsync();

        Assert.Equal(avatarPath, avatar.Path);
    }

    [Fact]
    public async Task UploadShouldThrowException()
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

        var avatarService = new AvatarService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object
        );

        await Assert.ThrowsAsync<Exception>(async () => await avatarService.Upload(null));
    }

    [Fact]
    public async Task UploadShouldReturnAvatar()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        var avatarPath = "http://localhost:4566/dummy-s3/myprofile.png";
        var dummyContent = @"{""path"": " + @"""" + avatarPath + @"""}";

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

        var avatarService = new AvatarService
        (
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object
        );
        var avatar = await avatarService.Upload(null);

        Assert.Equal(avatarPath, avatar.Path);
    }

    #endregion
}
