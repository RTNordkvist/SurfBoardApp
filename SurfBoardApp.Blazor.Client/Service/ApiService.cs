using SurfBoardApp.Blazor.Shared.ViewModels;
using SurfBoardApp.Blazor.Shared.ViewModels.ApplicationUserViewModels;
using SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels;
using System.Net.Http.Json;
using System.Text.Json;

namespace SurfBoardApp.Blazor.Client.Service
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BoardVM>> GetBoards()
        {
            var result = await _httpClient.GetFromJsonAsync<List<BoardVM>>("api/boards/GetBoards");

            return result;
        }

        public async Task<RequiredConfirmationVM<EditBoardVM>> EditBoard(BoardVM model)
        {
            var result = await _httpClient.PostAsJsonAsync("api/boards/EditBoard", model);

            if (result.IsSuccessStatusCode)
            {
                var responseBody = await result.Content.ReadAsStringAsync();
                RequiredConfirmationVM<EditBoardVM> requiredConfirmation = JsonSerializer.Deserialize<RequiredConfirmationVM<EditBoardVM>>(responseBody);

                return requiredConfirmation;
            }

            if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception();
            }

            throw new Exception();
        }

        public async Task<UserVM> GetCurrentUser()
        {
            var result = await _httpClient.GetFromJsonAsync<UserVM>("api/user/GetCurrentUser");

            return result;
        }
    }
}
