using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CasaDaRosa.API.E2ETests.Contracts;

namespace CasaDaRosa.API.E2ETests.Infrastructure;

internal static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task<ApiResponse<T>?> ReadSuccessAsync<T>(this HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<ApiResponse<T>>(SerializerOptions);
    }

    public static async Task<ApiErrorResponse?> ReadErrorAsync(this HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<ApiErrorResponse>(SerializerOptions);
    }
}
