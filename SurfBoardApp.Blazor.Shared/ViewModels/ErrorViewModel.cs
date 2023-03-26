// This namespace contains the models used in the SurfBoardApp application.

namespace SurfBoardApp.Blazor.Shared.ViewModels
{
    // This class represents an error view model.
    public class ErrorViewModel
    {
        // Gets or sets the request ID associated with the error.
        public string? RequestId { get; set; }
        // Gets a value indicating whether to show the request ID.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}