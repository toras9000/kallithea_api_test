// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

#r "nuget: Lestaly, 0.20.0"
#r "nuget: KallitheaApiClient, 0.7.0.11"
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using KallitheaApiClient;
using KallitheaApiClient.Utils;

// Moving a repository group with child elements.

var url = new Uri("http://localhost:8888/_admin/api");
var key = "1111222233334444555566667777888899990000";

// prepare
var now = DateTime.Now;
var kclient = new SimpleKallitheaClient(url, key);
var grpA = await kclient.CreateRepoGroupAsync(new($"parent_{now:HHmm_ss}_A"));
var grpB = await kclient.CreateRepoGroupAsync(new($"parent_{now:HHmm_ss}_B"));
var subgrp = await kclient.CreateRepoGroupAsync(new($"sub_{now:HHmm_ss}", parent: grpA.group_name));
var subsubgrp = await kclient.CreateRepoGroupAsync(new($"subsub_{now:HHmm_ss}", parent: subgrp.group_name));
await kclient.CreateRepoAsync(new($"{subgrp.group_name}/repo_{now:HHmm_ss}_A", repo_type: RepoType.git));
await kclient.CreateRepoAsync(new($"{subgrp.group_name}/repo_{now:HHmm_ss}_B", repo_type: RepoType.hg));
await kclient.CreateRepoAsync(new($"{subsubgrp.group_name}/repo_{now:HHmm_ss}_A", repo_type: RepoType.git));
await kclient.CreateRepoAsync(new($"{subsubgrp.group_name}/repo_{now:HHmm_ss}_B", repo_type: RepoType.hg));

// test update_repo_group - change parent
Console.WriteLine("Test update repogroup");
var client = new HttpClient();
var parameter = new
{
    id = "0",
    api_key = key,
    method = "update_repo_group",
    args = new
    {
        repogroupid = subgrp.group_name,
        group_name = $"{subgrp.group_name.Split('/')[^1]}-mv",
        parent = grpB.group_id,
    },
};
Console.WriteLine($"req: {JsonSerializer.Serialize(parameter)}");
var response = await client.PostAsJsonAsync(url, parameter);
Console.WriteLine($"rsp: {await response.Content.ReadFromJsonAsync<JsonElement>()}");

