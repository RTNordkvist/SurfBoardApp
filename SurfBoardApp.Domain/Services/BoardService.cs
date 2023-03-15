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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BoardService(SurfBoardAppContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BoardVM> GetBoard(int id)
        {
            // todo move to boardservice
            var board = await _context.Board
                .Include(x => x.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

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

            return model;
        }

        public async Task<IndexVM> GetBoardModels(IndexVM model)
        {
            var boards = _context.Board.Include(x => x.Images).Include(x => x.Bookings).OrderBy(x => x.Name).AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchString))
            {
                boards = boards.Where(b => b.Name.Contains(model.SearchString));
            }

            if (model.BookingStartDate != null && model.BookingEndDate != null)
            {
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

        public async Task<bool> AddBoard(CreateBoardVM model)
        {
            var images = new List<Image>();
            if (model.Images != null)
            {
                foreach (var file in model.Images)
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

            _context.Add(board);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
