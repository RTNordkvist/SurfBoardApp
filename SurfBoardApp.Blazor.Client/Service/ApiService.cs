using SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels;
using System.Net.Http.Json;

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
    }
}
