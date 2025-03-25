using System.Net;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Http;

[PublicAPI]
public static class HttpResponseMessageAssertionsExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static void BeBadRequest(this HttpResponseMessage response, string because = "", params object[] becauseArgs) =>
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest, string.Format(because, becauseArgs));

    public static void BeNotFound(this HttpResponseMessage response, string because = "", params object[] becauseArgs) =>
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound, string.Format(because, becauseArgs));

    public static void ContainValidationError(this HttpResponseMessage response, string fieldName, string expectedValidationMessage = "", string because = "", params object[] becauseArgs)
    {
        var responseContent = response.Content.ReadAsStringAsync().Result;
        var errorFound = false;
        try
        {
            var json = JsonSerializer.Deserialize<ValidationProblemDetails>(responseContent, Options);

            if (json != null && json.Errors.TryGetValue(fieldName, out var errorsField))
            {
                errorFound = string.IsNullOrEmpty(expectedValidationMessage)
                    ? errorsField.Any()
                    : errorsField.Any(msg =>
                        msg.Contains(expectedValidationMessage, StringComparison.OrdinalIgnoreCase));
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

        if (string.IsNullOrEmpty(expectedValidationMessage))
        {
            errorFound.ShouldBeTrue($"Expected response to have validation message with key: {fieldName}{string.Format(because, becauseArgs)}, but found {responseContent}.");
        }
        else
        {
            errorFound.ShouldBeTrue($"Expected response to have validation message with key: {fieldName} and message: {expectedValidationMessage} {string.Format(because, becauseArgs)}, but found {responseContent}.");
        }
    }
}
