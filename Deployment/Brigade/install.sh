helm repo add brigade https://azure.github.io/brigade
helm install -n brigade brigade/brigade
helm install brigade/brigade-project -n stickerstore-project -f values.yaml
