using System;

namespace Enigmatry.Entry.Core.Settings
{
    public class SmtpSettings
    {
        public const string AppSmtp = "App:Smtp";

        public string Server { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UsePickupDirectory { get; set; }
        public string PickupDirectoryLocation { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string CatchAllAddress { get; set; } = string.Empty;
    }
}
