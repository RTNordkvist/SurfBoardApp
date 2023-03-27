using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels
{
    public class ConfirmEditBoardVM
    {
        public EditBoardVM UserInput { get; set; }
        public EditBoardVM PersistedData { get; set; }
    }
}
