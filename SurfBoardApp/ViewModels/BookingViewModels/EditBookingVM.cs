using System.ComponentModel.DataAnnotations;

public class EditBookingVM
{
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public string BoardName { get; set; }

    [DataType(DataType.Date)]
    public DateTime OriginalStartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime OriginalEndDate { get; set; }
}
