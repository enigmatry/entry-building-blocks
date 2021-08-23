using System;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public abstract class ValidationRule
    {
        public string Name { get; } = String.Empty;
        public string PropertyName { get; } = String.Empty;
        public string Message { get; } = String.Empty;
        public string MessageTranslationId { get; set; } = String.Empty;

        protected ValidationRule(string name, PropertyInfo propertyInfo, string message, string messageTranslationId)
        {
            Name = name;
            PropertyName = propertyInfo.Name.ToLowerInvariant();
            Message = message;
            MessageTranslationId = messageTranslationId;
        }
    }
}
