// This script is meant to run with dotnet-script.
// You can install .NET SDK 6.0 and install dotnet-script with the following command.
// $ dotnet tool install -g dotnet-script

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

var url = new Uri("http://localhost:9999/_admin/api");
var key = "1111222233334444555566667777888899990000";

var parameters = new
{
    id = "0",
    api_key = key,
    method = "create_repo",
    args = new
    {
        repo_name = $"test_{DateTime.Now:HHmm_ss}",
        enable_downloads = true,
        enable_statistics = true,
    }
};

Console.WriteLine($"req: {JsonSerializer.Serialize(parameters)}");

var client = new HttpClient();
var response = await client.PostAsJsonAsync(url, parameters);

var rest_json = await response.Content.ReadFromJsonAsync<JsonElement>();
Console.WriteLine($"rsp: {rest_json}");
