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
    public class BoardsApiController : Controller
    {
        //DBContext is injected through dependency injection
        private readonly BoardService _boardService;

        public BoardsApiController(BoardService boardService)
        {
            _boardService = boardService;
        }

        // GET: Boards
        public async Task<IActionResult> GetBoards(IndexVM model)
        {
            //sets pagenumber and pagesize. Can later be changed to user input.
            if (model.PageNumber == null)
            {
                model.PageNumber = 1;
            }

            model.PageSize = 12;

            if (!ModelState.IsValid)
                return BadRequest(ModelState); // fejlkode

            if (model.BookingEndDate < model.BookingStartDate)
            {
                ModelState.AddModelError("InvalidEndDate", "End date cannot be before start date");
                model.BookingEndDate = null;
                model.Boards = new PaginatedList<IndexBoardVM>(new(), 0, 1, model.PageSize);
                return BadRequest(ModelState);
            }

            var result = await _boardService.GetBoardModels(model);

            return Ok(result);
        }

        // GET: Boards/Details/5
        [Authorize(Roles = "Admin")]
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

        // POST: Boards/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBoard(CreateBoardVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _boardService.AddBoard(model);

            return Ok(result);
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBoard(EditBoardVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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

        // POST: Boards/Delete/5
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
