using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDo_List.Models;

namespace ToDoApp.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Tarea")]
        public string Title { get; set; }

        [StringLength(500)]
        [DisplayName("Descripción")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Completado")]
        public bool IsCompleted { get; set; }

        [Required]
        [DisplayName("Fecha de Creación")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Fecha de Vencimiento")]
        public DateTime? DueDate { get; set; }


        //[Required]
        public string UserId { get; set; }

        //[ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
