import sys
import json
import os

def main():
    if len(sys.argv) < 2:
        print("Usage: python loader.py <data_path>", file=sys.stderr)
        sys.exit(1)

    data_path = sys.argv[1]

    # In a real implementation, we would:
    # 1. Parse the data at data_path (folder of extracted MSD dataset)
    # 2. Iterate over floor plans
    # 3. Construct the list of SwissDwellingLayout objects
    # 4. Serialize to JSON

    # For this implementation (stub), we will check if the path exists and return an empty list
    # or simple mock data to prove connectivity.

    if not os.path.exists(data_path):
        print(f"Error: Data path {data_path} does not exist", file=sys.stderr)
        sys.exit(1)

    # Mock output for now to satisfy the loader signature
    result = []

    # Check if we have any files to "process" (just to simulate work)
    # real logic would go here

    print(json.dumps(result))

if __name__ == "__main__":
    main()
