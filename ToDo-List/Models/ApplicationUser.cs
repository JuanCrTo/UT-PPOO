using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using ToDoApp.Models;

namespace ToDo_List.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(30, ErrorMessage = "El nombre no puede exceder los 30 caracteres.")]
        public string Name { get; set; }

        public virtual ICollection<TodoItem> TodoItems { get; set; }
    }
}