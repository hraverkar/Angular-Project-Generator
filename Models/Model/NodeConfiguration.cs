using Angular_Project_Generator.Enums;

namespace Angular_Project_Generator.Models.Model
{
    public class NodeConfiguration
    {
        public string Name { get; set; }
        public string Type { get; set; } 
        public bool IsRoot { get; set; } = false;
        public string Route { get; set; }
        public string ModulePath { get; set; }
        public string ParentModule { get; set; }
        public List<NodeConfiguration> Children { get; set; }
        public NodeConfiguration()
        {
            Children = new List<NodeConfiguration>(); // Initialize Children as an empty list
        }
    }
}
