using System.ComponentModel.DataAnnotations;

namespace ContainerApp.WebApp.Models
{
    public class TodoItemModel
    {
        public long Id { get; set; }
        //[Required]
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}