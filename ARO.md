# the location of your cluster
$LOCATION="eastus"     

# the name of the resource group where you want to create your cluster            
$RESOURCEGROUP="rg-aro"

# the name of your cluster
$CLUSTER="leandroarocluster"

# the name of container registry
$ACR="crcontainerappdemo"

# Create a resource group.
az group create --name $RESOURCEGROUP --location $LOCATION

# Create Service Principal
az ad sp create-for-rbac --name "sp-$RESOURCEGROUP-$CLUSTER" > app-service-principal.json

az ad sp list --show-mine -o table

$SP_CLIENT_ID = <appId>
$SP_CLIENT_SECRET = <password>
$SP_OBJECT_ID = $(az ad sp show --id $SP_CLIENT_ID  --query id --output tsv)

# Assign the Contributor role to the new service principal 
az role assignment create --role 'User Access Administrator' --assignee-object-id $SP_OBJECT_ID --resource-group $RESOURCEGROUP --assignee-principal-type 'ServicePrincipal'

az role assignment create --role 'Contributor' --assignee-object-id $SP_OBJECT_ID --resource-group $RESOURCEGROUP --assignee-principal-type 'ServicePrincipal'

# Get the service principal object ID for the OpenShift
$ARO_RP_SP_OBJECT_ID = $(az ad sp list --display-name "Azure Red Hat OpenShift RP" --query [0].id -o tsv)

# Create ACR
az acr create --resource-group $RESOURCEGROUP --name $ACR --sku Basic

# Push image
$IMAGE=$ACR + ".azurecr.io/dotnetcoreapp:v1"
az acr login --name $ACR
docker tag dotnetcoreapp $IMAGE
docker push $IMAGE
docker run -d -p 8888:80 --name dotnetcoreapp $IMAGE

# Create a new virtual network in the same resource group you created earlier:
az network vnet create --resource-group $RESOURCEGROUP --name aro-vnet --address-prefixes 10.0.0.0/22

# Add an empty subnet for the master nodes
az network vnet subnet create --resource-group $RESOURCEGROUP --vnet-name aro-vnet --name master-subnet --address-prefixes 10.0.0.0/23

# Add an empty subnet for the worker nodes.
az network vnet subnet create --resource-group $RESOURCEGROUP --vnet-name aro-vnet --name worker-subnet --address-prefixes 10.0.2.0/23

# Create the cluster
az aro create --resource-group $RESOURCEGROUP --name $CLUSTER --vnet aro-vnet --master-subnet master-subnet --worker-subnet worker-subnet

# List ARO instances
az aro list --resource-group $RESOURCEGROUP -o table

# Run the following command to find the password for the kubeadmin
az aro list-credentials --name $CLUSTER --resource-group $RESOURCEGROUP

# View Console URL
az aro show --name $CLUSTER --resource-group $RESOURCEGROUP --query "consoleProfile.url" -o tsv

# Login to the OpenShift cluster's API server
$apiServer = $(az aro show --name $CLUSTER --resource-group $RESOURCEGROUP --query "apiserverProfile.url" -o tsv)
$pass = $(az aro list-credentials --name $CLUSTER --resource-group $RESOURCEGROUP --query "kubeadminPassword" -o tsv)
oc login $apiServer -u kubeadmin -p $pass

# Listar os PODS
oc get pods
oc get nodes

# Recuperar o login e senha do Azure Container Registry
az acr credential show -n $ACR
$url_acr = $(az acr show -n $ACR --query loginServer --output tsv)
$login_acr = $(az acr credential show -n $ACR --query username --output tsv)
$password_acr = $(az acr credential show -n $ACR --query passwords[0].value --output tsv)

# Create secret for ACR
oc create secret docker-registry --docker-server=$url_acr --docker-username=$login_acr --docker-password=$password_acr --docker-email=unused acr-secret

# Aplicar o YML
oc apply -f .\k8s-deployment.yaml

# Ver a URL do Service
oc get services

# References
https://learn.microsoft.com/en-us/azure/openshift/tutorial-create-cluster
https://docs.openshift.com/container-platform/4.9/installing/installing_azure/installing-azure-user-infra.html
https://learn.microsoft.com/en-us/azure/openshift/quickstart-openshift-arm-bicep-template?pivots=aro-bicep
