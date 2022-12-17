#r "nuget: Lestaly, 0.19.0"

// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

var url = new Uri("http://localhost:9999/_admin/api");
var key = "1111222233334444555566667777888899990000";

// prepare
var now = DateTime.Now;
var client = new HttpClient();
var parentNameA = $"parent_{now:HHmm_ss}_A";
await client.PostAsJsonAsync(url, new { id = "0", api_key = key, method = "create_repo_group", args = new { group_name = parentNameA, } });
var parentNameB = $"parent_{now:HHmm_ss}_B";
await client.PostAsJsonAsync(url, new { id = "0", api_key = key, method = "create_repo_group", args = new { group_name = parentNameB, } });
var subName = $"sub_{now:HHmm_ss}";
await client.PostAsJsonAsync(url, new { id = "0", api_key = key, method = "create_repo_group", args = new { group_name = subName, parent = parentNameA, } });
Console.WriteLine($"Test repogroup: {parentNameA}/{subName}");

// test update_repo_group - change parent
Console.WriteLine("Test update repogroup");
var parameter = new { id = "0", api_key = key, method = "update_repo_group", args = new { repogroupid = $"{parentNameA}/{subName}", parent = parentNameB, } };
Console.WriteLine($"req: {JsonSerializer.Serialize(parameter)}");
var response = await client.PostAsJsonAsync(url, parameter);
Console.WriteLine($"rsp: {await response.Content.ReadFromJsonAsync<JsonElement>()}");

