using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDo_List.Models;

namespace ToDoApp.Models
{
    public class TodoItem
    {
        public enum EstadoTarea // Enum definido dentro de la clase
        {
            Pendiente,
            EnDesarrollo,
            Finalizado
        }

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
        [DisplayName("Estado")]
        public EstadoTarea Estado { get; set; } // Reemplaza IsCompleted

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