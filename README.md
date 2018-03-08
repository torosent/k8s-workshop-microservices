# k8s-workshop-microservices

## Architecture

![image](https://user-images.githubusercontent.com/17064840/35954452-b18fbdbe-0cc4-11e8-952d-cb5d3aee2341.png)

## Challenges

### Simple deployment

1. Build the containers. Example: 
```sh
docker build -t tomerplayground.azurecr.io/webstore:latest .
```
2. Provision [Azure Container Registry](https://docs.microsoft.com/en-us/azure/aks/tutorial-kubernetes-prepare-acr) & [Azure Storage](https://docs.microsoft.com/en-us/cli/azure/storage/account?view=azure-cli-latest#az_storage_account_create)
3. Setup ACR in the K8S cluster 
```sh
kubectl create secret docker-registry acr-auth --docker-server <acr-login-server> --docker-username <service-principal-ID> --docker-password <service-principal-password> --docker-email <email-address>
```
4. Push the containers into ACR
5. Get Azure storage [connection string](https://docs.microsoft.com/en-us/cli/azure/storage/account?view=azure-cli-latest#az_storage_account_show_connection_string)
6. Insert the connection string in [secret.yaml](/Deployment/YAML/secret.yaml)
##### Note
To prepare secret, we need to encode it to Base64
`echo -n "admin" | base64 ` or use `https://www.base64encode.org/`

7. Edit the relevant [YAML](/Deployment/YAML) files and deploy the micro-services application in K8S.
```sh
kubectl create -f . --save-config
```

#### Updating a service

8. Update [printer.cs](PrintingService/printer.cs#L61) Line 61 with a new version.
9. Build and push the container 
```sh
 docker build -t tomerplayground.azurecr.io/printingservice:latest .
 docker push tomerplayground.azurecr.io/printingservice:latest
```
10. Apply the [printing.yaml](/Deployment/YAML/printing.yaml) file.
```sh
kubectl apply -f printing.yaml
```

### Helm

##### Note
Remove the previous webstore deployment
```sh
kubectl delete deployment webstore-deployment
```

1. Create Helm chart for Webstore. [Example](/Deployment/Helm)
```sh
helm create stickerstore
```
2. Install Stickerstore chart
```sh
helm upgrade --install stickerstore ./stickerstore
```
3. Change a value like replicaCount and upgrade 
```sh
helm upgrade --install stickerstore ./stickerstore
--set replicaCount=3
```

### Brigade
1. Deploy [Brigade](https://github.com/Azure/brigade#quickstart)
2. Create a Github token [Here](https://help.github.com/articles/creating-a-personal-access-token-for-the-command-line/)
3. Create a Github webhook and connect it with brigade. [Instructions](https://github.com/Azure/brigade/blob/master/docs/topics/github.md)
4. Edit [values.yaml](/Deployment/Brigade/values.yaml) and install the brigade project 
```sh
helm install brigade/brigade-project -n stickerstore-project -f values.yaml
```

5. Commit an edit to `index.html` in Webstore and watch Brigade
```sh
kubectl get pods
kubectl logs <pod-name>
```

### Jenkins

Follow the [Jenkins](/Deployment/Jenkins/Jenkins.MD) readme file instructions
