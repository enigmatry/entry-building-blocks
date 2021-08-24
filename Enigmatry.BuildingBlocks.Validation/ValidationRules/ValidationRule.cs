using Humanizer;
using System;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public abstract class ValidationRule
    {
        public string Name { get; } = String.Empty;
        public object Value { get; }
        public string PropertyName { get; } = String.Empty;
        public string Message { get; } = String.Empty;
        public string MessageTranslationId { get; private set; } = String.Empty;

        protected ValidationRule(string name, object value, PropertyInfo propertyInfo, string message, string messageTranslationId)
        {
            Name = name;
            Value = value;
            PropertyName = propertyInfo.Name.Camelize();
            Message = message;
            MessageTranslationId = messageTranslationId;
        }

        public abstract string AsNameValueString();

        public void TrySetMessageTranslationId(string messageTranslationId)
        {
            if (String.IsNullOrWhiteSpace(MessageTranslationId))
            {
                MessageTranslationId = messageTranslationId;
            }
        }
    }
}
