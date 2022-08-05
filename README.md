# Dotnet Container App

This .NET 6 example application to demonstrate the process of creating a container application and deploying it to Azure Kubernetes Services

![Application](/images/img01.png "Application")

The project is divided as follows:

- **src/aspnetcoreiac:** Project for Bicep files for create Azure Environment
- **src/aspnetcorewebapi:** Project for Rest APIs and communicate with database
- **src/aspnetcorewebapp:** Project for my Web Application interact with API
- **src/aspnetcorewebapi.test:** Project for run Unit Testing


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

This application has the *.vscode\launch.json* file with the compound settings to run both projects (API and APP) at the same time

![Compound Settings](/images/img02.png "Application")

To run the application in debug mode, select the Debug menu and select the *aspnetcorewebapi & aspnetcorewebapp* option as shown in the image below

![Running](/images/img03.png "Application")

## Publish to Azure

