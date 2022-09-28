using System;

namespace Enigmatry.Entry.Validation.Helpers
{
    internal static class Check
    {
        public static void IfNotNumber(Type type, string message = "", params string[] messageArgs)
        {
            if (type.NotNumber())
            {
                throw new InvalidOperationException(String.IsNullOrWhiteSpace(message)
                    ? $"{type.Name} is not of number format. Only number formats supported."
                    : String.Format(message, messageArgs)
                );
            }
        }

        public static void IfEmpty(string value, string message = "", params string[] messageArgs)
        {
            if (value.IsEmpty())
            {
                throw new InvalidOperationException(String.IsNullOrWhiteSpace(message)
                    ? "Empty string value is not allowed."
                    : String.Format(message, messageArgs)
                );
            }
        }
    }
}
