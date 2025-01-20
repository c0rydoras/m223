using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.Core.Models;

public class Booking
{
    public const string CollectionName = "Booking";

    [Key]
    public int Id { get; set; }

    [ForeignKey("Source")]
    public int? SourceId { get; set; }

    [ForeignKey("Destination")]
    public int? DestinationId { get; set; }

    public decimal Amount { get; set; }

    public Ledger? Source { get; set; }
    public Ledger? Destination { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime Timestamp { get; set; }
}
