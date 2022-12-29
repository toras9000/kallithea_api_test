// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

#r "nuget: Lestaly, 0.19.0"
#r "nuget: KallitheaApiClient, 0.7.0.10"
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using KallitheaApiClient;
using KallitheaApiClient.Utils;

// Create a nested repository group and rename and move it. Use lib.

var url = new Uri("http://localhost:8888/_admin/api");
var key = "1111222233334444555566667777888899990000";

// prepare
var now = DateTime.Now;
var kclient = new ShuckedKallitheaClient(url, key);
var grpA = await kclient.CreateRepoGroupAsync(new($"parent_{now:HHmm_ss}_A"));
var grpB = await kclient.CreateRepoGroupAsync(new($"parent_{now:HHmm_ss}_B"));
var subgrp = await kclient.CreateRepoGroupAsync(new($"sub_{now:HHmm_ss}", parent: grpA.group_name));
Console.WriteLine($"Test repogroup: {subgrp.group_name}");

// test update_repo_group - change parent
Console.WriteLine("Test update repogroup");
var args = new { repogroupid = subgrp.group_name, group_name = $"{subgrp.group_name.Split('/')[^1]}-ren", parent = grpB.group_id, };
var parameter = new { id = "0", api_key = key, method = "update_repo_group", args = args, };
Console.WriteLine($"req: {JsonSerializer.Serialize(parameter)}");
var client = new HttpClient();
var response = await client.PostAsJsonAsync(url, parameter);
Console.WriteLine($"rsp: {await response.Content.ReadFromJsonAsync<JsonElement>()}");

