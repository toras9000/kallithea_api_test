#r "nuget: Lestaly, 0.13.0"
using Lestaly;

await Paved.RunAsync(async () =>
{
    var composeFile = ThisSource.GetRelativeFile("docker-compose.yml");
    if (!composeFile.Exists) throw new PavedMessageException("Not found compose file");

    Console.WriteLine("Stop service");
    var downProc = Process.Start("docker-compose", $"--file \"{composeFile.FullName}\" down");
    await downProc.WaitForExitAsync();
    if (downProc.ExitCode != 0) throw new PavedMessageException($"Failed to down. ExitCode={downProc.ExitCode}");

    Console.WriteLine("Start service");
    var upProc = Process.Start("docker-compose", $"--file \"{composeFile.FullName}\" up -d");
    await upProc.WaitForExitAsync();
    if (upProc.ExitCode != 0) throw new PavedMessageException($"Failed to up. ExitCode={upProc.ExitCode}");
});
