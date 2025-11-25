using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage.Models;

[PublicAPI]
public enum ContentDispositionType
{
    [Display(Name = "attachment")]
    Attachment = 0,
    [Display(Name = "inline")]
    Inline = 1
}
