using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MegaMarket.BlazorUI.Services.Auth;

namespace MegaMarket.BlazorUI.Services.GraphQL;

public class GraphQLClient
{
    private readonly HttpClient _httpClient;
    private readonly LocalStorageService _localStorage;
    private readonly string _endpoint;

    public GraphQLClient(IHttpClientFactory httpClientFactory, LocalStorageService localStorage, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("GraphQL");
        _localStorage = localStorage;
        _endpoint = configuration["GraphQL:Endpoint"] ?? "https://localhost:7284/graphql";
        _httpClient.BaseAddress = new Uri(_endpoint);
    }

    public async Task<GraphQLResponse<T>> QueryAsync<T>(string query, object? variables = null)
    {
        return await ExecuteAsync<T>(query, variables);
    }

    public async Task<GraphQLResponse<T>> MutateAsync<T>(string mutation, object? variables = null)
    {
        return await ExecuteAsync<T>(mutation, variables);
    }

    private async Task<GraphQLResponse<T>> ExecuteAsync<T>(string query, object? variables = null)
    {
        var request = new GraphQLRequest
        {
            Query = query,
            Variables = variables
        };

        // Add JWT token to request header
        var token = await _localStorage.GetItemAsync("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.PostAsJsonAsync("", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"GraphQL request failed: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<T>>();

        if (result == null)
        {
            throw new Exception("Failed to deserialize GraphQL response");
        }

        if (result.Errors != null && result.Errors.Any())
        {
            throw new GraphQLException(result.Errors);
        }

        return result;
    }
}

public class GraphQLRequest
{
    [JsonPropertyName("query")]
    public string Query { get; set; } = string.Empty;

    [JsonPropertyName("variables")]
    public object? Variables { get; set; }
}

public class GraphQLResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    public GraphQLError[]? Errors { get; set; }
}

public class GraphQLError
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("locations")]
    public GraphQLLocation[]? Locations { get; set; }

    [JsonPropertyName("path")]
    public string[]? Path { get; set; }
}

public class GraphQLLocation
{
    [JsonPropertyName("line")]
    public int Line { get; set; }

    [JsonPropertyName("column")]
    public int Column { get; set; }
}

public class GraphQLException : Exception
{
    public GraphQLError[] Errors { get; }

    public GraphQLException(GraphQLError[] errors)
        : base($"GraphQL errors: {string.Join(", ", errors.Select(e => e.Message))}")
    {
        Errors = errors;
    }
}
