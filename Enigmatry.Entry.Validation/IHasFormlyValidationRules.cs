using Enigmatry.Entry.Validation.ValidationRules;
using System.Collections.Generic;

namespace Enigmatry.Entry.Validation
{
    public interface IHasFormlyValidationRules
    {
        IEnumerable<IFormlyValidationRule> ValidationRules { get; }
    }
}
