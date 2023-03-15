using SurfBoardApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Data.Interfaces
{
    internal interface IBoardRepository
    {
        IEnumerable<Board> GetStudents();
        Board GetBoardByID(int boardId);
        void AddBoard(Board board);
        void DeleteBoard(int boardID);
        void UpdateBoard(Board board);
        void Save();
    }
}
