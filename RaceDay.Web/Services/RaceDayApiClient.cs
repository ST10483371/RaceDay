private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
{
    try
    {
        var error = await response.Content.ReadFromJsonAsync<ApiErrorBody>();
        if (!string.IsNullOrWhiteSpace(error?.Message))
            return error.Message;
    }
    catch (JsonException)
    {
        // Body was not JSON; fall through to the status-based message.
    }
    catch (NotSupportedException)
    {
        // Body had an unsupported content type; fall through.
    }

    return response.StatusCode switch
    {
        HttpStatusCode.Unauthorized => "Your session has expired. Please log in again.",
        HttpStatusCode.Forbidden => "Only organizers can do that.",
        _ => $"The API returned {(int)response.StatusCode} {response.ReasonPhrase}."
    };
}
