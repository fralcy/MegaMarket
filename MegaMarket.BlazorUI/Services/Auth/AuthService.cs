using MegaMarket.BlazorUI.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MegaMarket.BlazorUI.Services.Auth;

public class AuthService
{
    private readonly LocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;
    private const string TokenKey = "authToken";

    public AuthService(
        LocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> LoginAsync(string token, string userId, string username, string fullName, string role, DateTime expiresAt)
    {
        try
        {
            // Store token info in localStorage
            await _localStorage.SetItemAsync(TokenKey, token);
            await _localStorage.SetItemAsync("userId", userId);
            await _localStorage.SetItemAsync("username", username);
            await _localStorage.SetItemAsync("fullName", fullName);
            await _localStorage.SetItemAsync("role", role);
            await _localStorage.SetItemAsync("expiresAt", expiresAt.ToString("O"));

            // Notify authentication state changed
            await ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthenticationAsync(token);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync("userId");
        await _localStorage.RemoveItemAsync("username");
        await _localStorage.RemoveItemAsync("fullName");
        await _localStorage.RemoveItemAsync("role");
        await _localStorage.RemoveItemAsync("expiresAt");

        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync(TokenKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return false;

        var expiresAtStr = await _localStorage.GetItemAsync("expiresAt");
        if (string.IsNullOrEmpty(expiresAtStr))
            return false;

        if (DateTime.TryParse(expiresAtStr, out var expiresAt))
        {
            return DateTime.UtcNow < expiresAt;
        }

        return false;
    }

    public async Task<ClaimsPrincipal> GetCurrentUserAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return new ClaimsPrincipal(new ClaimsIdentity());

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
        return new ClaimsPrincipal(identity);
    }

    public async Task<string?> GetUserRoleAsync()
    {
        return await _localStorage.GetItemAsync("role");
    }
}
