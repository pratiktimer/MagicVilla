using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services.Business
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDTO loginRequest);

        Task<T> RegisterAsync<T>(RegisterationRequestDTO userDTO);
    }
}
