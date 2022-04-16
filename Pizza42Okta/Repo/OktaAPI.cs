using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pizza42Okta.Repo
{
    public class OktaAPI
    {
        private static string ClientId = "T3TrGwKcu1klLd5wWdEIScO17OXoPODM";
        private static string ClientSecret = "JhDXm1mtpxkHsrEPd3_V_KaMf9KJFJ6sKYeTmAqPvwHa0_1VTtDI99ZUSnffZiXF";
        private static string Audiance = "https://dev-zjcjakjo.us.auth0.com/api/v2/";
        private static string Domain = "https://dev-zjcjakjo.us.auth0.com";
        private readonly string _UserId;
        public OktaAPI(string userId)
        {
            this._UserId = userId;
        }

        private OktaUser _OktaUser;
        private OktaUser OktaUser
        {
            get
            {
                if(_OktaUser == null)
                {
                    var client = new RestClient($"{Domain}/api/v2/users/{_UserId}");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("authorization", $"Bearer {BearerToken}");
                    var response = client.Execute(request);
                    var oktaUser = JsonConvert.DeserializeObject<OktaUser>(response.Content);

                    if (oktaUser != null)
                        _OktaUser = oktaUser;
                }

                return _OktaUser;
            }
        }
        public List<PizzaOrder> GetOrderHistory()
        {
            return this.OktaUser.Orders.Orders;
        }
        public PizzaOrder AddOrder(string orderName)
        {
            var created = System.DateTime.UtcNow.ToString("MM/dd/yyyy");
            var orders = OktaUser.Orders;

            var newOrder = new PizzaOrder()
            {
                Created = created,
                OrderName = orderName
            };

            orders.Orders.Add(newOrder);

            var orderJSON = "{\"user_metadata\":{\"orders\":" + JsonConvert.SerializeObject(orders.Orders) + "}}";

            var client = new RestClient($"{Domain}/api/v2/users/{_UserId}");
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("authorization", $"Bearer {BearerToken}");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", orderJSON, ParameterType.RequestBody);
            var response = client.Execute(request);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
                return newOrder;
            else
                return null;
        }

        private string _BearerToken;

        private string BearerToken
        {
            get
            {
                if (string.IsNullOrEmpty(_BearerToken))
                {
                    var client = new RestClient($"{Domain}/oauth/token");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&client_id={ClientId}&client_secret={ClientSecret}&audience={Audiance}", ParameterType.RequestBody);
                    var response = client.Execute(request);
                    var authenticationTokenUser = JsonConvert.DeserializeObject<AuthenticationTokenUser>(response.Content);
                    if (authenticationTokenUser != null)
                    {
                        _BearerToken = authenticationTokenUser.AccessToken;
                    }
                }

                return _BearerToken;
            }
        }
    }
}
