# Graph API Building Block

Building Block for using the Microsoft AD Graph API

## Registration

You can use the `AppAddGraphApi` extension method on `IServiceCollection` to register a scoped inastance of `GraphServiceClient`:

```cs
 public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AppAddGraphApi(_configuration);
    }
```

## Configuration

Example:

```json
  "App": {
    
    "GraphApi": {
      "Enabled": true,
      "TenantId": "AZURE_AD_TENANT_ID",
      "ClientId": "APP_REGISTRATION_CLIENT_ID",
      "ClientSecret": "APP_REGISTRATION_CLIENT_SECRET",
      "PasswordPolicies": "OPTIONAL_LIST_PASSWORD_POLICIES"
    },
```

- Only TenantId, ClientId and ClientSecret are used to construct the `GraphServiceClient`
- `Enabled` can be used how you see fit, it is not used by the building block.
- `PasswordPolicies` can be used to supply a list of Password policies. Used by some of the GraphServiceClient extensions.

## GraphServiceClient Extensions

The Building Block also contains a number of extension methods for `GraphServiceClient`:

- GetUserById
- GetUserByIssuerAssignedId
- GetUsers
- GetUserPhoto
- AddUser
- UpdateUser
- UpdateUserPassword
- UpdateUserSignInEmailAddress