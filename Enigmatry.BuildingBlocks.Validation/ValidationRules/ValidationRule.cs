using Humanizer;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public abstract class ValidationRule<TRule> : IValidationRule
    {
        public TRule Rule { get; private set; }
        public string CustomMessage { get; private set; } = String.Empty;
        public string MessageTranslationId { get; private set; } = String.Empty;
        public bool HasCustomMessage => !String.IsNullOrWhiteSpace(CustomMessage);
        public bool HasMessageTranslationId => !String.IsNullOrWhiteSpace(MessageTranslationId);
        public LambdaExpression Expression { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public string PropertyName => PropertyInfo.Name.Camelize();
        public abstract string FormlyValidationMessage { get; }
        public abstract string FormlyRuleName { get; }
        public virtual string[] FormlyTemplateOptions => new[] { $"{FormlyRuleName}: {Rule}" };


        protected ValidationRule(
            TRule rule,
            PropertyInfo propertyInfo,
            LambdaExpression expression,
            string message,
            string messageTranslationId)
        {
            Rule = rule;
            PropertyInfo = propertyInfo;
            Expression = expression;
            SetCustomMessage(message);
            SetMessageTranslationId(messageTranslationId);
        }

        public void SetCustomMessage(string message)
        {
            MessageTranslationId = String.Empty;
            CustomMessage = message;
        }

        public void SetMessageTranslationId(string messageTranslationId) =>
            MessageTranslationId = messageTranslationId;
    }
}
