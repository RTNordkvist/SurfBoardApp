using SurfBoardApp.Data;
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
    }
}
