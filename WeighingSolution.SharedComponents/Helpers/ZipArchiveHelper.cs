using System.IO;
using System.IO.Compression;

namespace WeighingSolution.Helpers
{
    public static class ZipArchiveHelper
    {
        //More functions at https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-compress-and-extract-files
        //https://stackoverflow.com/questions/14795197/forcefully-replacing-existing-files-during-extracting-file-using-system-io-compr
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }

            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                WeighingSolution.DataLayer.Common.CreateDirectoryWithFullAccess(Path.GetDirectoryName(completeFileName));


                if (file.Name != "")
                    file.ExtractToFile(completeFileName, true);
            }
        }

        public static void ExtractFile(string ZipFilePath, string DestinationDirectory)
        {
            WeighingSolution.DataLayer.Common.CreateDirectoryWithFullAccess(DestinationDirectory);
            using (ZipArchive archive = ZipFile.Open(ZipFilePath, ZipArchiveMode.Read))
            {
                archive.ExtractToDirectory(DestinationDirectory, true);
            }
        }

        public static void CreateArchive(string SourceDirectory, string DestinationZipFilePath)
        {
            ZipFile.CreateFromDirectory(SourceDirectory, DestinationZipFilePath, CompressionLevel.Optimal, false);
        }
    }
}
