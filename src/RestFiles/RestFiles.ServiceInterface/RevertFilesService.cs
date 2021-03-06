using System.IO;
using System.Linq;
using RestFiles.ServiceModel;
using ServiceStack;

namespace RestFiles.ServiceInterface
{
    /// <summary>
    /// Define your ServiceStack web service request (i.e. Request DTO).
    /// </summary>
    public class RevertFilesService : Service
    {
        /// <summary>
        /// Gets or sets the AppConfig. The built-in IoC used with ServiceStack autowires this property.
        /// </summary>
        public AppConfig Config { get; set; }

        public object Post(RevertFiles request)
        {
            var rootDir = Config.RootDirectory;

            if (Directory.Exists(rootDir))
            {
                Directory.Delete(rootDir, true);
            }

            CopyFiles(rootDir, "~/App_Data/src".MapHostAbsolutePath(), ".cs", ".htm", ".md");

            var servicesDir = Path.Combine(rootDir, "services");
            CopyFiles(servicesDir, "~/App_Data/src/RestFiles.ServiceInterface/".MapHostAbsolutePath(), "Service.cs");

            var testsDir = Path.Combine(rootDir, "tests");
            CopyFiles(testsDir, "~/App_Data/src/RestFiles.Tests/".MapHostAbsolutePath(), ".cs");

            var dtosDir = Path.Combine(rootDir, "dtos");

            var opsDtoPath = dtosDir;
            CopyFiles(opsDtoPath, "~/App_Data/src/RestFiles.ServiceModel/".MapHostAbsolutePath());

            var typesDtoPath = Path.Combine(dtosDir, "Types");
            CopyFiles(typesDtoPath, "~/App_Data/src/RestFiles.ServiceModel/Types/".MapHostAbsolutePath());

            return new RevertFilesResponse();
        }

        private static void CopyFiles(string path, string filesPath, params string[] excludedFiles)
        {
            Directory.CreateDirectory(path);
            var files = Directory.GetFiles(filesPath);
            foreach (var file in files)
            {
                if (excludedFiles.IsEmpty() || excludedFiles.Any(x => file.EndsWith(x)))
                {
                    var fileName = Path.GetFileName(file);
                    if (file.EndsWith(".cs"))
                    {
                        fileName += ".txt";
                    }
                    File.Copy(file, Path.Combine(path, fileName));
                }
            }
        }
    }
}