using System.ComponentModel.DataAnnotations;

public class EditBookingVM
{
    public int Id { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public string? BoardName { get; set; }

}
