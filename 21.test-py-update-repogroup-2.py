# python -m pip install requests
import requests
import datetime

# Create and move nested repository groups. for Python

url = 'http://localhost:8888//_admin/api'
api_key = '1111222233334444555566667777888899990000'

# prepare
now = datetime.datetime.now()
parentNameA = f"parent_{now:%H%M_%S}_A"
parentGrpA = requests.post(url, json={ "id": "0", "api_key": api_key, "method": "create_repo_group", "args": { "group_name": parentNameA } }).json()
parentNameB = f"parent_{now:%H%M_%S}_B"
parentGrpB = requests.post(url, json={ "id": "0", "api_key": api_key, "method": "create_repo_group", "args": { "group_name": parentNameB } }).json()
subName = f"sub_{now:%H%M_%S}"
subGrp = requests.post(url, json={ "id": "0", "api_key": api_key, "method": "create_repo_group", "args": { "group_name": subName, "parent": parentNameA } }).json()

# test update_repo_group - change parent

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
response = requests.post(url, json={ "id": "0", "api_key": api_key, "method": "update_repo_group", "args": { "repogroupid": f"{parentNameA}/{subName}", "parent": parentNameB } }).json()
print(f"req: {response}")
