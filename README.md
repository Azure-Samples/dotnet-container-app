# Dotnet Container App

This .NET 6 example application to demonstrate the process of creating a container application and deploying it to Azure Kubernetes Services

![Application](/images/img01.png "Application")

The project is divided as follows:

- **src/aspnetcoreiac:** Bicep files for create Azure environment
- **src/aspnetcorewebapi:** Rest APIs to communicate with database
- **src/aspnetcorewebapp:** Web Application interact with rest API
- **src/aspnetcorewebapi.test:** Unit Testing project

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

This application has the *.vscode/launch.json* file with the compound settings to run both projects (API and APP) at the same time

![Compound Settings](/images/img02.png "Application")

To run the application in debug mode, select the Debug menu and select the *aspnetcorewebapi & aspnetcorewebapp* option as shown in the image below

![Running](/images/img03.png "Application")

Another option to run application based on docker containers. This application user 3 containers as show bellow:

- **aspnetcoredb:** SQL Server
- **aspnetcorewebapi:** Rest APIs to communicate with SQL Server database (backend)
- **aspnetcorewebapp:** ASP NET MVC application to communicate with API (frontend)

 In the root directory of the application we have the *DockerCompose.yml* file with all the necessary configurations to running the application using containers. To start run the command below.

```sh
docker-compose -f 'DockerCompose.yml' up --build -d
```

![Docker Compose](/images/img04.png "DockerCompose")

## Publish to Azure

In this section I will show you the steps to publish the application in Azure. To create the environment in Azure we are using the concept of Infrastructure as Code (IAC) where we have a bicep file that we declare all the resources we need to create in Azure. You can see this [main.bicep](src/aspnetcoreiac/main.bicep) file.

This bicep file will create the following resources in Azure.

- **[Azure Container Registry (ACR)](https://docs.microsoft.com/en-us/azure/container-registry/)** ACR allows you to build, store, and manage container images and artifacts in a private registry for all types of container deployments

- **[Azure Kubernetes Services (AKS)](https://docs.microsoft.com/en-us/azure/aks/intro-kubernetes)** AKS simplifies deploying a managed Kubernetes cluster in Azure by offloading the operational overhead to Azure

- **[SQL Server Database](https://docs.microsoft.com/en-us/azure/azure-sql/azure-sql-iaas-vs-paas-what-is-overview?view=azuresql)** is a relational database-as-a-service (DBaaS) hosted in Azure that falls into the industry category of Platform-as-a-Service (PaaS).

- **[Azure Monitor](https://docs.microsoft.com/en-us/azure/azure-monitor/overview)** helps you maximize the availability and performance of your applications and services. It delivers a comprehensive solution for collecting, analyzing, and acting on telemetry from your cloud and on-premises environments.

First step is creating the Azure Resource Group

```sh
$rgName = 'rg-dotnet-containerapp'
$location = 'eastus'

# Authenticate to Azure
az login

# Ger subscription ID
$subscriptionID = $(az account show --query id -o tsv)

# Create resource group
az group create --name $rgName --location $location
```

Next step is create a Service Principal identity to GitHub connect in this Azure resoure group

```sh
# Create Service Principal
az ad sp create-for-rbac --name spn-dotnet-containerapp --role contributor --scopes /subscriptions/$subscriptionID/resourceGroups/$rgName --sdk-auth 
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

![Add Secret](/images/img05.png "Add Secret")

Now we are ready to start the workflow [aspnetcore-bicep.yml](.github/workflows/aspnetcore-bicep.yml) that executes the bicep file and creates all the resources that the application needs.

- Under your repository name, click *Actions* tab.
- In the left sidebar, click the workflow "aspnetcore.bicep".
- Above the list of workflow runs, select *Run workflow*.
- Use the Branch dropdown to select the workflow's main branch, Click Run workflow.

![Bicep Workflow](/images/img06.png "Bicep Workflow")

After deployment, below resources will be created

![Azure Resources](/images/img07.png "Azure Resources")

After our environment is available, we need to create some GitHub Secrets variables to connect Azure Container Registry (ACR).

```sh
# Show login server url
az acr show -n acrdotnetcontainerapp --query loginServer --output tsv

# Show username
az acr credential show -n acrdotnetcontainerapp --query username --output tsv

# Show password
az acr credential show -n acrdotnetcontainerapp --query passwords[0].value --output tsv
```

Store the output values as a GitHub secret named *ACR_URL*, *ACR_LOGIN* and *ACR_PASSWORD*
- Under your repository name, click *Settings*.
- In the *Security* section of the sidebar, click *Secrets* and select *Actions*.
- At the top of the page, click *New repository secret*
- Provide the secret name as *ACR_URL*
- Add the output as secret value
- Click *Add secret* button.

Repeat these steps for the secrets ACR_LOGIN and ACR_PASSWORD.

Finally we will need the database connection string, store the output value as a GitHub secret named SQL_CONNECTION.

```
az sql db show-connection-string --name TodoItem_DB --server sqldotnetcontainerapp --client ado.net --output tsv
```

In the end we should have the following GitHub secrets created.

![GitHub Secrets for ACR](/images/img08.png "GitHub Secrets for ACR")

Now we can deploy our application to AKS by starting the workflow [aspnetcore.docker.yml](.github/workflows/aspnetcore-docker.yml). This workflow has the following steps

- **Build**
    - Check out source code
    - Docker Login to ACR
    - Docker Build and Push to ACR
    - Replace the image URL variables
    - Create kubernetes YML artifact
- **Release**
    - Download the kubernetes YML artifact 
    - Configure the AKS context 
    - Create Secret
    - Deploy the application to AKS

After the workflow ends, our application will be available for use.

- Log in Azure Portal
- Select the resouce group *rg-dotnet-containerapp*
- Select the AKS cluster *aksdotnetcontainerapp*
- In the *Kubernetes resources* section of the sidebar, click *Services and ingresses*
- Check the external IP for the *aspnetcorewebapp-svc* service

![Kubernetes Services](/images/img09.png "Kubernetes Services")

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

## Found an Issue?
If you find a bug in the source code or a mistake in the documentation, you can help us by [submitting an issue](CONTRIBUTING.md#-submitting-an-issue) to the GitHub Repository. Even better, you can [submit a Pull Request](CONTRIBUTING.md#-submitting-a-pull-request-pr) with a fix.

## Want a Feature?
You can *request* a new feature by [submitting an issue](CONTRIBUTING.md#-submitting-an-issue) to the GitHub Repository. If you would like to *implement* a new feature, please submit an issue with a proposal for your work first, to be sure that we can use it.

* **Small Features** can be crafted and directly [submitted as a Pull Request](CONTRIBUTING.md#-submitting-an-issue).

For more details [Contributing to Dotnet Container App](CONTRIBUTING.md).