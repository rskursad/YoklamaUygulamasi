namespace yoklamaVsty
{
    public class DBPath
    {
        public const string DatabaseFilename = "DB6.db";

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);


    }
}
