using System;

namespace Enigmatry.BuildingBlocks.Tests.Validation
{
    internal class ValidationMockModel
    {
        public int IntField { get; set; }
        public int? NullableIntField { get; set; }
        public double DoubleField { get; set; }
        public double? NullableDoubleField { get; set; }
        public byte ByteField { get; set; }
        public byte? NullableByteField { get; set; }
        public DateTimeOffset DateTimeOffsetField { get; set; }
        public string StringField { get; set; } = String.Empty;
        public string OtherStringField { get; set; } = String.Empty;
    }
}
