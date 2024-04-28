using Angular_Project_Generator.Models.Model;

namespace Angular_Project_Generator.Models.Helper
{
    public static class NgServiceBuilder
    {
        public static string BuildCommand(string nodeName)
        {
            string command = CommandAndToken.CreateService
                .Replace(Tokens.NodeName, nodeName);

            Console.WriteLine(command); // Use Console.WriteLine for logging in C#

            return command;
        }

    }
}
