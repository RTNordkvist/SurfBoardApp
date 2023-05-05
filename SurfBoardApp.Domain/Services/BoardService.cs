using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using SurfBoardApp.Blazor.Shared.ViewModels;
using SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels;
using SurfBoardApp.Data;
using SurfBoardApp.Data.Models;
using SurfBoardApp.Domain.Exceptions;

namespace SurfBoardApp.Domain.Services
{
    public class BoardService
    {
        private readonly SurfBoardAppContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BoardService(SurfBoardAppContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
                Equipment = board.Equipment
            };

            if (board.Images != null)
            {
                model.Images = new List<ImageVM>();

                foreach (var image in board.Images)
                {
                    var imageVM = new ImageVM
                    {
                        Id = image.Id,
                        BoardId = image.BoardId,
                        Picture = image.Picture
                    };
                    model.Images.Add(imageVM);
                }
            }


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
                MembersOnly = board.MembersOnly,
                Version = board.Version
            };

            if (board.Images != null)
            {
                boardModel.ExistingImages = new List<ImageVM>();

                foreach (var image in board.Images)
                {
                    var imageVM = new ImageVM
                    {
                        Id = image.Id,
                        BoardId = image.BoardId,
                        Picture = image.Picture
                    };
                    boardModel.ExistingImages.Add(imageVM);
                }
            }

