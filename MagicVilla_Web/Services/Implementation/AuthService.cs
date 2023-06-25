using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.Business;

namespace MagicVilla_Web.Services.Implementation
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory httpClient;

        private string villaUrl;

        public AuthService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            this.httpClient = httpClient;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestDTO loginRequest)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequest,
                Url = villaUrl + "/api/UsersAuth/login",
            });
        }

        public Task<T> RegisterAsync<T>(RegisterationRequestDTO userDTO)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = userDTO,
                Url = villaUrl + "/api/UsersAuth/register",
            });
        }
    }
   
}
