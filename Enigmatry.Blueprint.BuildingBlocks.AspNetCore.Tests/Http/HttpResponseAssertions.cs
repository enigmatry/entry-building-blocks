using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Enigmatry.Blueprint.BuildingBlocks.AspNetCore.Tests.Http
{
    public class HttpResponseAssertions : ReferenceTypeAssertions<HttpResponseMessage, HttpResponseAssertions>
    {
        protected override string Identifier => "HttpResponse";

        public HttpResponseAssertions(HttpResponseMessage value)
        {
            Subject = value;
        }

        public AndConstraint<HttpResponseAssertions> BeBadRequest(string because = "", params object[] becauseArgs) =>
            HaveStatusCode(HttpStatusCode.BadRequest, because, becauseArgs);

        public AndConstraint<HttpResponseAssertions> BeNotFound(string because = "", params object[] becauseArgs) =>
            HaveStatusCode(HttpStatusCode.NotFound, because, becauseArgs);

        private AndConstraint<HttpResponseAssertions> HaveStatusCode(HttpStatusCode expected, string because = "",
            params object[] becauseArgs)
        {
            AssertionScope assertion = Execute.Assertion;
            AssertionScope assertionScope =
                assertion.ForCondition(Subject.StatusCode == expected).BecauseOf(because, becauseArgs);
            const string message = "Expected response to have HttpStatusCode {0}{reason}, but found {1}. Response: {2}";
            object[] failArgs = {expected, Subject.StatusCode, Subject.Content.ReadAsStringAsync().Result};
            _ = assertionScope.FailWith(message, failArgs);
            return new AndConstraint<HttpResponseAssertions>(this);
        }

        public AndConstraint<HttpResponseAssertions> ContainValidationError(string fieldName,
            string expectedValidationMessage = "", string because = "", params object[] becauseArgs)
        {
            var responseContent = Subject.Content.ReadAsStringAsync().Result;
            var errorFound = false;
            try
            {
                var json = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);

                if (json.Errors.TryGetValue(fieldName, out var errorsField))
                {
                    errorFound = String.IsNullOrEmpty(expectedValidationMessage)
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
            if (String.IsNullOrEmpty(expectedValidationMessage))
            {
                message = "Expected response to have validation message with key: {0}{reason}, but found {1}.";
                failArgs = new object[] {fieldName, responseContent};
            }
            else
            {
                message =
                    "Expected response to have validation message with key: {0} and message: {1} {reason}, but found {2}.";
                failArgs = new object[] {fieldName, expectedValidationMessage, responseContent};
            }

            _ = assertionScope.FailWith(message, failArgs);
            return new AndConstraint<HttpResponseAssertions>(this);
        }
    }
}
