﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels
{
    public class CreateBoardVM
    {
        [Required]
        [MinLength(1, ErrorMessage = "The board name must be at least 1 character long")]
        public string Name { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "The length must be a positive value")]
        public double? Length { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "The width must be a positive value")]
        public double? Width { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "The thickness must be a positive value")]
        public double? Thickness { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "The volume must be a positive value")]
        public double? Volume { get; set; }

        public string? Type { get; set; }

        [Required]
        [Range(0.0, (double)decimal.MaxValue, ErrorMessage = "The price must be a positive value")]
        public decimal Price { get; set; }

        public string? Equipment { get; set; }

        public List<IFormFile>? Images { get; set; }

        public bool MembersOnly { get; set; }
    }
}
