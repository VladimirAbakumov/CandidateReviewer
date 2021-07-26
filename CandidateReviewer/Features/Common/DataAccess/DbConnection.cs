using System.IO;
using SQLite;
using Xamarin.Essentials;

namespace VA.Candidate.Reviewer.Features.Common.DataAccess
{
  public interface IDbConnection
  {
    SQLiteConnection Connection { get; }
    void Initialize();
  }

  public class DbConnection : IDbConnection
  {
    private const string DbFileName = "localDb.db3";
    private const string DbFolderName = "db";

    public SQLiteConnection Connection { get; private set; } = null!;

    public void Initialize()
    {
      var dbPath = InitializeDbFile(DbFileName);
      Connection = new SQLiteConnection(dbPath,
        SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.PrivateCache, true);
      
      Connection.CreateTable(typeof(CandidateStorageModel));
      Connection.CreateTable(typeof(ExperienceStorageModel));
      Connection.CreateTable(typeof(TechnologyStorageModel));
      Connection.CreateTable(typeof(ApprovalStorageModel));
    }

    private string InitializeDbFile(string fileName)
    {
      var dbPath = GetDbFilePath(fileName);

      if (!Directory.Exists(Path.GetDirectoryName(dbPath)))
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

      if (!File.Exists(dbPath))
        File.Create(dbPath).Flush();

      return dbPath;
    }

    private string GetDbFilePath(string fileName)
    {
      var rootFolder = Path.Combine(FileSystem.AppDataDirectory, DbFolderName);
      return Path.Combine(rootFolder, fileName);
    }
  }
}