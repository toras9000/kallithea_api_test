// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

#r "nuget: System.Data.SQLite.Core, 1.0.117"
#r "nuget: Dapper, 2.0.123"
#r "nuget: Lestaly, 0.20.0"
using System.Data.SQLite;
using Dapper;
using Lestaly;

// In kallithea's SQLite database, rewrite admin's API key to a fixed key for debugging.

return await Paved.RunAsync(optionsBuilder: o => { var _ = Args.Contains("--nointeract") ? o.NoPause() : o.AnyPause(); }, action: async () =>
{
    // API key to set up.
    var apiKey = "1111222233334444555566667777888899990000";

    // check db file.
    var dbFile = ThisSource.GetRelativeFile("./config/kallithea.db");
    if (!dbFile.Exists) throw new PavedMessageException("db file not found.");

    // Force update of admin's API key. 
    Console.WriteLine("Rewrite the API key for test.");
    var db_settings = new SQLiteConnectionStringBuilder();
    db_settings.DataSource = dbFile.FullName;
    db_settings.FailIfMissing = true;
    using var db = new SQLiteConnection(db_settings.ConnectionString);
    await db.ExecuteAsync("update users set api_key = @key where username = 'admin'", new { key = apiKey, });

});
