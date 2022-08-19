param acrName string
param clusterName string
param sqlserverName string
param location string

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
    linuxAdminUsername: '<Linux Admin Username>'
    sshRSAPublicKey: '<SSH RSA Public Key>'
  }
}

module sqlModule 'SQLServer.bicep' = {
  name: 'sqlDeploy'
  params: {
    sqlserverName: sqlserverName
    location: location
    sqlAdministratorLogin: '<SQL Administrator Login>'
    sqlAdministratorLoginPassword: '<SQL Administrator Login Password>'
    databaseName: 'TodoItem_DB'
  }
}
