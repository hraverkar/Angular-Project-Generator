
namespace Angular_Project_Generator.Models.Model
{
    public class ProjectModel
    {
        public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();
        public string Title { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        public List<string> Tags { get; set; }
    }
}
