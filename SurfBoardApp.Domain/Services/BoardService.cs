using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using SurfBoardApp.Blazor.Shared.ViewModels;
using SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels;
using SurfBoardApp.Blazor.Shared.ViewModels.BookingViewModels;
using SurfBoardApp.Data;
using SurfBoardApp.Data.Models;
using SurfBoardApp.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Domain.Services
{
    public class BoardService
    {
        private readonly SurfBoardAppContext _context;

        public BoardService(SurfBoardAppContext context)
        {
            _context = context;
        }

        // Receives an id of a board and returns a board view model
        public async Task<BoardVM> GetBoard(int id)
        {
            // The board model is found in the DBcontext from the id
            var board = await _context.Board
                .Include(x => x.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            // The board model is projected to a view model
            var model = new BoardVM
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
                Images = board.Images
            };

            // The view model is returned
            return model;
        }

        // Receives an id of a board and returns a board view model for editing
        public async Task<EditBoardVM> GetEditBoard(int id)
        {
            // The board model is found in the DBcontext from the id
            var board = await _context.Board.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);

            // The board model is projected to a view model
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

            // The view model is returned
            return boardModel;
        }

        // Receives a view model 
        public async Task<IndexVM> GetBoardModels(IndexVM model)
        {
            // All boards from the DBcontext is found
            var boards = _context.Board.Include(x => x.Images).Include(x => x.Bookings).OrderBy(x => x.Name).AsQueryable();

            // If the view model contains a search string, the list of boards is filtered to match the search string
            if (!string.IsNullOrEmpty(model.SearchString))
            {
                boards = boards.Where(b => b.Name.Contains(model.SearchString));
            }

            // If the view model contains booking dates, the list of boards is filtered to only hold the boards without existing bookings in the selected period
            if (model.BookingStartDate != null && model.BookingEndDate != null)
            {
                boards = boards.Where(b => b.Bookings == null || !b.Bookings.Any(x => x.StartDate <= model.BookingEndDate && x.EndDate >= model.BookingStartDate));
            }

            // The list of board models is projected to a list of view models
            IQueryable<IndexBoardVM> boardVMs = boards.Select(x => new IndexBoardVM
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Price = x.Price,
                Image = x.Images != null ? x.Images.FirstOrDefault() : null //Checks if Images is null. Returns first picture in the list if it's not null. Otherwise returns null.
            }).AsNoTracking();

            // The list of view models is projected to a paginated list
            var paginatedBoards = await PaginatedList<IndexBoardVM>.CreateAsync(boardVMs, model.PageNumber ?? 1, model.PageSize);

            // The paginated list is added to the input viewmodel
            model.Boards = paginatedBoards;

            // The input view model is returned
            return model;
        }

        // Adds a board to DBcontext from a view model
        public async Task<bool> AddBoard(CreateBoardVM model)
        {
            var images = new List<Image>();

            // if the Image list in the viewmodel is not empty, each image is converted to a string
            if (model.Images != null)
            {
                foreach (var file in model.Images)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);

                        // An Image obejct is created and populated with a base 54 encoded string which can be explicit set by an img source
                        var Image = new Image
                        {
                            Picture = "data:" + file.ContentType + ";base64, " + s
                        };
                        images.Add(Image);
                    }
                };
            }

            // The view model is projected to a board model
            var board = new Board
            {
                Name = model.Name,
                Length = model.Length,
                Width = model.Width,
                Thickness = model.Thickness,
                Volume = model.Volume,
                Type = model.Type,
                Price = model.Price,
                Equipment = model.Equipment,
                Images = images
            };

            // The model is saved to the database
            _context.Add(board);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateBoard(EditBoardVM model)
        {
            var board = await _context.Board.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == model.Id);

            if (board == null)
            {
                return false;
            }

            board.Id = model.Id;
            board.Name = model.Name;
            board.Length = model.Length;
            board.Width = model.Width;
            board.Thickness = model.Thickness;
            board.Volume = model.Volume;
            board.Type = model.Type;
            board.Price = model.Price;
            board.Equipment = model.Equipment;
            board.Images = model.ExistingImages;

            if (model.Images != null)
            {
                //If there is no existing images for the board, a new empty list is created to contain the new images
                if (board.Images == null)
                {
                    board.Images = new List<Image>();
                }

                //New files are read and converted to the Image Class and added to the Board
                foreach (var file in model.Images)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);

                        // An Image obejct is created and populated with a base 54 encoded string which can be explicit set by an img source
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

            return true;
        }

        public async Task<bool> RemoveBoard(int id)
        {
            var board = await _context.Board.FindAsync(id);

            if (board == null)
            {
                return false;
            }

            _context.Board.Remove(board);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveImage(int boardId, int imageId)
        {
            // Retrieve the board with the specified boardId, including its images
            var board = await _context.Board.Include(b => b.Images).FirstOrDefaultAsync(b => b.Id == boardId);

            // If the board is not found, return a NotFound result
            if (board == null)
            {
                throw new Exception();
            }

            // Retrieve the image with the specified imageId from the board's images
            var image = board.Images.FirstOrDefault(i => i.Id == imageId);

            // If the image is not found, an exception is thrown
            if (image == null)
            {
                throw new Exception();
            }

            // Removes the image from DBcontext
            _context.Remove(image);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
