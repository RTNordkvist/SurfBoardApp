namespace SurfBoardApp.Blazor.Client.ClientModels
{
    public class BoardFilterOptions
    {
        public string? SearchValue { get; set; }

        public double? MinLength { get; set; }
        public double? MaxLength { get; set; }

        public double? MinWidth { get; set; }
        public double? MaxWidth { get; set; }

        public double? MinThickness { get; set; }
        public double? MaxThickness { get; set; }

        public double? MinVolume { get; set; }
        public double? MaxVolume { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public void Reset()
        {
            SearchValue = null;
            MinLength = null;
            MaxLength = null;
            MinWidth = null;
            MaxWidth = null;
            MinThickness = null;
            MaxThickness = null;
            MinVolume = null;
            MaxVolume = null;
            MinPrice = null;
            MaxPrice = null;
        }
    }
}
