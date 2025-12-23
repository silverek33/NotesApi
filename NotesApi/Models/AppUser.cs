
using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models;

public class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string PasswordHash { get; set; } = default!;

    public ICollection<Note> Notes { get; set; } = new List<Note>();
}
