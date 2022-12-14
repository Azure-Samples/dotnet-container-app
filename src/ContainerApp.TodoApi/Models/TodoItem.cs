using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContainerApp.TodoApi.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Column("IsComplete")]
        public int _IsComplete { get; set; }

        [NotMapped]
        public bool IsComplete
        {
            get
            {
                return _IsComplete != 0;
            }
            set 
            {
                _IsComplete = value ? 1 : 0;
            }
        }   
    }
}