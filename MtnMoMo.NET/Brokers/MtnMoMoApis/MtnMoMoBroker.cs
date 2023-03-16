// --------------------------------------------------------------- 
// Copyright (c) Coalition of the Good-Hearted Engineers 
// ---------------------------------------------------------------

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MtnMoMo.NET.Models.Configurations;
//using MtnMoMo.NET.Models.ExternalProducts;
using RESTFulSense.Clients;

namespace MtnMoMo.NET.Brokers.MtnMoMoApis
{
    internal partial class MtnMoMoBroker : IMtnMoMoBroker
    {
        private readonly ApiConfigurations apiConfigurations;
        private readonly IRESTFulApiFactoryClient apiClient, authApiClient;
        private readonly HttpClient httpClient, authHttpClient;

        public MtnMoMoBroker(ApiConfigurations apiConfigurations)
        {
            this.apiConfigurations = apiConfigurations;
            this.httpClient = SetupHttpClient();
            this.apiClient = SetupApiClient();
            this.authHttpClient = SetupAuthHttpClient();
            this.authApiClient = SetupAuthApiClient();
        }

        private async ValueTask<T> GetAuthAsync<T>(string relativeUrl) =>
            await this.authApiClient.GetContentAsync<T>(relativeUrl);

        private async ValueTask<T> GetAsync<T>(string relativeUrl) =>
            await this.apiClient.GetContentAsync<T>(relativeUrl);

        private async ValueTask<T> PostAsync<T>(string relativeUrl, T content) =>
            await this.apiClient.PostContentAsync(relativeUrl, content);

        private async ValueTask<TResult> PostAsync<TRequest, TResult>(string relativeUrl, TRequest content) =>
            await this.apiClient.PostContentAsync<TRequest, TResult>(relativeUrl, content, mediaType: "application/json");

        private async ValueTask<T> PutAsync<T>(string relativeUrl, T content) =>
            await this.apiClient.PutContentAsync(relativeUrl, content);

        private async ValueTask<T> DeleteAsync<T>(string relativeUrl) =>
            await this.apiClient.DeleteContentAsync<T>(relativeUrl);

        private HttpClient SetupHttpClient()
        {
            var httpClient =  new HttpClient()
            {
                BaseAddress =
                    new Uri(uriString: this.apiConfigurations.BaseUrl),
            };

            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue(
                    scheme: "Bearer",
                    parameter: this.apiConfigurations.AccessToken);

            httpClient.DefaultRequestHeaders.Add(
                name: "Ocp-Apim-Subscription-Key",
                value: this.apiConfigurations.SubscriptionKey);

            httpClient.DefaultRequestHeaders.Add (
                name: "X-Target-Environment", 
                value: this.apiConfigurations.TargetEnvironment);
                        
            return httpClient;
        }

        private HttpClient SetupAuthHttpClient()
        {
            var httpClient =  new HttpClient()
            {
                BaseAddress =
                    new Uri(uriString: this.apiConfigurations.BaseUrl),
            };

            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue(
                    scheme: "Bearer",
                    parameter: this.apiConfigurations.AccessToken);

            httpClient.DefaultRequestHeaders.Add(
                name: "Ocp-Apim-Subscription-Key",
                value: this.apiConfigurations.SubscriptionKey);


            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue(
                    scheme: "Basic", 
                    parameter: Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{this.apiConfigurations.ApiUser}:{this.apiConfigurations.ApiKey}")));

            return httpClient;
        }

        private IRESTFulApiFactoryClient SetupApiClient() =>
            new RESTFulApiFactoryClient(this.httpClient);

        private IRESTFulApiFactoryClient SetupAuthApiClient() =>
            new RESTFulApiFactoryClient(this.authHttpClient);
    }
}