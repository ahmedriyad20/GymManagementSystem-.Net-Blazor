using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace GymManagementSystem.BlazorWASM.Services;

public class JwtAuthenticationStateProvider(HttpClient httpClient, IJSRuntime jsRuntime) : AuthenticationStateProvider
{
    private const string AccessTokenKey = "gym.accessToken";
    private const string RefreshTokenKey = "gym.refreshToken";
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(Anonymous);
        }

        try
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            await MarkUserAsLoggedOutAsync();
            return new AuthenticationState(Anonymous);
        }
    }

    public async Task MarkUserAsAuthenticatedAsync(string accessToken, string? refreshToken)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, accessToken);
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, refreshToken);
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(accessToken), "jwt");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
        httpClient.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
    }

    public async Task<string?> GetAccessTokenAsync() => await jsRuntime.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}
