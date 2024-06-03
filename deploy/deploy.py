# script to deploy the app using command line arguments
# wrap in authentication aws-vault exec <profile> -- python deploy.py --env ...
# see password manager for the configuration values

import argparse
import os
import pathlib
import re
import shutil
import subprocess

def main():
    parser = argparse.ArgumentParser(description='Deploy the hello app')
    parser.add_argument('--cluster-name', type=str, help='EKS cluster name', required=True)
    parser.add_argument('--env', type=str, choices=['stage', 'production'], required=True)
    parser.add_argument('--image', type=str, help='Docker image to deploy, with :version', required=True)
    parser.add_argument('--authority-url', type=str, required=True)
    parser.add_argument('--redirect-url', type=str, required=True)
    parser.add_argument('--api-endpoint', type=str, required=True)
    parser.add_argument('--target-group-arn', type=str, required=True)
    args = parser.parse_args()

    print(f"Deploying the app to {args.env} environment")

    print(f"Verifying access to the cluster {args.cluster_name}...")
    subprocess.check_call(["kubectl", "config", "use-context", args.cluster_name])

    temp_folder = pathlib.Path("_" + args.env)

    # copy all files to temp dir for replacements
    if os.path.exists(temp_folder):
        shutil.rmtree(temp_folder)
    shutil.copytree('k8s', temp_folder)

    # replace the variables in yaml files
    for f in os.listdir(temp_folder):
        if not f.endswith('.yml'):
            continue

        file_path = temp_folder / f

        with open(file_path, 'r') as file:
            filedata = file.read()
        filedata = filedata.replace('__deploymentEnvironment__', args.env)
        filedata = filedata.replace('__AUTHORITY_URL__', args.authority_url)
        filedata = filedata.replace('__REDIRECT_URL__', args.redirect_url)
        filedata = filedata.replace('__API_ENDPOINT__', args.api_endpoint)
        filedata = filedata.replace('__DOCKER_IMAGE__', args.image)
        filedata = filedata.replace('__TARGET_GROUP_ARN__', args.target_group_arn)

        # use a regex to find un-replaced variables
        results = re.findall(r'\b__\w+__\b', filedata)
        if results:
            raise Exception("Found '__' did a variable not get replaced: " + str(results))

        with open(file_path, 'w') as file:
            file.write(filedata)

    print("Replaced all variables in the yaml files, deploying...")
    for f in os.listdir(temp_folder):
        if not f.endswith('.yml'):
            continue
        file_path = temp_folder / f

        subprocess.check_call(["kubectl", "apply", "-f", file_path])

    # clean up
    shutil.rmtree(temp_folder)

    print("Deployment complete!")

if __name__ == "__main__":
    main()
