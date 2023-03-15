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
    public class BoardsController : BaseController
    {
        //DBContext is injected through dependency injection
        private readonly SurfBoardAppContext _context;
        private readonly BoardService _boardService;

        public BoardsController(SurfBoardAppContext context, UserManager<ApplicationUser> userManager, BoardService boardService) : base(userManager)
        {
            _context = context;
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

            var result = _boardService.GetBoard((int)id);

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
        public async Task<IActionResult> Create(CreateBoardVM boardModel)
        {
            if (!ModelState.IsValid)
            {
                return View(boardModel);
            }

            _boardService.AddBoard(boardModel);

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

            // todo move to boardservice and return EditBoardVM
            var board = await _context.Board.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            var boardModel = new EditBoardVM
            {
                Id = board.Id,
                Name = board.Name,
                Length = board.Length,
                Width = board.Width,
                Thickness = board.Thickness,
                Volume = board.Volume,
                Type = board.Type,
                Price = board.Price,
                Equipment = board.Equipment,
                ExistingImages = board.Images
            };

            return View(boardModel);
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBoardVM boardModel)
        {
            if (!ModelState.IsValid)
            {
                return View(boardModel);
            }

            var board = await _context.Board.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == boardModel.Id);
            if (board == null)
            {
                return NotFound();
            }

            board.Id = boardModel.Id;
            board.Name = boardModel.Name;
            board.Length = boardModel.Length;
            board.Width = boardModel.Width;
            board.Thickness = boardModel.Thickness;
            board.Volume = boardModel.Volume;
            board.Type = boardModel.Type;
            board.Price = boardModel.Price;
            board.Equipment = boardModel.Equipment;
            board.Images = boardModel.ExistingImages;

            if (boardModel.Images != null)
            {
                //If there is no existing images for the board, a new empty list is created to contain the new images
                if (board.Images == null)
                {
                    board.Images = new List<Image>();
                }

                //New files are read and converted to the Image Class and added to the Board
                foreach (var file in boardModel.Images)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);

                        var Image = new Image
                        {
                            BoardId = board.Id,
                            Picture = "data:" + file.ContentType + ";base64, " + s
                        };
                        board.Images.Add(Image);
                    }
                };
            }

            _context.Update(board);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Boards/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .FirstOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // POST: Boards/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Board == null)
            {
                return Problem("Entity set 'SurferDemoContext.Board'  is null.");
            }
            var board = await _context.Board.FindAsync(id);
            if (board != null)
            {
                _context.Board.Remove(board);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardExists(int id)
        {
            return _context.Board.Any(e => e.Id == id);
        }

        //Remove Image Action (button click)
        // This method handles an HTTP POST request and is only accessible with a valid anti-forgery token
        [HttpPost]
        [ValidateAntiForgeryToken]
        // This method can only be accessed by users with the "Admin" role
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveImage(int boardId, int imageId)
        {
            // Retrieve the board with the specified boardId, including its images
            var board = await _context.Board.Include(b => b.Images).FirstOrDefaultAsync(b => b.Id == boardId);

            // If the board is not found, return a NotFound result
            if (board == null)
            {
                return NotFound();
            }

            // Retrieve the image with the specified imageId from the board's images
            var image = board.Images.FirstOrDefault(i => i.Id == imageId);

            // If the image is not found, return a NotFound result
            if (image == null)
            {
                return NotFound();
            }

            // Remove the image from the board
            board.RemoveImage(image);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the Edit method of the current controller with the boardId parameter
            return RedirectToAction(nameof(Edit), new { id = boardId });
        }
    }
}
