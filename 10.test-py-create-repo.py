# python -m pip install requests
import requests
import datetime

# Create a repository from API. for Python

url = 'http://localhost:9999//_admin/api'
api_key = '1111222233334444555566667777888899990000'

params = {
    "id": "0",
    "api_key": api_key,
    "method": "create_repo",
    "args": {
        "repo_name" : f"test_{datetime.datetime.now():%H%M_%S}", 
        "enable_downloads": True,
        "enable_statistics": True,
    }
}

print(f"req: {params}")
response = requests.post(url, json=params)
print(f"req: {response.json()}")
