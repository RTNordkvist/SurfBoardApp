using SurfBoardApp.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels
{
    public class EditBoardVM : CreateBoardVM
    {
        public int Id { get; set; }

        public List<Image>? ExistingImages { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastEditedDate { get; set; }
    }
}
