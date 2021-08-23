using Enigmatry.BuildingBlocks.Validation.ValidationRules;
using System.Collections.Generic;

namespace Enigmatry.BuildingBlocks.Validation
{
    public interface IHasValidationRules
    {
        public IEnumerable<ValidationRule> GetValidationRules();
    }
}
