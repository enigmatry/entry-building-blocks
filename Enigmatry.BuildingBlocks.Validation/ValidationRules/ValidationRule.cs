using Humanizer;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public abstract class ValidationRule : IValidationRule
    {
        public string Name { get; private set; } = String.Empty;
        public string Message { get; private set; } = String.Empty;
        public string MessageTranslationId { get; private set; } = String.Empty;
        public LambdaExpression Expression { get; }
        public PropertyInfo PropertyInfo { get; }
        public string PropertyName => PropertyInfo.Name.Camelize();

        protected ValidationRule(string name, LambdaExpression expression, PropertyInfo propertyInfo)
        {
            Name = name;
            Expression = expression;
            PropertyInfo = propertyInfo;
        }

        public void SetMessage(string message) => Message = message;

        public void SetMessageTranslationId(string messageTranslationId) => MessageTranslationId = messageTranslationId;

        public void TrySetDefaultMessageTranslationId(string messageTranslationId)
        {
            if (String.IsNullOrWhiteSpace(MessageTranslationId))
            {
                MessageTranslationId = messageTranslationId;
            }
        }
    }
}
