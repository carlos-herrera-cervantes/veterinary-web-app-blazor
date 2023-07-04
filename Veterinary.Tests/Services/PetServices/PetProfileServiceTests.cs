using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Blazored.LocalStorage;
using Xunit;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Veterinary.Services.PetServices;
using Veterinary.Domain.Models;
using Veterinary.Domain.Types;

namespace Veterinary.Tests.Services.PetServices;

[Collection(nameof(PetProfileService))]
public class PetProfileServiceTests
{
    #region snippet_Properties

    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    private readonly Mock<ILogger<PetProfileService>> _mockLogger;

    #endregion

    #region snippet_Constructors

    public PetProfileServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLocalStorageService = new Mock<ILocalStorageService>();
        _mockLogger = new Mock<ILogger<PetProfileService>>();
    }

    #endregion

    #region snippet_Tests

    [Fact(DisplayName = "Should return empty data when request fails")]
    public async Task GetByCustomerIdAsyncShouldReturnEmptyData()
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

        var petProfileService = new PetProfileService(
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var httpListResponse = await petProfileService.GetByCustomerIdAsync("dummy-id");

        Assert.Empty(httpListResponse.Data);
    }

    [Fact(DisplayName = "Should return data when request succeeds")]
    public async Task GetByCustomerIdAsyncShouldReturnData()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();

        var dummyProfileList = new List<PetProfile>
        {
            new PetProfile
            {
                CustomerId = "645ee8e20813510c2a14d7f7",
                Name = "Antionio"
            }
        };
        var dummyContent = JsonConvert.SerializeObject(new HttpListResponse<PetProfile>
        {
            Data = dummyProfileList
        });

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

        var petProfileService = new PetProfileService(
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var httpListResponse = await petProfileService.GetByCustomerIdAsync("dummy-id");

        Assert.NotEmpty(httpListResponse.Data);
    }

    [Fact(DisplayName = "Should return empty profile when request fails")]
    public async Task GetByIdAsyncShouldReturnEmptyProfile()
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

        var petProfileService = new PetProfileService(
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var profile = await petProfileService.GetByIdAsync("dummy-id");

        Assert.Null(profile.Name);
    }

    [Fact(DisplayName = "Should return profile when request succeeds")]
    public async Task GetByIdAsyncShouldReturnProfile()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();

        var dummyContent = JsonConvert.SerializeObject(new PetProfile
        {
            CustomerId = "645ee8e20813510c2a14d7f7",
            Name = "Antionio"
        });

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

        var petProfileService = new PetProfileService(
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        var profile = await petProfileService.GetByIdAsync("dummy-id");

        Assert.NotEmpty(profile.Name);
    }

    #endregion
}
