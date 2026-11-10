using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Services;

public record ApiResult<T>(bool Success, T? Data, string? Error);

// Wraps every call to the RaceDay API.
public class RaceDayApiClient
{
    private readonly HttpClient _http;

    public RaceDayApiClient(HttpClient http)
    {
        _http = http;
    }

    // ---- Auth ----

    public Task<ApiResult<LoginResponse>> LoginAsync(string email, string password) =>
        SendAsync<LoginResponse>(HttpMethod.Post, "api/Auth/login", null,
            new { Email = email, Password = password });

    public Task<ApiResult<object>> RegisterAsync(RegisterRequest request) =>
        SendAsync<object>(HttpMethod.Post, "api/Auth/register", null, new
        {
            request.Name,
            request.Email,
            request.Password,
            Contact_Info = request.ContactInfo
        });

    // ---- Races ----

    public Task<ApiResult<List<Race>>> GetRacesAsync() =>
        SendAsync<List<Race>>(HttpMethod.Get, "api/Races", null);

    public Task<ApiResult<Race>> GetRaceAsync(int id) =>
        SendAsync<Race>(HttpMethod.Get, $"api/Races/{id}", null);

    public Task<ApiResult<Race>> CreateRaceAsync(Race race, string token) =>
        SendAsync<Race>(HttpMethod.Post, "api/Races", token, race);

    public Task<ApiResult<object>> UpdateRaceAsync(Race race, string token) =>
        SendAsync<object>(HttpMethod.Put, $"api/Races/{race.RaceID}", token, race);

    public Task<ApiResult<object>> DeleteRaceAsync(int id, string token) =>
        SendAsync<object>(HttpMethod.Delete, $"api/Races/{id}", token);

    // ---- Plumbing ----

    private async Task<ApiResult<T>> SendAsync<T>(
        HttpMethod method, string url, string? token, object? body = null)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url);

            if (token is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (body is not null)
                request.Content = JsonContent.Create(body);

            using var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new ApiResult<T>(false, default, await ReadErrorAsync(response));

            if (response.StatusCode == HttpStatusCode.NoContent)
                return new ApiResult<T>(true, default, null);

            var data = await response.Content.ReadFromJsonAsync<T>();
            return new ApiResult<T>(true, data, null);
        }
        catch (HttpRequestException)
        {
            return new ApiResult<T>(false, default,
                "Could not reach the RaceDay API. Check that it is running.");
        }
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return "Your session has expired. Please log in again.";

        if (response.StatusCode == HttpStatusCode.Forbidden)
            return "Only organizers can do that.";

        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorBody>();
            if (!string.IsNullOrWhiteSpace(error?.Message))
                return error.Message;
        }
        catch (JsonException)
        {
            // Body was not JSON; fall through to the generic message.
        }
        catch (NotSupportedException)
        {
            // Body had an unsupported content type; fall through.
        }

        return $"The API returned {(int)response.StatusCode} {response.ReasonPhrase}.";
    }
}
