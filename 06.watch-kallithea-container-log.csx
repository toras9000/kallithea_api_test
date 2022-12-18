// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

#r "nuget: Lestaly, 0.19.0"
using System.Threading;
using Lestaly;

// Display the log of the app container.

return await Paved.RunAsync(async () =>
{
    var composeFile = ThisSource.GetRelativeFile("docker-compose.yml");
    if (!composeFile.Exists) throw new PavedMessageException("Not found compose file");
    await CmdProc.ExecAsync("docker-compose", new[] { "--file", composeFile.FullName, "logs", "--follow", "--no-log-prefix", "app", }, stdOutWriter: Console.Out, stdErrWriter: Console.Error);
});
