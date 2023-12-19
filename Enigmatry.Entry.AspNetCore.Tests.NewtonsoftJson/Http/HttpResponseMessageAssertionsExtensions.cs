using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Http;

[PublicAPI]
public static class HttpResponseMessageAssertionsExtensions
{
    public static AndConstraint<HttpResponseMessageAssertions> BeBadRequest(this HttpResponseMessageAssertions constraints, string because = "", params object[] becauseArgs) =>
        constraints.HaveStatusCode(HttpStatusCode.BadRequest, because, becauseArgs);

    public static AndConstraint<HttpResponseMessageAssertions> BeNotFound(this HttpResponseMessageAssertions constraints, string because = "", params object[] becauseArgs) =>
        constraints.HaveStatusCode(HttpStatusCode.NotFound, because, becauseArgs);

    public static AndConstraint<HttpResponseMessageAssertions> ContainValidationError(this HttpResponseMessageAssertions constraints
        , string fieldName,
        string expectedValidationMessage = "", string because = "", params object[] becauseArgs)
    {
        var responseContent = constraints.Subject.Content.ReadAsStringAsync().Result;
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

        AssertionScope assertion = Execute.Assertion;
        AssertionScope assertionScope = assertion.ForCondition(errorFound).BecauseOf(because, becauseArgs);
        string message;
        object[] failArgs;
        if (string.IsNullOrEmpty(expectedValidationMessage))
        {
            message = "Expected response to have validation message with key: {0}{reason}, but found {1}.";
            failArgs = new object[] { fieldName, responseContent };
        }
        else
        {
            message =
                "Expected response to have validation message with key: {0} and message: {1} {reason}, but found {2}.";
            failArgs = new object[] { fieldName, expectedValidationMessage, responseContent };
        }

        _ = assertionScope.FailWith(message, failArgs);
        return new AndConstraint<HttpResponseMessageAssertions>(constraints);
    }
}
