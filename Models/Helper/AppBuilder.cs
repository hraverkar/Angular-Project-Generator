﻿
using Angular_Project_Generator.Enums;
using Angular_Project_Generator.Models.Model;
using Azure;
using Azure.Storage.Blobs;
using System.Diagnostics;
using System.IO.Compression;

namespace Angular_Project_Generator.Models.Helper
{
    public class AppBuilder
    {
        public AppConfiguration Configuration { get; set; }
        public string Pwd { get; set; }
        public NgCommandBuilder CommandBuilder { get; set; }

        public string BlobConnectionString = "DefaultEndpointsProtocol=https;AccountName=schoolblogblobst;AccountKey=UgfmCHYD+m0YbOPrOcLAWzA8RwZ+67zi2CBlScciDAG+Ik33pjydyeOf5sn/1hxD0Hmu7cnNAsGy+AStmqCJew==;EndpointSuffix=core.windows.net";
        public string BlobContainerName = "photostore";
        public AppBuilder(AppConfiguration nodeConfiguration)
        {
            Configuration = nodeConfiguration;
            CommandBuilder = new NgCommandBuilder();
        }

        public async Task<Tuple<bool, string>> GenerateProject(ProjectModel projectModel, string folderPath)
        {
            var isSuccessful = false;
            var newZipName = string.Empty;
            try
            {
                CreateApp();
                CommandBuilder.Append($"cd {Configuration.Name}");
                await GenerateApplication(this.Configuration.NodeConfiguration);
                this.CommandBuilder.Append("cd ..");
                var completeCommand = this.CommandBuilder.GetCommand();
                Console.WriteLine(completeCommand.ToString());
                await processSync(completeCommand);
                string[] files = null;
                if (projectModel != null)
                {
                    var result = GetProjectStructure(this.Configuration.Name, this.Configuration.Name, files, projectModel);
                }
                string zipFileName = this.Configuration.Name + ".zip";
                string zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), zipFileName);

                // Check if the zip file exists and delete it if it does
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);

                    Console.WriteLine($"Deleted existing zip file: {zipFilePath}");
                }
                CreateZipArchive(folderPath, zipFilePath);
                newZipName = await UploadZipToBlobStorageAsync(zipFileName, zipFilePath);
                Console.WriteLine("Zip archive created successfully.");
                isSuccessful = true;
            }
            catch (Exception ex)
            {
                isSuccessful = false;
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                this.CommandBuilder.Reset();
            }
            return Tuple.Create(isSuccessful, newZipName);
        }

        private static void CreateZipArchive(string folderPath, string zipFilepath)
        {
            ZipFile.CreateFromDirectory(folderPath, zipFilepath);
        }


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
                    case NodeType.module:
                        command = NgModuleBuilder.BuildCommand(nodeName, currentNode, false);
                        break;
                    case NodeType.moduleWithRoute:
                        command = NgModuleBuilder.BuildCommand(nodeName, currentNode);
                        break;
                    case NodeType.component:
                        command = NgComponentBuilder.BuildCommand(nodeName, currentNode);
                        break;
                    case NodeType.standaloneComponent:
                        command = NgStandaloneComponentBuilder.BuildCommand(nodeName, currentNode);
                        break;
                    case NodeType.service:
                        command = NgServiceBuilder.BuildCommand(nodeName);
                        break;
                    case NodeType.pipe:
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
        private async Task<string> UploadZipToBlobStorageAsync(string zipFileName, string zipPath)
        {
            byte[] zipFileContent = File.ReadAllBytes(zipPath);
            var blobServiceClient = new BlobServiceClient(BlobConnectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(BlobContainerName);
            await blobContainerClient.CreateIfNotExistsAsync();
            var newZipName = Guid.NewGuid() + "_" + zipFileName;
            var blobClient = blobContainerClient.GetBlobClient(newZipName);
            await using (var memoryStream = new MemoryStream(zipFileContent))
            {
                await blobClient.UploadAsync(memoryStream, true);
            }
            return newZipName;
        }
        public async Task<byte[]> GetFileFromBlob(string fileName)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(BlobConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(BlobContainerName);
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                using MemoryStream memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Failed to retrieve file '{fileName}' from Azure Blob Storage: {ex.Message}");
                return null;
            }
        }
    }
}