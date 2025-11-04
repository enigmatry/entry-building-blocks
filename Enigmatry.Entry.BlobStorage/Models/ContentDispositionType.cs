using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage.Models;

[PublicAPI]
public enum ContentDispositionType
{
    Attachment = 0,
    Inline = 1
}