            // The view model is returned
            return boardModel;
        }

        // Receives a view model 
        public async Task<IndexVM> GetBoardModels(IndexVM model)
        {
            // All boards from the DBcontext is found
            var boards = _context.Board
                .Include(x => x.Images)
                .Include(x => x.Bookings)
                .OrderBy(x => x.Name)
                .AsQueryable();


            //Filtering 
            // Call the filter method to filter the boards based on the input view model
            var filteredBoards = FilterBoards(boards, model);

            // The list of board models is projected to a list of view models
            IQueryable<IndexBoardVM> boardVMs = filteredBoards.Select(x => new IndexBoardVM
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Price = x.Price,
                Image = x.Images != null ? x.Images.Select(y => new ImageVM {Id = y.Id, BoardId = y.BoardId, Picture = y.Picture }).FirstOrDefault() : null //Checks if Images is null. Returns first picture in the list if it's not null. Otherwise returns null.
            }).AsNoTracking();

            // The list of view models is projected to a paginated list
            var paginatedBoards = await PaginatedList<IndexBoardVM>.CreateAsync(boardVMs, model.PageNumber ?? 1, model.PageSize);

            // The paginated list is added to the input viewmodel
            model.Boards = paginatedBoards;

            // The input view model is returned
            return model;
        }
        //IQueryable repræsenterer en samling af data, der kan forespørges ved hjælp af LINQ-operatorer,
        //men den faktiske udførelse af forespørgslen udskydes, indtil dataene er opregnet.
        //Det betyder, at forespørgslen ikke udføres, før resultaterne rent faktisk er nødvendige
        private IQueryable<Board> FilterBoards(IQueryable<Board> boards, IndexVM model)
        {
            // Checks if the user is logged in and returns either the full list of boards including boards for MembersOnly or a limited amount of boards if the user is not logged in.
            boards = boards.Where(x => IsUserAuthenticated() ? true : !x.MembersOnly);

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

            // Apply search criteria
            if (!string.IsNullOrEmpty(model.SearchParameter) && !string.IsNullOrEmpty(model.SearchValue))
            {
                switch (model.SearchParameter.ToLower())
                {
                    case "name":
                        {
                            boards = boards.Where(b => b.Name.Contains(model.SearchValue));
                            break;
                        }

                    case "length":
                        {
                            double.TryParse(model.SearchValue, out double length);
                            boards = boards.Where(b => b.Length == length);
                            break;
                        }

                    case "width":
                        {
                            double.TryParse(model.SearchValue, out double width);
                            boards = boards.Where(b => b.Width == width);
                            break;
                        }

                    case "thickness":
                        {
                            double.TryParse(model.SearchValue, out double thickness);
                            boards = boards.Where(b => b.Thickness == thickness);
                            break;
                        }

                    case "volume":
                        {
                            double.TryParse(model.SearchValue, out double volume);
                            boards = boards.Where(b => b.Volume == volume);
                            break;
                        }

                    case "type":
                        {
                            boards = boards.Where(b => b.Type.Contains(model.SearchValue));
                            break;
                        }

                    case "price":
                        {
                            decimal.TryParse(model.SearchValue, out decimal price);
                            boards = boards.Where(b => b.Price == price);
                            break;
                        }
                }
            }

            // Apply advanced search options
            if (model.SearchLengthFrom.HasValue)
            {
                boards = boards.Where(b => b.Length >= model.SearchLengthFrom.Value);
            }

            if (model.SearchLengthTo.HasValue)
            {
                boards = boards.Where(b => b.Length <= model.SearchLengthTo.Value);
            }

            if (model.SearchWidthFrom.HasValue)
            {
                boards = boards.Where(b => b.Width >= model.SearchWidthFrom.Value);
            }

            if (model.SearchWidthTo.HasValue)
            {
                boards = boards.Where(b => b.Width <= model.SearchWidthTo.Value);
            }

            if (model.SearchThicknessFrom.HasValue)
            {
                boards = boards.Where(b => b.Thickness >= model.SearchThicknessFrom.Value);
            }

            if (model.SearchThicknessTo.HasValue)
            {
                boards = boards.Where(b => b.Thickness <= model.SearchThicknessTo.Value);
            }

            if (model.SearchVolumeFrom.HasValue)
            {
                boards = boards.Where(b => b.Volume >= model.SearchVolumeFrom.Value);
            }

            if (model.SearchVolumeTo.HasValue)
            {
                boards = boards.Where(b => b.Volume <= model.SearchVolumeTo.Value);
            }

            return boards;
        }


        // Adds a board to DBcontext from a view model
        public async Task<int> AddBoard(CreateBoardVM model)
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
                MembersOnly = model.MembersOnly,
                Images = images,
                Version = 1
            };

            // The model is saved to the database
            _context.Add(board);
            await _context.SaveChangesAsync();

            return board.Id;
        }

        public async Task UpdateBoard(EditBoardVM model)
        {
            var board = await _context.Board.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == model.Id);

            if (board == null)
            {
                throw new BoardNotFoundException();
            }

            if (board.Version != model.Version && !model.ConfirmedOverwrite)
            {
                throw new OutdatedBoardInformationException();
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
            board.MembersOnly = model.MembersOnly;
            board.Version += 1;

            if (model.ExistingImages != null)
            {
                board.Images = new List<Image>();

                foreach (var image in model.ExistingImages)
                {
                    var existingImage = new Image
                    {
                        Id = image.Id,
                        BoardId = image.BoardId,
                        Picture = image.Picture
                    };
                    board.Images.Add(existingImage);
                }
            }

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
        }

        public async Task RemoveBoard(int id)
        {
            var board = await _context.Board.FindAsync(id);

            if (board == null)
            {
                throw new BookingNotFoundException();
            }

            _context.Board.Remove(board);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveImage(int boardId, int imageId)
        {
            // Retrieve the board with the specified boardId, including its images
            var board = await _context.Board.Include(b => b.Images).FirstOrDefaultAsync(b => b.Id == boardId);

            // If the board is not found, return a NotFound result
            if (board == null)
            {
                throw new BoardNotFoundException();
            }

            // Retrieve the image with the specified imageId from the board's images
            var image = board.Images.FirstOrDefault(i => i.Id == imageId);

            // If the image is not found, an exception is thrown
            if (image == null)
            {
                throw new ImageNotFoundException();
            }

            // Removes the image from DBcontext
            _context.Remove(image);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        private bool IsUserAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}
