using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfBoardApp.Data;
using SurfBoardApp.Data.Models;
using SurfBoardApp.Domain.Services;
using SurfBoardApp.ViewModels.BoardViewModels;
using SurfBoardApp.ViewModels.BookingViewModels;

namespace SurfBoardApp.Controllers
{
    public class BoardsController : BaseController
    {
        //DBContext is injected through dependency injection
        private readonly SurfBoardAppContext _context;
        // private readonly BoardService _boardService;

        public BoardsController(SurfBoardAppContext context, UserManager<ApplicationUser> userManager/*, BoardService boardService*/) : base(userManager)
        {
            _context = context;
            //_boardService = boardService;
        }

        // GET: Boards
        public async Task<IActionResult> Index(IndexVM model)
        {
            if (model.BookingEndDate < model.BookingStartDate)
            {
                ModelState.AddModelError("InvalidEndDate", "End date cannot be before start date");
            }

            model = await GetIndexViewModel(model);

            return View(model);
        }

        private async Task<IndexVM> GetIndexViewModel(IndexVM model)
        {
            model.PageNumber = 1;
            model.PageSize = 12;
            model.ShowBookingOptions = false;

            var boards = _context.Board.Include(x => x.Images).Include(x => x.Bookings).OrderBy(x => x.Name).AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchString))
            {
                boards = boards.Where(b => b.Name.Contains(model.SearchString));
            }

            if (model.BookingStartDate != null && model.BookingEndDate != null)
            {
                model.ShowBookingOptions = true;
                boards = boards.Where(b => b.Bookings == null || !b.Bookings.Any(x => x.StartDate <= model.BookingEndDate && x.EndDate >= model.BookingStartDate));
            }

            IQueryable<IndexBoardVM> boardVMs = boards.Select(x => new IndexBoardVM
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Price = x.Price,
                Image = x.Images != null ? x.Images.FirstOrDefault() : null //Checks if Images is null. Returns first picture in the list if it's not null. Otherwise returns null.
            }).AsNoTracking();

            var paginatedBoards = await PaginatedList<IndexBoardVM>.CreateAsync(boardVMs, model.PageNumber ?? 1, model.PageSize);

            model.Boards = paginatedBoards;

            return model;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookBoard(BookBoardVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index), await GetIndexViewModel(new IndexVM {BookingStartDate = model.StartDate, BookingEndDate = model.EndDate})); //TODO Redirect to Index
            }

            if(model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("InvalidEndDate", "End date cannot be before start date");
                //ViewData["InvalidEndDate"] = true;
                return View(nameof(Index), await GetIndexViewModel(new IndexVM { BookingStartDate = model.StartDate, BookingEndDate = model.EndDate })); //TODO unittest
            }

            if (await _context.Booking.AnyAsync(x => x.StartDate <= model.EndDate && x.EndDate >= model.StartDate && x.BoardId == model.BoardId))
            {
                ModelState.AddModelError("BoardUnavailable", "Board is unavailable for the selected period");
                //ViewData["BoardUnavailable"] = true;
                return View(nameof(Index), await GetIndexViewModel(new IndexVM { BookingStartDate = model.StartDate, BookingEndDate = model.EndDate }));
            }

            var booking = new Booking
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                BoardId = model.BoardId,
                CustomerId = _userManager.GetUserId(User)
            };

            _context.Booking.Add(booking);
            _context.SaveChanges();

            return View(new BookingConfirmationVM {StartDate = booking.StartDate, EndDate = booking.EndDate, BoardName = _context.Board.AsNoTracking().First(x => x.Id == booking.BoardId).Name});
        }

        // GET: Boards/Details/5
        public async Task<IActionResult> Details(int? id)

        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .Include(x => x.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // GET: Boards/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Boards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBoardVM boardModel)
        {
            if (!ModelState.IsValid)
            {
                return View(boardModel);
            }

            var images = new List<Image>();

            if (boardModel.Images != null)
            {
                foreach (var file in boardModel.Images)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);

                        var Image = new Image
                        {
                            Picture = "data:" + file.ContentType + ";base64, " + s
                        };
                        images.Add(Image);
                    }
                };
            }

            var board = new Board
            {
                Name = boardModel.Name,
                Length = boardModel.Length,
                Width = boardModel.Width,
                Thickness = boardModel.Thickness,
                Volume = boardModel.Volume,
                Type = boardModel.Type,
                Price = boardModel.Price,
                Equipment = boardModel.Equipment,
                Images = images
            };

            _context.Add(board);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Boards/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

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
            //board.Images = boardModel.ExistingImages;

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
