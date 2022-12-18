// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

#r "nuget: Lestaly, 0.19.0"
using System.Threading;
using Lestaly;

// Restart docker container with deletion of persistent data.
// (If it is not activated, it is simply activated.)

return await Paved.RunAsync(optionsBuilder: o => { if (Args.Contains("--nointeract")) { o.NoPause(); } }, action: async () =>
{
    var composeFile = ThisSource.GetRelativeFile("docker-compose.yml");
    if (!composeFile.Exists) throw new PavedMessageException("Not found compose file");

    var redirect = false;
    var outWriter = redirect ? Console.Out : null;
    var errWriter = redirect ? Console.Error : null;

    Console.WriteLine("Stop service");
    var downResult = await CmdProc.ExecAsync("docker-compose", new[] { "--file", composeFile.FullName, "down", }, stdOutWriter: outWriter, stdErrWriter: errWriter);
    if (downResult != 0) throw new PavedMessageException($"Failed to down. ExitCode={downResult}");

    Console.WriteLine("Delete config/repos");
    var confDir = ThisSource.GetRelativeDirectory("config");
    var reposDir = ThisSource.GetRelativeDirectory("repos");
    if (confDir.Exists) { confDir.DoFiles(c => c.File?.SetReadOnly(false)); confDir.Delete(recursive: true); }
    if (reposDir.Exists) { reposDir.DoFiles(c => c.File?.SetReadOnly(false)); reposDir.Delete(recursive: true); }

    Console.WriteLine("Start service");
    var upResult = await CmdProc.ExecAsync("docker-compose", new[] { "--file", composeFile.FullName, "up", "-d", }, stdOutWriter: outWriter, stdErrWriter: errWriter);
    if (upResult != 0) throw new PavedMessageException($"Failed to up. ExitCode={upResult}");

    Console.Write("Waiting initialize ... ");
    using var timer = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    var iniFile = ThisSource.GetRelativeFile("config/kallithea.ini");
    do
    {
        await Task.Delay(500, timer.Token);
        iniFile.Refresh();
    } while (!iniFile.Exists);
    Console.WriteLine("completed.");
});
