# Dotnet Container App

This .NET 6 example application to demonstrate the process of creating a container application. You can choose to publish the application to Azure Kubernetes Services (AKS) or Azure RedHat OpenShift (ARO).

![Application](/docs/images/img01.png "Application")

This is the application architecture diagram

![Architecture Diagram](/docs/images/img14.png "Architecture Diagram")

The project is divided as follows:

- **src/ContainerApp.IAC:** Bicep files for create Azure environment
- **src/ContainerApp.TodoApi:** TODO API to communicate with SQL Server database
- **src/ContainerApp.WeatherApi:** WEATHER API simulates weather forecasts
- **src/ContainerApp.WebApp:** Web Application interact with rest TODO API and
- **src/ContainerApp.Test:** Unit Testing project

## Getting Started

### Prerequisites

- [Dotnet 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Visual Studio Code](https://code.visualstudio.com/download)
- [Azure Account](https://azure.microsoft.com/en-us/free/)

### Quickstart

Follow the steps below to run the application locally

```sh
# Clone the repository
git clone https://github.com/Azure-Samples/dotnet-container-app.git

# switch to repository directory
cd dotnet-container-app

# Open Visual Studio Code
code .
```

You will need to configure the ```Connection String``` in file */src/aspnetcorewebapi/appsettings.json*

![Connection String](/docs/images/img10.png "Application")

```
Connection String example: *Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=TodoItem_DB;Integrated Security=SSPI;*
```

To run the application in debug mode, select the *Debug* menu and select the *aspnetcorewebapi & aspnetcorewebapp* option as shown in the image below

![Running](/docs/images/img03.png "Application")

Another option to run application based on docker containers. In the root directory of the application we have the *DockerCompose.yml* file with all the necessary configurations to running the application using containers. To start run the command below.

```sh
docker-compose -f 'DockerCompose.yml' up --build -d
```

![Docker Compose](/docs/images/img04.png "DockerCompose")

## Publishing the application
Now you can choose to publish the application to Azure Kubernetes Services (AKS) or Azure RedHat OpenShift (ARO).

## Connect GitHub and Azure Cloud
First step is create a Service Principal identity to GitHub connect to Azure Subscription

```sh
# Login Azure
az login

# Get subscription ID
$subscriptionID = $(az account show --query id -o tsv)

# Create Service Principal
az ad sp create-for-rbac --name <Service Principal Name> --role contributor --scopes /subscriptions/$subscriptionID --sdk-auth 
```

The command should output a JSON object similar to this:

```json
{
  "clientId": "<GUID>",
  "clientSecret": "<PASSWORD>",
  "subscriptionId": "<GUID>",
  "tenantId": "<GUID>",
  "activeDirectoryEndpointUrl": "<URL>",
  "resourceManagerEndpointUrl": "<URL>",
  "activeDirectoryGraphResourceId": "<URL>",
  "sqlManagementEndpointUrl": "<URL>",
  "galleryEndpointUrl": "<URL>",
  "managementEndpointUrl": "<URL>"
}
```

Store the output JSON as the value of a GitHub Actions secret named 'AZURE_CREDENTIALS'

- Under your repository name, click *Settings*.
- In the *Security* section of the sidebar, click *Secrets* and select *Actions*.
- At the top of the page, click *New repository secret*
- Provide the secret name as *AZURE_CREDENTIALS*
- Add the output JSON as secret value
- Click *Add secret* button.

![Add Secret](/docs/images/img05.png "Add Secret")

Create other two secrets
1. Store SQL username 'AZURE_SQL_USERNAME' 
2. Store SQL password 'AZURE_SQL_PASSWORD'

This sample application can use the following Azure features:

- **[Azure Container Registry (ACR)](https://docs.microsoft.com/en-us/azure/container-registry/)** ACR allows you to build, store, and manage container images and artifacts in a private registry for all types of container deployments

- **[Azure Kubernetes Services (AKS)](https://docs.microsoft.com/en-us/azure/aks/intro-kubernetes)** AKS simplifies deploying a managed Kubernetes cluster in Azure by offloading the operational overhead to Azure

- **[Azure RedHat OpenShift (ARO)](https://learn.microsoft.com/en-us/azure/openshift/)** Azure Red Hat OpenShift provides highly available, fully managed OpenShift clusters on demand, monitored and operated jointly by Microsoft and Red Hat. Kubernetes is at the core of Red Hat OpenShift

- **[SQL Server Database](https://docs.microsoft.com/en-us/azure/azure-sql/azure-sql-iaas-vs-paas-what-is-overview?view=azuresql)** is a relational database-as-a-service (DBaaS) hosted in Azure that falls into the industry category of Platform-as-a-Service (PaaS).

- **[Azure Monitor](https://docs.microsoft.com/en-us/azure/azure-monitor/overview)** helps you maximize the availability and performance of your applications and services. It delivers a comprehensive solution for collecting, analyzing, and acting on telemetry from your cloud and on-premises environments.

- **[Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/)** is a cloud service for securely storing and accessing secrets. A secret is anything that you want to tightly control access to, such as API keys, passwords, certificates, or cryptographic keys.

## Deploy to Azure Kubernetes Services (AKS)

In this section I will show you the steps to publish the application in **Azure Kubernetes Services**. To create the environment we are using the concept of Infrastructure as Code (IAC) where we have a bicep file that we declare all the resources we need to create in Azure. You can see this [aks.bicep](src/ContainerApp.IAC/aks.bicep) file.

To create the environment, you need to configure the following variables in the workflow [aks-aspnetcore-deployment.yml](.github/workflows/aks-aspnetcore-deployment.yml).

- AZ_RG_NAME
- AZ_RG_LOCATION
- AZ_ACR_NAME
- AZ_AKS_NAME
- AZ_SQLSERVER_NAME
- AZ_KV_NAME
- AZ_LOADTEST_NAME

This workflow has the following steps:

- **IAC**
  - Run the Bicep file to create environment
- **Build**
  - Check out source code
  - Docker Login to ACR
  - Docker Build and Push to ACR
  - Replace the image URL variables
  - Create kubernetes YML artifact
- **Release**
  - Download the kubernetes YML artifact
  - Configure the AKS context
  - Deploy the application to AKS

This example using manual trigger, to start the workflow following these steps:

- Under your repository name, click *Actions* tab.
- In the left sidebar, click the workflow *aks-aspnetcore-deployment*.
- Above the list of workflow runs, select *Run workflow*.
- Use the Branch dropdown to select the workflow's main branch, Click *Run workflow*.

![Bicep Workflow](/docs/images/img06.png "Bicep Workflow")

Workflow result

![Azure Resources](/docs/images/img13.png "Azure Resources")

After deployment, below resources will be created in your Azure subscription

![Azure Resources](/docs/images/img07.png "Azure Resources")

After the workflow ends, our application will be available for use.

- Log in Azure Portal
- Select the resource group *rg-dotnet-containerapp*
- Select the AKS cluster *Kubernetes Service Name*
- In the *Kubernetes resources* section of the sidebar, click *Services and ingresses*
- Check the external IP for the *aspnetcorewebapp-svc* service

![Kubernetes Services](/docs/images/img09.png "Kubernetes Services")

## Deploy to Azure RedHat OpenShift (ARO)
In this section I will show you the steps to publish the application in **Azure RedHat OpenShift**. To publish this application in ARO it is necessary to follow the following manual steps.

```ps
# Variables
$RESOURCEGROUP = "rg-dotnetcontainerapp-aro"
$LOCATION = "eastus"
$CLUSTER = "arodotnetcontainerapp"

# Create a resource group.
az group create --name $RESOURCEGROUP --location $LOCATION

# Create Service Principal
az ad sp create-for-rbac --name "sp-$RESOURCEGROUP-$CLUSTER"

# Save the values into variables
$SP_CLIENT_ID = <appId returned from above command >
$SP_CLIENT_SECRET = <password returned from above command>
$SP_OBJECT_ID = $(az ad sp show --id $SP_CLIENT_ID  --query id --output tsv)

# Assign the Contributor role to the new service principal 
az role assignment create --role 'User Access Administrator' --assignee-object-id $SP_OBJECT_ID --resource-group $RESOURCEGROUP --assignee-principal-type 'ServicePrincipal'

az role assignment create --role 'Contributor' --assignee-object-id $SP_OBJECT_ID --resource-group $RESOURCEGROUP --assignee-principal-type 'ServicePrincipal'

# Get the service principal object ID for the OpenShift
$ARO_RP_SP_OBJECT_ID = $(az ad sp list --display-name "Azure Red Hat OpenShift RP" --query [0].id -o tsv)
```

Now let's create Secrets on GitHub to store these informations

![ARO Secrets](/docs/images/img15.png "ARO Secrets")

To create the environment we are using this bicep file [aro.bicep](src/ContainerApp.IAC/aro.bicep).

Now you need to configure the following variables in the workflow [aro-aspnetcore-deployment.yml](.github/workflows/aro-aspnetcore-deployment.yml).

- AZ_RG_NAME
- AZ_RG_LOCATION
- AZ_ACR_NAME
- AZ_ARO_NAME
- AZ_SQLSERVER_NAME
- AZ_KV_NAME
- AZ_LOADTEST_NAME

This example using manual trigger, to start the workflow following these steps:

- Under your repository name, click *Actions* tab.
- In the left sidebar, click the workflow *aro-aspnetcore.deployment*.
- Above the list of workflow runs, select *Run workflow*.
- Use the Branch dropdown to select the workflow's main branch, Click *Run workflow*.

![Bicep Workflow](/docs/images/img16.png "Bicep Workflow")

Workflow result

![Azure Resources](/docs/images/img17.png "Azure Resources")

After deployment, below resources will be created in your Azure subscription

![Azure Resources](/docs/images/img18.png "Azure Resources")

For you to manage your application in ARO it is necessary to execute the command below to return the URL of the ARO administration portal

```ps
# View Console URL
az aro show --name $CLUSTER --resource-group $RESOURCEGROUP --query "consoleProfile.url" -o tsv

# Run the following command to find the password for the kubeadmin
az aro list-credentials --name $CLUSTER --resource-group $RESOURCEGROUP
```

- Log in ARO Portal with console URL and password
- Select *Developer* mode
- Select *Topology*
- Select the *containerapp-webapp-deploy*
- Check the external IP in the Routes section

![ARO portal](/docs/images/img19.png "ARO Portal")

# Contributing
This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

## Found an Issue?
If you find a bug in the source code or a mistake in the documentation, you can help us by [submitting an issue](CONTRIBUTING.md#-submitting-an-issue) to the GitHub Repository. Even better, you can [submit a Pull Request](CONTRIBUTING.md#-submitting-a-pull-request-pr) with a fix.

## Want a Feature?
You can *request* a new feature by [submitting an issue](CONTRIBUTING.md#-submitting-an-issue) to the GitHub Repository. If you would like to *implement* a new feature, please submit an issue with a proposal for your work first, to be sure that we can use it.

* **Small Features** can be crafted and directly [submitted as a Pull Request](CONTRIBUTING.md#-submitting-an-issue).

For more details [Contributing to Dotnet Container App](CONTRIBUTING.md).
