using BookStore_UI.Contracts;
using BookStore_UI.Models;
using BookStore_UI.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_UI.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _client;
        //private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public AuthenticationRepository(IHttpClientFactory client,
           //ILocalStorageService localStorage,
           AuthenticationStateProvider authenticationStateProvider)
        {
            _client = client;
            //_localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }
        public async Task<bool> Register(RegistrationModel user)
        {
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post
            //    , Endpoints.RegisterEndpoint);
            //request.Content = new StringContent(JsonConvert.SerializeObject(user)
            //    , Encoding.UTF8, "application/json");

            //var client = _client.CreateClient();
            //HttpResponseMessage response = await client.SendAsync(request);

            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var httpClient = _client.CreateClient();
            var response = await httpClient.PostAsync(Endpoints.RegisterEndpoint, content);

            return response.IsSuccessStatusCode;
        }
    }
}
