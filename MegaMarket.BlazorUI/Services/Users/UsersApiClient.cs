using System.Net.Http.Json;
using System.Text.Json;
using MegaMarket.Data.Models;
using MegaMarket.API.DTOs;
using MegaMarket.BlazorUI.Services.Auth;

namespace MegaMarket.BlazorUI.Services.Users;

public class UsersApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public UsersApiClient(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private async Task AddAuthorizationHeader()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    // GET: api/Users
    public async Task<List<User>?> GetUsersAsync()
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<User>>("api/Users", _serializerOptions);
        }
        catch
        {
            return null;
        }
    }

    // GET: api/Users/{id}
    public async Task<User?> GetUserAsync(int id)
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<User>($"api/Users/{id}", _serializerOptions);
        }
        catch
        {
            return null;
        }
    }

    // GET: api/Users/username/{username}
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<User>($"api/Users/username/{username}", _serializerOptions);
        }
        catch
        {
            return null;
        }
    }

    // POST: api/Users
    public async Task<User?> CreateUserAsync(UserInputDto input)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Users", input, _serializerOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<User>(_serializerOptions);
        }
        catch
        {
            throw;
        }
    }

    // PUT: api/Users/{id}
    public async Task<User?> UpdateUserAsync(int id, UserInputDto input)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/Users/{id}", input, _serializerOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<User>(_serializerOptions);
        }
        catch
        {
            throw;
        }
    }

    // DELETE: api/Users/{id}
    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Users/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
