
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApi.Models;

public class Note
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = default!;

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = default!;
}
