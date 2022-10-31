using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Sitecore.Foundation.Azure.Service
{
    public class AzureB2CGraphClient
    {

        private string ClientId { get; set; }

        private string ClientSecret { get; set; }

        private string Tenant { get; set; }

        private AuthenticationContext authContext;
        private ClientCredential credential;

        public AzureB2CGraphClient(string clientId, string clientSecret, string tenant)
        {
            // The client_id, client_secret, and tenant are pulled in from the config file
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.Tenant = tenant;

            // The AuthenticationContext is ADAL's primary class, in which you indicate the direcotry to use.
            this.authContext = new AuthenticationContext("https://login.microsoftonline.com/" + tenant);

            // The ClientCredential is where you pass in your client_id and client_secret, which are 
            // provided to Azure AD in order to receive an access_token using the app's identity.
            this.credential = new ClientCredential(clientId, clientSecret);
        }

        public async Task<string> GetUserByEmail(string email)
        {
            string query = $"$select=displayName,id&$filter=identities/any(c:c/issuerAssignedId eq '{email}' and c/issuer eq '{this.Tenant}')";

            return await this.SendNewGraphGetRequest("/users", query);
        }

        /// <summary>
        ///  Generic method to call Graph api
        /// </summary>
        /// <param name="api"> for example  /users</param>
        /// <param name="query"> the query including select and filter </param>
        /// <returns>response as a string</returns>
        public async Task<string> SendNewGraphGetRequest(string api, string query)
        {
            // Get the token using the app's identity (the credential)
            // The first parameter is the resource we want an access_token for the Graph API.
            AuthenticationResult result = await this.authContext.AcquireTokenAsync("https://graph.microsoft.com", credential);

            HttpClient http = new HttpClient();
            string url = "https://graph.microsoft.com/v1.0" + api;
            if (!string.IsNullOrEmpty(query))
            {
                url += "?" + query;
            }

            // Append the access token for the Graph API to the Authorization header of the request, using the Bearer scheme.
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                // throw ? ;
            }

            return await response.Content.ReadAsStringAsync();
        }

    }
}