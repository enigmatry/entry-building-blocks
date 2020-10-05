using System;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Options
{
    public class SmtpOptions
    {
        public string Server { get; set; } = String.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public bool UsePickupDirectory { get; set; }
        public string PickupDirectoryLocation { get; set; } = String.Empty;
        public string From { get; set; } = String.Empty;
    }
}
