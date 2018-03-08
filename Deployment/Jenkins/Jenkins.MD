# K8S, Jenkins and StickerStore app deployment

## The purpose of this challenge is to build and deploy a application to a Kubernetes cluster

In this challenge we will:
1.	Install a Jenkins from Helm Chart
2.	Build the images from GitHub repo.
3.	Push the images to Docker Hub.
4.	Execute YAML deployment files in the K8S cluster from the Jenkins pipeline.

 
## Installation

`helm install --name jenkins-release -f values.yaml  stable/jenkins`

## CI Setup

* fork git repo `https://github.com/<your-username>/k8s-workshop-microservices.git`

* Create a new "Freestyle Project"

![image](https://user-images.githubusercontent.com/17064840/35767676-869f2014-08f9-11e8-8698-1a18be05a7ba.png)

* Fill in the GitHub URL `https://github.com/<your-username>/k8s-workshop-microservices` in the GitHub project tab and the git URL`https://github.com/<your-username>/k8s-workshop-microservices.git` in the "Source Code Management"

![image](https://user-images.githubusercontent.com/17064840/35767689-a73909ac-08f9-11e8-8064-3d75ef21233e.png)

* Create a build step "Docker Build and Publish"
* Enter the repo name and tag

![image](https://user-images.githubusercontent.com/17064840/35767691-b95dd2fc-08f9-11e8-9d38-d8cf2b96798b.png)


* Add your Docker Hub credentials by clicking on "Add" in "Registry Credentials 


![image](https://user-images.githubusercontent.com/17064840/35767696-cdfc3122-08f9-11e8-92cd-b4ab28105198.png)

 
*	Modify the "Build Context" in the "Advanced" section to the relevant service folder.
*	Modify the "Dockerfile path" in the "Advanced" section to the relevant service folder dockerfile. For example: "Dockerfile"

![image](https://user-images.githubusercontent.com/17064840/35767710-1c0d3de8-08fa-11e8-9ef5-2364f3b4e070.png)

*	Add more "Build steps" for each service. You can test is by clicking "Build Now" and checking the Jenkins console for the result.
 
## Deploy to K8S

*	Add "Build step" - > "Deploy to kubernetes"

![image](https://user-images.githubusercontent.com/17064840/37149619-5ce0af5c-22d7-11e8-9625-f8e9130d32d7.png)


*	Add the Kubernetes credentials from ~/.kube/config

![image](https://user-images.githubusercontent.com/17064840/37149687-9e41d58e-22d7-11e8-8cc9-f145642b728f.png)

*	Define the service YAML file. You can include multiple YAML files in the "Config Files" section

`Deployment/YAML/webstore.yaml`

*	Check the K8S cluster
