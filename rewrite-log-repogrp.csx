#r "nuget: Lestaly, 0.13.0"

// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

using System.Text.RegularExpressions;
using Lestaly;

await Paved.RunAsync(async () =>
{
    // check ini file.
    var iniFile = ThisSource.GetRelativeFile("./config/kallithea.ini");
    if (!iniFile.Exists) throw new PavedMessageException("ini file not found.");

    // The pattern to rewrite.
    var logKeysPattern = new Regex(@"^\s*keys\s*=\s*root\s*,\s*routes\s*,\s*kallithea(_repogrp)?");
    var logQualPattern = new Regex(@"^\s*\[logger_kallithea(_repogrp)?\]\s*$");

    // Read the ini file.
    var orgLines = await iniFile.ReadAllLinesAsync();

    // Rewrite the contents of the file.
    var newLines = new List<string>(orgLines.Length + 30);
    var cancel = false;
    foreach (var line in orgLines)
    {
        var logKeysMatch = logKeysPattern.Match(line);
        if (logKeysMatch.Success)
        {
            if (logKeysMatch.Groups[1].Value.IsNotWhite())
            {
                cancel = true;
                break;
            }
            var keys = string.Concat(
                line.AsSpan(0, logKeysMatch.Length),
                "_repogrp, kallithea",
                line.AsSpan(logKeysMatch.Length)
            );
            newLines.Add(keys);
            continue;
        }

        var logQualMatch = logQualPattern.Match(line);
        if (logQualMatch.Success)
        {
            if (logQualMatch.Groups[1].Value.IsNotWhite())
            {
                cancel = true;
                break;
            }
            newLines.Add("[logger_kallithea_repogrp]");
            newLines.Add("level = DEBUG");
            newLines.Add("handlers =");
            newLines.Add("qualname = kallithea.model.repo_group");
            newLines.Add("");
            newLines.Add(line);
            continue;
        }

        newLines.Add(line);
    }

    // If not canceled, save the rewrited contents.
    if (cancel)
    {
        Console.WriteLine("Already rewrited.");
    }
    else
    {
        await iniFile.WriteAllLinesAsync(newLines);
        Console.WriteLine("Rewrite ini.");
    }


}, o => o.AnyPause());
