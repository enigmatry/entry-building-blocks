using Microsoft.Graph.Models;
using System;

namespace Enigmatry.Entry.GraphApi.Models;

/// <summary>
/// Changeable user details. All nullable properties that are unset or set to null explicitly, won't be changed.
/// </summary>
/// <param name="userId">User id.</param>
/// <param name="passwordProfile">Password profile for the user. The profile contains the user's password.</param>
public class UserDetails(string userId, PasswordProfile passwordProfile)
{
    public string UserId { get; } = userId ?? throw new ArgumentNullException(nameof(userId));
    public PasswordProfile PasswordProfile { get; } = passwordProfile ?? throw new ArgumentNullException(nameof(passwordProfile));

    /// <summary>
    /// Display name of the user.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Identities that can be used to sign in to this user account.
    /// </summary>
    public ObjectIdentity? Identity { get; set; }

    /// <summary>
    /// Specifies password policies for the user, for example: DisablePasswordExpiration, DisableStrongPassword.
    /// </summary>
    public string? PasswordPolicies { get; set; }
}
