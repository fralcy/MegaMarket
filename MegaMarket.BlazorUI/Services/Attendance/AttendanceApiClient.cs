using System.Net.Http.Json;
using System.Text.Json;
using MegaMarket.Data.Models;
using MegaMarket.API.DTOs;
using MegaMarket.BlazorUI.Services.Auth;

namespace MegaMarket.BlazorUI.Services.Attendance;

public class AttendanceApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public AttendanceApiClient(HttpClient httpClient, AuthService authService)
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

    // GET: api/Attendance
    public async Task<List<Data.Models.Attendance>?> GetAttendancesAsync()
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<Data.Models.Attendance>>("api/Attendance", _serializerOptions);
        }
        catch
        {
            return null;
        }
    }

    // GET: api/Attendance/{id}
    public async Task<Data.Models.Attendance?> GetAttendanceAsync(int id)
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<Data.Models.Attendance>($"api/Attendance/{id}", _serializerOptions);
        }
        catch
        {
            return null;
        }
    }

    // GET: api/Attendance/user/{userId}
    public async Task<List<Data.Models.Attendance>?> GetAttendancesByUserAsync(int userId)
    {
        try
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<Data.Models.Attendance>>($"api/Attendance/user/{userId}", _serializerOptions);
        }
        catch
        {
            return null;
        }
    }

    // GET: api/Attendance/date/{date}
    public async Task<List<Data.Models.Attendance>?> GetAttendancesByDateAsync(DateTime date)
    {
        try
        {
            await AddAuthorizationHeader();
            var dateStr = date.ToString("yyyy-MM-dd");
            return await _httpClient.GetFromJsonAsync<List<Data.Models.Attendance>>($"api/Attendance/date/{dateStr}", _serializerOptions);
        }
        catch
        {
            return null;
        }
    }

    // POST: api/Attendance
    public async Task<Data.Models.Attendance?> CreateAttendanceAsync(AttendanceInputDto input)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Attendance", input, _serializerOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Data.Models.Attendance>(_serializerOptions);
        }
        catch
        {
            throw;
        }
    }

    // POST: api/Attendance/check-in
    public async Task<Data.Models.Attendance?> CheckInAsync(int userId, DateTime date)
    {
        try
        {
            await AddAuthorizationHeader();
            var dto = new CheckInOutDto { UserId = userId, Date = date };
            var response = await _httpClient.PostAsJsonAsync("api/Attendance/check-in", dto, _serializerOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Data.Models.Attendance>(_serializerOptions);
        }
        catch
        {
            throw;
        }
    }

    // POST: api/Attendance/check-out
    public async Task<Data.Models.Attendance?> CheckOutAsync(int userId, DateTime date)
    {
        try
        {
            await AddAuthorizationHeader();
            var dto = new CheckInOutDto { UserId = userId, Date = date };
            var response = await _httpClient.PostAsJsonAsync("api/Attendance/check-out", dto, _serializerOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Data.Models.Attendance>(_serializerOptions);
        }
        catch
        {
            throw;
        }
    }

    // PUT: api/Attendance/{id}
    public async Task<Data.Models.Attendance?> UpdateAttendanceAsync(int id, AttendanceInputDto input)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/Attendance/{id}", input, _serializerOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Data.Models.Attendance>(_serializerOptions);
        }
        catch
        {
            throw;
        }
    }

    // DELETE: api/Attendance/{id}
    public async Task<bool> DeleteAttendanceAsync(int id)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Attendance/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

// DTO for check-in/check-out operations
public class CheckInOutDto
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
}
