// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

#r "nuget: Lestaly, 0.20.0"
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

// Create a nested repository group and rename it.

var url = new Uri("http://localhost:8888/_admin/api");
var key = "1111222233334444555566667777888899990000";

// prepare
var now = DateTime.Now;
var client = new HttpClient();
var parentName = $"parent_{now:HHmm_ss}";
await client.PostAsJsonAsync(url, new { id = "0", api_key = key, method = "create_repo_group", args = new { group_name = parentName, } });
var subName = $"sub_{now:HHmm_ss}";
await client.PostAsJsonAsync(url, new { id = "0", api_key = key, method = "create_repo_group", args = new { group_name = subName, parent = parentName, } });
Console.WriteLine($"Test repogroup: {parentName}/{subName}");

// test update_repo_group - no parent
Console.WriteLine("Test update repogroup");
var parameter = new
{
    id = "0",
    api_key = key,
    method = "update_repo_group",
    args = new
    {
        repogroupid = $"{parentName}/{subName}",
        group_name = $"{subName}-renamed",
        description = "abc",
    },
};
Console.WriteLine($"req: {JsonSerializer.Serialize(parameter)}");
var response = await client.PostAsJsonAsync(url, parameter);
Console.WriteLine($"rsp: {await response.Content.ReadFromJsonAsync<JsonElement>()}");

