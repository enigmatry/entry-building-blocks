using System;

namespace Enigmatry.BuildingBlocks.Validation.Helpers
{
    internal static class Check
    {
        public static void IsNumber(Type type, string message = "", params string[] messageArgs)
        {
            if (!Extensions.IsNumber(type))
            {
                throw new InvalidOperationException(String.IsNullOrWhiteSpace(message)
                    ? $"{type.Name} is not of number format. Only number formats supported."
                    : String.Format(message, messageArgs)
                );
            }
        }

        public static void IsEmpty(string value, string message = "", params string[] messageArgs)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(String.IsNullOrWhiteSpace(message)
                    ? "Empty string value is not allowed."
                    : String.Format(message, messageArgs)
                );
            }
        }
    }
}
