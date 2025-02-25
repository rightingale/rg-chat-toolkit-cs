using rg_chat_toolkit_api_cs.Chat;
using rg_chat_toolkit_api_cs.Data;
using rg_chat_toolkit_api_cs.Data.Models;
using rg_chat_toolkit_cs.Chat;
using System.Security.Claims;
using System.Security.Principal;

namespace rg_chat_toolkit_api_cs;

public static class AuthenticationHelper
{
    public const string MESSAGE_TENANT_ID_MISSING = "Authentication: Missing TenantID header.";
    public const string MESSAGE_TENANT_ID_MISSING_REQUEST = "Authentication: Missing TenantID parameter.";
    public const string MESSAGE_TENANT_ID_INVALID = "Authentication: Invalid TenantID header.";
    public const string MESSAGE_TENANT_ID_INVALID_REQUEST = "Authentication: Invalid TenantID parameter.";
    public const string MESSAGE_TENANT_ID_UNKNOWN = "Authentication: Unknown TenantID.";
    public const string MESSAGE_TENANT_ID_UNKNOWN_HEADER = "Authentication: Unknown TenantID header.";

    public const string MESSAGE_TOKEN_INVALID = "Authentication: Security token invalid or expired.";

    public const string HEADER_KEY_TENANT_ID = "rg-TenantId";

    public static void Authorize(IIdentity? identity, RequestBase request)
    {
        // Verify that the TenantID in the HTTP Header value matches the TenantID in the request
        bool isAuthorizedTenant = false;
        var httpContext = new HttpContextAccessor().HttpContext;
        var httpHeaderTenantID = httpContext?.Request?.Headers[HEADER_KEY_TENANT_ID].ToString();
        var requestTenantID = request.TenantID.ToString();

        var isHttpHeaderTenantIDGuidParsed = Guid.TryParse(httpHeaderTenantID, out var httpHeaderTenantIDGuid);
        var isRequestTenantIDGuidParsed = Guid.TryParse(requestTenantID, out var requestTenantIDGuid);

        if (!isHttpHeaderTenantIDGuidParsed)
        {
            throw new UnauthorizedAccessException(MESSAGE_TENANT_ID_INVALID);
        }
        if (!isRequestTenantIDGuidParsed)
        {
            throw new UnauthorizedAccessException(MESSAGE_TENANT_ID_INVALID_REQUEST);
        }

        if (httpHeaderTenantIDGuid == requestTenantIDGuid)
        {
            isAuthorizedTenant = true;
        }
        if (!isAuthorizedTenant)
        {
            throw new UnauthorizedAccessException("Authorization: Unauthorized TenantID.");
        }

        // Get auth
        var auths = DataMethods.JwtAuthentication_Select(requestTenantIDGuid);
        if (auths == null || auths.Count == 0)
        {
            throw new UnauthorizedAccessException(MESSAGE_TENANT_ID_UNKNOWN);
        }

        var authorizationClaimType = auths[0].RoleAttributeName;
        var authorizationValueSuperuser = auths[0].SuperUserAttributeValue;

        bool isRgAdmin = false;
        isRgAdmin = ((ClaimsIdentity)identity)?
            .Claims?.Where(claim => claim.Type.EndsWith(authorizationClaimType))?
            .Any(claim => claim.Value == authorizationValueSuperuser) ?? false;
        bool isAuthorizedUser = false;
        if (isRgAdmin)
        {
            isAuthorizedUser = true;
        }
        else
        {
            // Verify that "sub" in the JWT token matches the UserID in the request
            string? claimUserID = ((ClaimsIdentity)identity)?
                .Claims?.Where(claim => claim.Type == auths[0].UserIdattributeName)?
                .FirstOrDefault()?
                .Value;
            var requestedUserID = request.UserID.ToString();
            if (claimUserID?.ToLower() == requestedUserID?.ToLower())
            {
                isAuthorizedUser = true;
            }
        }
        if (!isAuthorizedUser)
        {
            throw new UnauthorizedAccessException("Authorization: Unauthorized UserID.");
        }
    }
}
