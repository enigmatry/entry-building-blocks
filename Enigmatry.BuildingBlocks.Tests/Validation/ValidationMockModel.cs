using System;

namespace Enigmatry.BuildingBlocks.Tests.Validation
{
    internal class ValidationMockModel
    {
        public int IntField { get; set; }
        public double DoubleField { get; set; }
        public DateTimeOffset DateTimeOffsetField { get; set; }
        public string StringField { get; set; } = String.Empty;
        public string OtherStringField { get; set; } = String.Empty;
    }
}
