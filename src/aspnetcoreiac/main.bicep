param acrName string
param clusterName string
param sqlserverName string
param location string = resourceGroup().location

module acrModule 'ContainerRegistry.bicep' = {
  name: 'acrDeploy'
  params: {
    acrName: acrName
    location: location
  }
}

module aksModule 'Kubernetes.bicep' = {
  name: 'aksDeploy'
  params: {
    clusterName: clusterName
    location: location
    dnsPrefix: clusterName
  }
}

module sqlModule 'SQLServer.bicep' = {
  name: 'sqlDeploy'
  params: {
    sqlserverName: sqlserverName
    location: location
    sqlAdministratorLogin: 'sqluser'
    sqlAdministratorLoginPassword: '#P@ssw0rd1234#'
    databaseName: 'TodoItem_DB'
  }
}
