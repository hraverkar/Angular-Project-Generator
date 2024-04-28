namespace Angular_Project_Generator.Models.Model
{
    public class NgCommandBuilder
    {
        private List<string> AllCommands = new List<string>();
        public void Append(string command)
        {
            AllCommands.Add(command);
        }
        public string GetCommand()
        {
            return string.Join(" && ", AllCommands);
        }
        public void Reset()
        {
            AllCommands.Clear();
        }
    }
}
