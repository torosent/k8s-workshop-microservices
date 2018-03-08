# k8s-workshop-microservices

## Architecture

![image](https://user-images.githubusercontent.com/17064840/35954452-b18fbdbe-0cc4-11e8-952d-cb5d3aee2341.png)

## Challenge

1. Build the containers. Example: `docker build -t tomerplayground.azurecr.io/webstore:latest .`
2. Provision Azure Container Registry [ACR](https://docs.microsoft.com/en-us/azure/aks/tutorial-kubernetes-prepare-acr) & [Azure Storage](https://docs.microsoft.com/en-us/cli/azure/storage/account?view=azure-cli-latest#az_storage_account_create)
3. Setup ACR in the K8S cluster 
```kubectl create secret docker-registry acr-auth --docker-server <acr-login-server> --docker-username <service-principal-ID> --docker-password <service-principal-password> --docker-email <email-address>```
4. Push the containers into ACR
5. Deploy the micro-services application in K8S
