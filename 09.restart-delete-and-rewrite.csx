#r "nuget: Lestaly, 0.19.0"
using System.Threading;
using Lestaly;

return await Paved.RunAsync(async () =>
{
    Console.WriteLine("Restart and delete config/repos");
    await CmdProc.CallAsync("dotnet-script", new[] { CurrentDir.GetRelativeFile("00.restart-delete-all.csx").FullName, "--", "--nointeract", });

    Console.WriteLine("Rewrite debug aky-key");
    await CmdProc.CallAsync("dotnet-script", new[] { CurrentDir.GetRelativeFile("01.rewrite-api-key-for-debug.csx").FullName, "--", "--nointeract", });

    Console.WriteLine("Rewrite log settings");
    await CmdProc.CallAsync("dotnet-script", new[] { CurrentDir.GetRelativeFile("02.rewrite-log-settings.csx").FullName, "--", "--nointeract", });

    Console.WriteLine("Restart kallithea");
    await CmdProc.CallAsync("dotnet-script", new[] { CurrentDir.GetRelativeFile("00.restart.csx").FullName, "--", "--nointeract", });
});
