using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardModel = SurfBoardApp.Data.Models.Board;
using SurfBoardApp.Data;

namespace WebAPISurf.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebApiController : ControllerBase
    {
        private readonly SurfBoardAppContext _dbcontext;

        public WebApiController(SurfBoardAppContext context)
        {
            _dbcontext = context;
        }

        [HttpGet]
        [Route("Board")]
        public async Task<IActionResult> GetBoard()
        {
            try
            {
                List<BoardModel> listBoards = _dbcontext.Board.Select(b => new BoardModel { Id = b.Id, Name = b.Name }).ToList();
                if (listBoards != null)
                {
                    return Ok(listBoards);
                }
                return Ok("There are no boards in the database");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Board")]
        public async Task<IActionResult> CreateBoard([FromBody] SurfBoardApp.Data.Models.Board board)
        {
            try
            {
                if (board != null)
                {
                    _dbcontext.Board.Add(board);
                    _dbcontext.SaveChanges();
                    return Ok("Board created successfully");
                }
                return BadRequest("Board object is null");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Board {id}")]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            var board = await _dbcontext.Board.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }

            _dbcontext.Board.Remove(board);
            await _dbcontext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("Board {id}")]
        public async Task<ActionResult<SurfBoardApp.Data.Models.Board>> GetBoard(int id)
        {
            var board = await _dbcontext.Board.FindAsync(id);

            if (board == null)
            {
                return NotFound();
            }

            return board;
        }

        [HttpPut("Board {id}")]
        public async Task<IActionResult> UpdateBoard(int id, SurfBoardApp.Data.Models.Board board)
        {
            if (id != board.Id)
            {
                return BadRequest();
            }

            _dbcontext.Entry(board).State = EntityState.Modified;

            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool BoardExists(long id)
        {
            return _dbcontext.Board.Any(e => e.Id == id);
        }
    }
}