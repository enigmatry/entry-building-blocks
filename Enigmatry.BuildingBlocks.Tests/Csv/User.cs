using System;
using Enigmatry.BuildingBlocks.Csv;

namespace Enigmatry.BuildingBlocks.Tests.Csv
{
    public class User
    {
        public string FirstName { get; set; } = null!;
        [Ignore]
        public string IgnoreMe { get; set; } = "Ignore me please!";
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        [Name("Ingelogd op")]
        public DateTimeOffset LastLogon { get; set; }
        public DateTime SomeDateTime { get; set; }
    }
}
