using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfBoardApp.Data;
using SurfBoardApp.Models;
using SurfBoardApp.ViewModels.Boards;

namespace SurfBoardApp.Controllers
{
    public class BoardsController : Controller
    {
        private readonly SurfBoardAppContext _context;

        public BoardsController(SurfBoardAppContext context)
        {
            _context = context;
        }

        // GET: Boards
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            var boards = _context.Board.Include(x => x.Images).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                boards = boards.Where(b => b.Name.Contains(searchString));
            }

            int pageSize = 12; //number of images on index page

            return View(await PaginatedList<Board>.CreateAsync(boards.AsNoTracking(), pageNumber ?? 1, pageSize));
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

            if (boardModel.NewImages != null)
            {
                //If there is no existing images for the board, a new empty list is created to contain the new images
                if (board.Images == null)
                {
                    board.Images = new List<Image>();
                }

                //New files are read and converted to the Image Class and added to the Board
                foreach (var file in boardModel.NewImages)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);

                        var Image = new Image
                        {
                            BoardId= board.Id,
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveImage(int boardId, int imageId)
        {
            var board = await _context.Board.Include(b => b.Images).FirstOrDefaultAsync(b => b.Id == boardId);

            if (board == null)
            {
                return NotFound();
            }

            var image = board.Images.FirstOrDefault(i => i.Id == imageId);

            if (image == null)
            {
                return NotFound();
            }

            board.RemoveImage(image);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = boardId });
        }

    }
}
