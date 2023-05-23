using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfBoardApp.Blazor.Shared.ViewModels;
using SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels;
using SurfBoardApp.Blazor.Shared.ViewModels.BookingViewModels;
using SurfBoardApp.Data;
using SurfBoardApp.Data.Models;
using SurfBoardApp.Domain.Exceptions;
using SurfBoardApp.Domain.Services;

namespace SurfBoardApp.Areas.api
{
    [Route("api/boards/[action]/{id?}")]
    // forklaring på hvorfor vi bruger [Route("api/boards")] istedet for [Route("api/[controller] : 
    //Brug [Route("api/boards")]i stedet for [Route("api/[controller]")] anses for at være mere RESTful,
    //fordi det overholder de almindelige konventioner for en RESTful API.
    //RESTful API'er anbefales det at bruge små bogstaver og flertal ressourcenavne i URL'erne.
    //Dette gør API'en mere konsistent og lettere for kunder at forstå og bruge.
    //[Route("api/boards/[Action]")]
    public class BoardsApiController: Controller
    {
        //DBContext is injected through dependency injection
        private readonly BoardService _boardService;

        public BoardsApiController(BoardService boardService)
        {
            _boardService = boardService;
        }

        public async Task<IActionResult> GetBoards()
        {
            var result = await _boardService.GetBoards();

            return Ok(result);
        }

        public async Task<IActionResult> GetBoard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _boardService.GetBoard((int)id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBoard(CreateBoardVM model)
        {
            var result = await _boardService.AddBoard(model);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBoard(EditBoardVM model)
        {
            try
            {
                await _boardService.UpdateBoard(model);
                return Ok();
            }
            catch (BoardNotFoundException)
            {
                return NotFound();
            }
            catch (OutdatedBoardInformationException)
            {
                var updatedModel = await _boardService.GetEditBoard(model.Id);

                var viewModel = new ConfirmEditBoardVM { PersistedData = updatedModel, UserInput = model };

                return Ok(viewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            try
            {
                await _boardService.RemoveBoard(id);
                return Ok();
            }
            catch (BoardNotFoundException)
            {
                return NotFound();
            }
        }

        //Remove Image Action (button click)
        // This method handles an HTTP POST request and is only accessible with a valid anti-forgery token
        [HttpPost]
        [ValidateAntiForgeryToken]
        // This method can only be accessed by users with the "Admin" role
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage(int boardId, int imageId)
        {
            try
            {
                await _boardService.RemoveImage(boardId, imageId);
                return Ok();
            }
            catch (BoardNotFoundException)
            {
                return NotFound();
            }
            catch (ImageNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
