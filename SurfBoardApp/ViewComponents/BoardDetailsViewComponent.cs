using Microsoft.AspNetCore.Mvc;
using SurfBoardApp.Domain.Services;

namespace SurfBoardApp.ViewComponents
{
    public class BoardDetailsViewComponent : ViewComponent
    {
        private readonly BoardService _boardService;

        public BoardDetailsViewComponent(BoardService boardService)
        {
            _boardService = boardService;
            // TODO inject needed services
        }

        public async Task<IViewComponentResult> InvokeAsync(int boardId)
        {
            // USAGE in view: @(await Component.InvokeAsync("BoardDetails", new { boardId = 1 }))  
            var model = await _boardService.GetBoard(boardId);

            return View(model);
        }
    }
}
