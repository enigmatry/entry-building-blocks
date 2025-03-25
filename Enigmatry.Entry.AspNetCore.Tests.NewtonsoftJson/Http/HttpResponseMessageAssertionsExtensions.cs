using System.Net;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shouldly;

namespace Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Http;

[PublicAPI]
public static class HttpResponseMessageAssertionsExtensions
{
    public static HttpResponseMessage BeBadRequest(this HttpResponseMessage response, string because = "", params object[] becauseArgs)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest, string.Format(because, becauseArgs));
        return response;
    }

    public static HttpResponseMessage BeNotFound(this HttpResponseMessage response, string because = "", params object[] becauseArgs)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound, string.Format(because, becauseArgs));
        return response;
    }

    public static HttpResponseMessage ContainValidationError(this HttpResponseMessage response, string fieldName, string expectedValidationMessage = "", string because = "", params object[] becauseArgs)
    {
        var responseContent = response.Content.ReadAsStringAsync().Result;
        var errorFound = false;
        try
        {
            var json = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent, HttpSerializationSettings.Settings);

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

        return response;
    }
}
