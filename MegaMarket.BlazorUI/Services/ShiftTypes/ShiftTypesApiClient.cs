using System.Net.Http.Json;
using System.Text.Json;
using MegaMarket.Data.Models;
using MegaMarket.API.DTOs;
using MegaMarket.BlazorUI.Services.Auth;

namespace MegaMarket.BlazorUI.Services.ShiftTypes;

public class ShiftTypesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public ShiftTypesApiClient(HttpClient httpClient, AuthService authService)
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

    // GET: api/ShiftTypes
    public async Task<List<ShiftType>?> GetShiftTypesAsync()
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<ShiftType>>("api/ShiftTypes", _serializerOptions);
        }
        catch
        {
            return null;
        }
    }

    // GET: api/ShiftTypes/{id}
    public async Task<ShiftType?> GetShiftTypeAsync(int id)
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<ShiftType>($"api/ShiftTypes/{id}", _serializerOptions);
        }
        catch
        {
            return null;
        }
    }

    // GET: api/ShiftTypes/name/{name}
    public async Task<ShiftType?> GetShiftTypeByNameAsync(string name)
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<ShiftType>($"api/ShiftTypes/name/{name}", _serializerOptions);
        }
        catch
        {
            return null;
        }
    }

    // POST: api/ShiftTypes
    public async Task<ShiftType?> CreateShiftTypeAsync(ShiftTypeInputDto input)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/ShiftTypes", input, _serializerOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShiftType>(_serializerOptions);
        }
        catch
        {
            throw;
        }
    }

    // PUT: api/ShiftTypes/{id}
    public async Task<ShiftType?> UpdateShiftTypeAsync(int id, ShiftTypeInputDto input)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/ShiftTypes/{id}", input, _serializerOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShiftType>(_serializerOptions);
        }
        catch
        {
            throw;
        }
    }

    // DELETE: api/ShiftTypes/{id}
    public async Task<bool> DeleteShiftTypeAsync(int id)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/ShiftTypes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
