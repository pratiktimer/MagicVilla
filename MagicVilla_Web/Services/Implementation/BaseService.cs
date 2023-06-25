using MagicVilla_Web.Models;
using MagicVilla_Web.Services.Business;
using Newtonsoft.Json;
using System;
using System.Text;

namespace MagicVilla_Web.Services.Implementation
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel {get;set;}

        public IHttpClientFactory httpClient { get;set;}

        public BaseService(IHttpClientFactory httpClient)
        {
                this.responseModel = new();
                this.httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                if(apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8,"application/json" );
                }
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.GET:
                        message.Method = HttpMethod.Get;
                        break;
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                }
                if (!string.IsNullOrEmpty(apiRequest.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiRequest.Token); 
                }
                HttpResponseMessage httpResponse = null;

                httpResponse = await client.SendAsync(message);

                var apiContent = await httpResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(apiContent);

            }
            catch (Exception ex)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string>()
                    {
                        Convert.ToString(ex.Message),

                    },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);

                return JsonConvert.DeserializeObject<T>(res);
            }
        }
    }
}
