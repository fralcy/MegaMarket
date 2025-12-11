using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MegaMarket.BlazorUI.Services.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly LocalStorageService _localStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthStateProvider(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync("authToken");

            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(_anonymous);

            // Check if token is expired
            var expiresAtStr = await _localStorage.GetItemAsync("expiresAt");
            if (!string.IsNullOrEmpty(expiresAtStr) && DateTime.TryParse(expiresAtStr, out var expiresAt))
            {
                if (DateTime.UtcNow >= expiresAt)
                {
                    await ClearStorage();
                    return new AuthenticationState(_anonymous);
                }
            }

            // Get user info from localStorage
            var userId = await _localStorage.GetItemAsync("userId");
            var username = await _localStorage.GetItemAsync("username");
            var fullName = await _localStorage.GetItemAsync("fullName");
            var role = await _localStorage.GetItemAsync("role");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId ?? ""),
                new Claim(ClaimTypes.Name, username ?? ""),
                new Claim("FullName", fullName ?? ""),
                new Claim(ClaimTypes.Role, role ?? "")
            };

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        var authState = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(new AuthenticationState(_anonymous));
        NotifyAuthenticationStateChanged(authState);
    }

    private async Task ClearStorage()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userId");
        await _localStorage.RemoveItemAsync("username");
        await _localStorage.RemoveItemAsync("fullName");
        await _localStorage.RemoveItemAsync("role");
        await _localStorage.RemoveItemAsync("expiresAt");
    }
}
