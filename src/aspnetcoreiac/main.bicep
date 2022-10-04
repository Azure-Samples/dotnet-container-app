targetScope='subscription'

param rgName string
param rgLocation string
param acrName string
param clusterName string
param sqlserverName string
param sqlAdminLogin string
@secure()
param sqlAdminPassword string

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

module aksRoleAssigment 'AksRoleAssignments.bicep' = {
  name: 'aksRoleAssigment'
  scope: rgModule
  params: {
    acrName: acrName
    aksPrincipalId: aksModule.outputs.principalId
  }
}

module sqlModule 'SQLServer.bicep' = {
  name: 'sqlDeploy'
  scope: rgModule
  params: {
    sqlserverName: sqlserverName
    location: rgLocation
    sqlAdministratorLogin: sqlAdminLogin
    sqlAdministratorLoginPassword: sqlAdminPassword
    databaseName: 'TodoItem_DB'
  }
}
