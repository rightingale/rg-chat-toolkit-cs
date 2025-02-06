using rg_chat_toolkit_api_cs.Chat;
using rg_chat_toolkit_api_cs.Data.Models;
using rg_chat_toolkit_cs.Chat;
using System.Security.Claims;
using System.Security.Principal;

namespace rg_chat_toolkit_api_cs;

public static class AuthenticationHelper
{
    const string CLAIMS_KEY_USER_ROLE_RGADMIN = "rg:user_role";
    const string CLAIMS_VALUE_USER_ROLE_RGADMIN_ADMIN = "administrator";

    public static void Authorize(IIdentity? identity, RequestBase request)
    {
        // Verify that the TenantID in the HTTP Header value matches the TenantID in the request
        bool isAuthorizedTenant = false;
        var httpContext = new HttpContextAccessor().HttpContext;
        var httpHeaderTenantID = httpContext?.Request?.Headers["TenantID"].ToString();
        var requestTenantID = request.TenantID.ToString();
        if (httpHeaderTenantID == requestTenantID)
        {
            isAuthorizedTenant = true;
        }
        if (!isAuthorizedTenant)
        {
            throw new UnauthorizedAccessException("Authorization: Unauthorized TenantID.");
        }

        bool isRgAdmin = false;
        isRgAdmin = ((ClaimsIdentity)identity)?
            .Claims?.Where(claim => claim.Type.EndsWith(CLAIMS_KEY_USER_ROLE_RGADMIN))?
            .Any(claim => claim.Value == CLAIMS_VALUE_USER_ROLE_RGADMIN_ADMIN) ?? false;
        bool isAuthorizedUser = false;
        if (isRgAdmin)
        {
            isAuthorizedUser = true;
        }
        else
        {
            // Verify that "sub" in the JWT token matches the UserID in the request
            const string CLAIMS_KEY_USER_ID = "custom:producer_token";
            string? claimUserID = ((ClaimsIdentity)identity)?
                .Claims?.Where(claim => claim.Type == CLAIMS_KEY_USER_ID)?
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
