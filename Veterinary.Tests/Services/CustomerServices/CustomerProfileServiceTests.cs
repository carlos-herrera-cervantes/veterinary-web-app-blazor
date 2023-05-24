using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Xunit;
using Newtonsoft.Json;
using Moq;
using Moq.Protected;
using Veterinary.Domain.Models;
using Veterinary.Services.CustomerServices;
using Veterinary.Domain.Types;

namespace Veterinary.Tests.Services.CustomerServices;

[Collection(nameof(CustomerProfileService))]
public class CustomerProfileServiceTests
{
    #region snippet_Properties

    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    private readonly Mock<ILogger<CustomerProfileService>> _mockLogger;

    #endregion

    #region snippet_Constructors

    public CustomerProfileServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLocalStorageService = new Mock<ILocalStorageService>();
        _mockLogger = new Mock<ILogger<CustomerProfileService>>();
    }

    #endregion

    #region snippet_Tests

    [Fact(DisplayName = "Should return en empty response when request fails")]
    public async Task GetAllAsyncShouldReturnEmptyResponse()
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

        var customerProfileService = new CustomerProfileService(
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        HttpListResponse<CustomerProfile> profiles = await customerProfileService.GetAllAsync(offset: 0, limit: 10);

        Assert.Null(profiles.Data);
    }

    [Fact(DisplayName = "Should return data when request is successful")]
    public async Task GetAllAsyncShouldReturnData()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();

        var dummyProfileList = new List<CustomerProfile>
        {
            new CustomerProfile
            {
                CustomerId = "645ee8e20813510c2a14d7f7",
                Name = "User",
                LastName = "Example"
            }
        };
        var dummyContent = JsonConvert.SerializeObject(new HttpListResponse<CustomerProfile>
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

        var customerProfileService = new CustomerProfileService(
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        HttpListResponse<CustomerProfile> profiles = await customerProfileService.GetAllAsync(offset: 0, limit: 10);

        Assert.NotEmpty(profiles.Data);
    }

    [Fact(DisplayName = "Should return an empty profile when request fails")]
    public async Task GetByIdAsyncShouldReturnEmptyResponse()
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

        var customerProfileService = new CustomerProfileService(
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        CustomerProfile profile = await customerProfileService.GetByIdAsync(id: "6460e990f0483254f96089fa");

        Assert.Null(profile.Name);
    }

    [Fact(DisplayName = "Should return customer profile when request is successful")]
    public async Task GetByIdAsyncShouldReturnProfile()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:9001");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();

        var dummyProfile = new CustomerProfile
        {
            CustomerId = "645ee8e20813510c2a14d7f7",
            Name = "User",
            LastName = "Example"
        };
        var dummyContent = JsonConvert.SerializeObject(dummyProfile);

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

        var customerProfileService = new CustomerProfileService(
            _mockHttpClientFactory.Object,
            _mockLocalStorageService.Object,
            _mockLogger.Object
        );
        CustomerProfile profile = await customerProfileService.GetByIdAsync(id: "645ee8e20813510c2a14d7f7");

        Assert.NotNull(profile.Name);
    }

    #endregion
}
