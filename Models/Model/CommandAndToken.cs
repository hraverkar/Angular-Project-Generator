namespace Angular_Project_Generator.Models.Model
{
    public static class Tokens
    {
        public static string AppName { get; } = "$appName$";
        public static string NodeName { get; } = "$moduleName$";
        public static string RouteName { get; } = "$routeName$";
        public static string ParentModuleName { get; } = "$parentModuleName$";
    }

    public static class CommandAndToken
    {
        // Use string interpolation to insert token values
        public static string CreateApp { get; } = $"ng new {Tokens.AppName} --standalone false --force --skip-install --routing --defaults";
        public static string CreateModule { get; } = $"ng generate module {Tokens.NodeName} --module {Tokens.ParentModuleName}.module";
        public static string CreateModuleWithRoute { get; } = $"ng generate module {Tokens.NodeName} --route {Tokens.RouteName} --module {Tokens.ParentModuleName}.module";
        public static string CreateComponent { get; } = $"ng generate component {Tokens.NodeName} --module {Tokens.ParentModuleName}.module";
        public static string CreateStandaloneComponent { get; } = $"ng generate component {Tokens.NodeName} --standalone";
        public static string CreatePipe { get; } = $"ng generate pipe {Tokens.NodeName} --module {Tokens.ParentModuleName}.module";
        public static string CreateService { get; } = $"ng generate service {Tokens.NodeName}";
    }
}
