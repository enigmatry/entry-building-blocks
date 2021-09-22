using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public interface IBaseValidationRule
    {
        string CustomMessage { get; }
        string MessageTranslationId { get; }
        bool HasCustomMessage { get; }
        bool HasMessageTranslationId { get; }
        PropertyInfo PropertyInfo { get; }
        string PropertyName { get; }
    }

    public interface IFormlyValidationRule : IBaseValidationRule
    {
        string FormlyValidationMessage { get; }
        string FormlyRuleName { get; }
        string[] FormlyTemplateOptions { get; }

        void SetMessageTranslationId(string messageTranslationId);
    }

    public interface IFluentValidationValidationRule : IBaseValidationRule
    {
        LambdaExpression Expression { get; }
    }

    public interface IValidationRule : IFormlyValidationRule, IFluentValidationValidationRule
    {
        void SetCustomMessage(string message);
    }
}
