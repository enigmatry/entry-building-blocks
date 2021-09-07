using Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules;
using Enigmatry.BuildingBlocks.Validation.ValidationRules.CustomValidationRules;
using System.Collections.Generic;

namespace Enigmatry.BuildingBlocks.Validation
{
    public interface IHasValidationRules
    {
        IEnumerable<BuiltInValidationRule> BuiltInValidationRules { get; }
        IEnumerable<CustomValidatorValidationRule> ValidatorValidationRules { get; }
        IEnumerable<AsyncCustomValidatorValidationRule> AsyncValidatorValidationRules { get; }
    }
}
