# HelloPropertyMe.NET

Sample .NET app to demonstrate connecting to PropertyMe with OAuth 2.0

## Deployment

To deploy run the below scripts:

```bash
    cd deploy
    python ./deploy.py \
        --cluster-name "<EKS_CLUSTER_NAME>" \
        --env "<ENV>" \
        --image "<IMAGE_NAME>:<TAG>" \
        --authority-url "<AUTHORITY_URL>" \
        --redirect-url "<REDIRECT_URL>" \
        --api-endpoint "<API_ENDPOINT>" \
        --target-group-arn "<TARGET_GROUP_ARN>"
```
