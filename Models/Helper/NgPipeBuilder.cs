using Angular_Project_Generator.Models.Model;

namespace Angular_Project_Generator.Models.Helper
{
    public static class NgPipeBuilder
    {
        public static string BuildCommand(string nodeName, NodeConfiguration node)
        {
            string command = CommandAndToken.CreatePipe
                .Replace(Tokens.NodeName, nodeName)
                .Replace(Tokens.ParentModuleName, node.ParentModule);

            Console.WriteLine(command); // Use Console.WriteLine for logging in C#

            return command;
        }
    }
}
