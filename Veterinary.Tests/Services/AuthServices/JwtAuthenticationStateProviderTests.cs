using System.Threading.Tasks;
using Blazored.LocalStorage;
using Moq;
using Veterinary.Services.AuthServices;
using Xunit;

namespace Veterinary.Tests.Services.AuthServices;

[Collection("JwtAuthenticationStateProvider")]
public class JwtAuthenticationStateProviderTests
{
    #region snippet_Properties

    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    #endregion

    #region snippet_Constructors

    public JwtAuthenticationStateProviderTests()
    {
        _mockLocalStorageService = new Mock<ILocalStorageService>();
    }

    #endregion

    #region snippet_Tests

    [Fact]
    public async Task GetAuthenticationStateAsyncShouldReturnStateProvider()
    {
        var jwtAuthenticationStateProvider = new JwtAuthenticationStateProvider
        (
            _mockLocalStorageService.Object
        );
        var authenticationState = await jwtAuthenticationStateProvider.GetAuthenticationStateAsync();

        Assert.NotNull(authenticationState);
    }

    [Fact]
    public void ManageUserState()
    {
        var jwtAuthenticationStateProvider = new JwtAuthenticationStateProvider
        (
            _mockLocalStorageService.Object
        );

        jwtAuthenticationStateProvider.MarkUserAsAuthenticated("dummyid");
        jwtAuthenticationStateProvider.MarkUserAsLoggedOut();
    }

    #endregion
}
