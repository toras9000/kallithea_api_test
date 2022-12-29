// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

#r "nuget: Lestaly, 0.20.0"
using Lestaly;

// Restart the docker container with the data intact.
// (If it is not activated, it is simply activated.)

return await Paved.RunAsync(optionsBuilder: o => { if (Args.Contains("--nointeract")) { o.NoPause(); } }, action: async () =>
{
    var composeFile = ThisSource.GetRelativeFile("docker-compose.yml");
    if (!composeFile.Exists) throw new PavedMessageException("Not found compose file");

    var redirect = true;
    var outWriter = redirect ? Console.Out : null;
    var errWriter = redirect ? Console.Error : null;

    ConsoleWig.WriteLineColord(ConsoleColor.Blue, "Stop service");
    var downResult = await CmdProc.ExecAsync("docker-compose", new[] { "--file", composeFile.FullName, "down", }, stdOutWriter: outWriter, stdErrWriter: errWriter);
    if (downResult != 0) throw new PavedMessageException($"Failed to down. ExitCode={downResult}");

    ConsoleWig.WriteLineColord(ConsoleColor.Blue, "Start service");
    var upResult = await CmdProc.ExecAsync("docker-compose", new[] { "--file", composeFile.FullName, "up", "-d", }, stdOutWriter: outWriter, stdErrWriter: errWriter);
    if (upResult != 0) throw new PavedMessageException($"Failed to up. ExitCode={upResult}");
});
