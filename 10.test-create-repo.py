# python -m pip install requests
import requests
import datetime

# Create a repository from API. for Python

url = 'http://localhost:8888//_admin/api'
api_key = '1111222233334444555566667777888899990000'

parameter = {
    "id": "0",
    "api_key": api_key,
    "method": "create_repo",
    "args": {
        "repo_name" : f"test_{datetime.datetime.now():%H%M_%S}", 
        "enable_downloads": True,
        "enable_statistics": True,
    },
}
print(f"req: {parameter}")
response = requests.post(url, json=parameter)
print(f"req: {response.json()}")
