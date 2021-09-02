using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public interface IValidationRule
    {
        string Name { get; }
        string Message { get; }
        string MessageTranslationId { get; }
        LambdaExpression Expression { get; }
        PropertyInfo PropertyInfo { get; }
        string PropertyName { get; }

        string AsNameRulePair();
        void SetMessage(string message);
        void SetMessageTranslationId(string messageTranslationId);
        void TrySetDefaultMessageTranslationId(string messageTranslationId);
    }
}
