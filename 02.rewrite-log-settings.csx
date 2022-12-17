#r "nuget: Lestaly, 0.19.0"

// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

using System.Text.RegularExpressions;
using Lestaly;

var settings = new
{
    IniPath = "./config/kallithea.ini",

    ReplaceBegin = new Regex(@"^##\s+LOGGING CONFIGURATION\s+##\s*$"),

    ReplaceEnd = new Regex(@"^##\s+HANDLERS\s+##$"),

    LogSettings = """
    ##   LOGGING CONFIGURATION    ##
    ################################

    [loggers]
    keys = root, routes, kallithea, kallithea_repogrp, sqlalchemy, tg, gearbox, beaker, templates, whoosh_indexer, werkzeug, backlash

    [handlers]
    keys = console, console_color, console_color_sql, null

    [formatters]
    keys = generic, color_formatter, color_formatter_sql

    #############
    ## LOGGERS ##
    #############

    [logger_root]
    level = NOTSET
    handlers = console
    ## For coloring based on log level:
    #handlers = console_color

    [logger_routes]
    level = WARN
    handlers =
    qualname = routes.middleware
    ## "level = DEBUG" logs the route matched and routing variables.

    [logger_beaker]
    level = WARN
    handlers =
    qualname = beaker.container

    [logger_templates]
    level = WARN
    handlers =
    qualname = pylons.templating

    [logger_kallithea]
    level = WARN
    handlers =
    qualname = kallithea

    [logger_kallithea_repogrp]
    level = DEBUG
    handlers =
    qualname = kallithea.model.repo_group

    [logger_tg]
    level = WARN
    handlers =
    qualname = tg

    [logger_gearbox]
    level = WARN
    handlers =
    qualname = gearbox

    [logger_sqlalchemy]
    level = WARN
    handlers =
    qualname = sqlalchemy.engine
    ## For coloring based on log level and pretty printing of SQL:
    #level = INFO
    #handlers = console_color_sql
    #propagate = 0

    [logger_whoosh_indexer]
    level = WARN
    handlers =
    qualname = whoosh_indexer

    [logger_werkzeug]
    level = WARN
    handlers =
    qualname = werkzeug

    [logger_backlash]
    level = WARN
    handlers =
    qualname = backlash

    ##############
    ## HANDLERS ##
    """,
};

await Paved.RunAsync(async () =>
{
    // check ini file.
    var iniFile = ThisSource.GetRelativeFile(settings.IniPath);
    if (!iniFile.Exists) throw new PavedMessageException("ini file not found.");

    // Read the ini file.
    var orgLines = await iniFile.ReadAllLinesAsync();

    // Rewrite the contents of the file.
    var newLines = new List<string>(orgLines.Length + 30);
    var range = false;
    var rewrite = false;
    foreach (var line in orgLines)
    {
        if (range)
        {
            if (settings.ReplaceEnd.Match(line) is var end && end.Success)
            {
                newLines.Add(settings.LogSettings);
                rewrite = true;
                range = false;
            }
        }
        else
        {
            if (settings.ReplaceBegin.Match(line) is var begin && begin.Success)
            {
                range = true;
            }
            else
            {
                newLines.Add(line);
            }
        }
    }

    // If not canceled, save the rewrited contents.
    if (rewrite)
    {
        await iniFile.WriteAllLinesAsync(newLines);
        Console.WriteLine("Rewrite ini.");
    }
    else
    {
        Console.WriteLine("Not rewrite.");
    }

}, o => o.AnyPause());
