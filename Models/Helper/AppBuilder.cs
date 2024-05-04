using Angular_Project_Generator.Models.Model;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO.Compression;

namespace Angular_Project_Generator.Models.Helper
{
    public class AppBuilder
    {
        public AppConfiguration Configuration { get; set; }
        public NgCommandBuilder CommandBuilder { get; set; }

        public string BlobConnectionString = "DefaultEndpointsProtocol=https;AccountName=schoolblogblobst;AccountKey=UgfmCHYD+m0YbOPrOcLAWzA8RwZ+67zi2CBlScciDAG+Ik33pjydyeOf5sn/1hxD0Hmu7cnNAsGy+AStmqCJew==;EndpointSuffix=core.windows.net";
        public string BlobContainerName = "photostore";
        public AppBuilder(AppConfiguration nodeConfiguration)
        {
            Configuration = nodeConfiguration;
            CommandBuilder = new NgCommandBuilder();
        }

        public async Task<Tuple<bool, FileContentResult>> GenerateProject(ProjectModel projectModel, string folderPath)
        {
            var isSuccessful = false;
            var zipFileName = string.Empty;
            var zipFilePath = string.Empty;
            try
            {
                CreateApp();
                CommandBuilder.Append($"cd {Configuration.Name}");
                await GenerateApplication(this.Configuration.NodeConfiguration);
                CommandBuilder.Append("cd ..");
                var completeCommand = CommandBuilder.GetCommand();
                Console.WriteLine(completeCommand.ToString());
                await processSync(completeCommand);

                if (projectModel != null)
                {
                    GetProjectStructure(this.Configuration.Name, this.Configuration.Name, null, projectModel);
                }

                zipFileName = this.Configuration.Name + ".zip";
                zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), zipFileName);

                // Check if the zip file exists and delete it if it does
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                    Console.WriteLine($"Deleted existing zip file: {zipFilePath}");
                }

                var result = await CreateZipArchive(folderPath, zipFileName);
                Console.WriteLine("Zip archive created successfully.");
                isSuccessful = true;
                return Tuple.Create(isSuccessful, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                CommandBuilder.Reset();
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }
            }
            return null;
        }

        private static async Task<FileContentResult> CreateZipArchive(string folderPath, string zipFileName)
        {
            try
            {
                // Create the archive in memory
                using (var memoryStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        // Add files asynchronously
                        await AddFilesToZipArchiveAsync(folderPath, zipArchive);
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return new FileContentResult(memoryStream.ToArray(), "application/zip") { FileDownloadName = zipFileName };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        private static async Task AddFilesToZipArchiveAsync(string folderPath, ZipArchive archive)
        {
            foreach (var file in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
            {
                var entry = archive.CreateEntry(Path.GetRelativePath(folderPath, file));
                using (var fileStream = File.OpenRead(file))
                using (var entryStream = entry.Open())
                {
                    await fileStream.CopyToAsync(entryStream);
                }
            }
        }

        //private static async Task<FileContentResult> CreateZipArchive(string folderPath, string zipFilepath, string zipFileName)
        //{
        //    try
        //    {
        //        ZipFile.CreateFromDirectory(folderPath, zipFilepath);
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            // Create a new zip archive
        //            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //            {
        //                var fileInfo = new FileInfo(zipFilepath);
        //                // Create a new entry in the zip archive for each file
        //                var entry = zipArchive.CreateEntry(fileInfo.Name);

        //                // Write the file contents into the entry
        //                using (var entryStream = entry.Open())
        //                using (var fileStream = new FileStream(zipFilepath, FileMode.Open, FileAccess.Read))
        //                {
        //                    fileStream.CopyTo(entryStream);
        //                }
        //            }
        //            memoryStream.Seek(0, SeekOrigin.Begin);
        //            return new FileContentResult(memoryStream.ToArray(), "application/zip") { FileDownloadName = zipFileName };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return null;
        //}


        public string[] GetProjectStructure(string projectName, string dirPath, string[] arrayOfFiles, ProjectModel projectModel)
        {
            arrayOfFiles = arrayOfFiles ?? new string[0];

            string[] files = Directory.GetFiles(dirPath);
            string[] directories = Directory.GetDirectories(dirPath);

            List<string> fileList = new List<string>(arrayOfFiles);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string filePath = Path.Combine(dirPath, fileName);
                fileList.Add(filePath);

                string fileNameKey = filePath.Replace(projectName + "/", "");
                string fileContent = File.ReadAllText(filePath);
                projectModel.Files[fileNameKey] = fileContent;

                fileList.Add(Path.Combine(dirPath.Replace(projectName + "/", ""), fileName));
            }

            foreach (string directory in directories)
            {
                fileList.AddRange(GetProjectStructure(projectName, directory, arrayOfFiles, projectModel));
            }

            return fileList.ToArray();
        }

        private async Task processSync(string completeCommand)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe", // Specify the command interpreter
                    Arguments = $"/c {completeCommand}", // Pass the command as an argument
                    RedirectStandardOutput = true, // Redirect standard output
                    UseShellExecute = false // Don't use the shell to execute the command
                };

                Process process = new Process
                {
                    StartInfo = startInfo
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private async Task GenerateApplication(NodeConfiguration nodeConfiguration)
        {
            foreach (var currentNode in nodeConfiguration.Children)
            {
                string command = string.Empty;
                string nodeName = string.Empty;

                if (!string.IsNullOrEmpty(currentNode.ModulePath))
                {
                    // nodeName = currentNode.ModulePath + "/" + currentNode.Name;
                    nodeName = currentNode.ModulePath;
                }
                else
                {
                    nodeName = currentNode.Name;
                }

                switch (currentNode.Type)
                {
                    case "modele":
                        command = NgModuleBuilder.BuildCommand(nodeName, currentNode, false);
                        break;
                    case "moduleWithRoute":
                        command = NgModuleBuilder.BuildCommand(nodeName, currentNode);
                        break;
                    case "component":
                        command = NgComponentBuilder.BuildCommand(nodeName, currentNode);
                        break;
                    case "standaloneComponent":
                        command = NgStandaloneComponentBuilder.BuildCommand(nodeName, currentNode);
                        break;
                    case "service":
                        command = NgServiceBuilder.BuildCommand(nodeName);
                        break;
                    case "pipe":
                        command = NgPipeBuilder.BuildCommand(nodeName, currentNode);
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(command))
                {
                    this.CommandBuilder.Append(command);
                }
                if (currentNode.Children.Count > 0)
                {
                    await GenerateApplication(currentNode);
                }

            }
        }

        private void CreateApp()
        {
            var command = CommandAndToken.CreateApp.Replace(Tokens.AppName, Configuration.Name);
            CommandBuilder.Append(command);
        }
    }
}