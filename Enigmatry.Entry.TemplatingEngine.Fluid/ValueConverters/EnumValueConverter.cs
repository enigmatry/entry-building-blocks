using Enigmatry.Entry.Core.Helpers;

namespace Enigmatry.Entry.TemplatingEngine.Liquid.ValueConverters;

public class EnumValueConverter : IFluidValueConverter
{
    public object? Convert(object? value) => value is not Enum @enum ? null : @enum.GetDescription();
}
