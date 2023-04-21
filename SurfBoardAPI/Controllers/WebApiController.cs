using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardModel = SurfBoardApp.Data.Models.Board;
using SurfBoardApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPISurf.Controllers
{
    [ApiController]
    // forklaring på hvorfor vi bruger [Route("api/boards")] istedet for [Route("api/[controller] : 
    //Brug [Route("api/boards")]i stedet for [Route("api/[controller]")] anses for at være mere RESTful,
    //fordi det overholder de almindelige konventioner for en RESTful API.
    //RESTful API'er anbefales det at bruge små bogstaver og flertal ressourcenavne i URL'erne.
    //Dette gør API'en mere konsistent og lettere for kunder at forstå og bruge.
    [Route("api/boards")] 
    public class BoardController : ControllerBase
    {
        private readonly SurfBoardAppContext _context;

        public BoardController(SurfBoardAppContext context)
        {
            _context = context;
        }

        // Get a list of all boards
        [HttpGet]
        public async Task<IActionResult> GetBoards()
        {
            try
            {
                var boards = await _context.Board
                    .Select(b => new BoardModel { Id = b.Id, Name = b.Name })
                    .ToListAsync();

                if (boards != null && boards.Count > 0)
                {
                    return Ok(boards);
                }
                else
                {
                    return NotFound("No boards found in the database");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Create a new board
        [HttpPost]
        public async Task<IActionResult> CreateBoard([FromBody] SurfBoardApp.Data.Models.Board board)
        {
            try
            {
                if (board == null)
                {
                    return BadRequest("Board object is null");
                }

                _context.Board.Add(board);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, board);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Delete a board by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            var board = await _context.Board.FindAsync(id);
            if (board == null)
            {
                return NotFound($"Board with ID {id} not found");
            }

            _context.Board.Remove(board);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Get a specific board by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<SurfBoardApp.Data.Models.Board>> GetBoard(int id)
        {
            var board = await _context.Board.FindAsync(id);

            if (board == null)
            {
                return NotFound($"Board with ID {id} not found");
            }

            return Ok(board);
        }

        // Update a board by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBoard(int id, SurfBoardApp.Data.Models.Board board)
        {
            if (id != board.Id)
            {
                return BadRequest("Board ID mismatch");
            }

            try
            {
                _context.Entry(board).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardExists(id))
                {
                    return NotFound($"Board with ID {id} not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Check if a board with a specific ID exists
        private bool BoardExists(int id) // Changed parameter type to int for consistency
        {
            return _context.Board.Any(e => e.Id == id);
        }
    }
}
