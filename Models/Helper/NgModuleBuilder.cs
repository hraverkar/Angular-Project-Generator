using Angular_Project_Generator.Models.Model;

namespace Angular_Project_Generator.Models.Helper
{
    public static class NgModuleBuilder
    {
        public static string BuildCommand(string nodeName, NodeConfiguration node, bool withRoute = true)
        {
            string command = string.Empty;

            if (withRoute)
            {
                command = CommandAndToken.CreateModuleWithRoute
                    .Replace(Tokens.NodeName, nodeName)
                    .Replace(Tokens.RouteName, node.Route)
                    .Replace(Tokens.ParentModuleName, node.ParentModule);
            }
            else
            {
                command = CommandAndToken.CreateModule
                    .Replace(Tokens.NodeName, nodeName)
                    .Replace(Tokens.ParentModuleName, node.ParentModule);
            }

            Console.WriteLine(command); // Use Console.WriteLine for logging in C#

            return command;
        }
    }
}
