targetScope='subscription'

param rgName string
param rgLocation string
param acrName string
param clusterName string
param sqlserverName string

resource rgModule 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: rgName
  location: rgLocation
}

module acrModule 'ContainerRegistry.bicep' = {
  name: 'acrDeploy'
  scope: rgModule
  params: {
    acrName: acrName
    location: rgLocation
  }
}

module aksModule 'Kubernetes.bicep' = {
  name: 'aksDeploy'
  scope: rgModule
  params: {
    clusterName: clusterName
    location: rgLocation
    dnsPrefix: clusterName
  }
}

module sqlModule 'SQLServer.bicep' = {
  name: 'sqlDeploy'
  scope: rgModule
  params: {
    sqlserverName: sqlserverName
    location: rgLocation
    sqlAdministratorLogin: 'sqluser'
    sqlAdministratorLoginPassword: '#P@ssw0rd1234#'
    databaseName: 'TodoItem_DB'
  }
}
