namespace Angular_Project_Generator.Models.Model
{
    public class ProjectModel
    {
        public Dictionary<string, object> Files { get; set; } = new Dictionary<string, object>();
        public string Title { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        public List<string> Tags { get; set; }
    }
}
