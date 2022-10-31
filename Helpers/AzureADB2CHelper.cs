namespace Sitecore.Foundation.Azure.Helpers
{
    using System.Threading.Tasks;
    using Sitecore.Foundation.Azure.Models;
    using Sitecore.Foundation.Azure.Service;
    using Newtonsoft.Json;

    /// <summary>
    /// Helper class to call Graph API.
    /// </summary>
    public class AzureADB2CHelper
    {

        /// <summary>
        /// Retrieve Azure AD B2C objectId for the specified email.
        /// </summary>
        /// <param name="email">the email of the user to retrieve the object id</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<string> GetUserObjectId(string email)
        {
            string result;
            var clientId = Sitecore.Configuration.Settings.GetSetting("AzureB2C.ClientID");
            var tenantId = Sitecore.Configuration.Settings.GetSetting("AzureB2C.TenantId");
            var clientsecret = Sitecore.Configuration.Settings.GetSetting("AzureB2C.ClientSecret");
            var client = new AzureB2CGraphClient(clientId, clientsecret, tenantId);
            result = await client.GetUserByEmail(email);

            var userObject = JsonConvert.DeserializeObject<GraphUser>(result);
            if (userObject != null && userObject.Users != null && userObject.Users.Count != 0)
            {
                return userObject.Users[0].id;
            }

            return string.Empty;
        }
    }
}