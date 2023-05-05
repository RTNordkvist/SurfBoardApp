using System;
using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels
{
    public class EditBoardVM : CreateBoardVM
    {
        public int Id { get; set; }

        public List<ImageVM>? ExistingImages { get; set; }

        public int Version { get; set; }

        public bool ConfirmedOverwrite { get; set; }
    }
}
