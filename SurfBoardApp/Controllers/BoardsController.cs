using System;
using System.Collections.Generic;
using System.Linq;
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

namespace SurfBoardApp.Controllers
{
    public class BoardsController : Controller
    {
        //DBContext is injected through dependency injection
        private readonly BoardService _boardService;

        public BoardsController(BoardService boardService)
        {
            _boardService = boardService;
        }

        // GET: Boards
        public async Task<IActionResult> Index(IndexVM model)
        {
            //sets pagenumber and pagesize. Can later be changed to user input.
            model.PageNumber = 1;
            model.PageSize = 12;

            if (!ModelState.IsValid)
                return View(model);

            if (model.BookingEndDate < model.BookingStartDate)
            {
                ModelState.AddModelError("InvalidEndDate", "End date cannot be before start date");
                model.BookingEndDate = null;
                model = await _boardService.GetBoardModels(model);
                return View(model);
            }

            model = await _boardService.GetBoardModels(model);

            return View(model);
        }

        // GET: Boards/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(result); // todo convert to a viewmodel instead of entity model
        }

        // GET: Boards/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Boards/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBoardVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _boardService.AddBoard(model);

            return RedirectToAction(nameof(Index));
        }

        // TODO: Refactor from here and below -> move responsibility to BoardService
        // GET: Boards/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _boardService.GetEditBoard((int)id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBoardVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _boardService.UpdateBoard(model);

            if (result == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Boards/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var result = _boardService.GetBoard((int)id);
                return View(result);
            }
            catch (BoardNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: Boards/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _boardService.RemoveBoard(id); //TODO handle error from delete (try-catch?)
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> RemoveImage(int boardId, int imageId)
        {
            try
            {
                await _boardService.RemoveImage(boardId, imageId);
                // Redirect to the Edit method of the current controller with the boardId parameter
                return RedirectToAction(nameof(Edit), new { id = boardId });
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
