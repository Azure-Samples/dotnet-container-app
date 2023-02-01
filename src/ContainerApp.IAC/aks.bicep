param acrName string
param clusterName string
param sqlserverName string
param sqlAdminLogin string
@secure()
param sqlAdminPassword string
param kvName string
param loadTestName string
param location string = resourceGroup().location

module acrModule 'Modules/ContainerRegistry.bicep' = {
  name: 'acrDeploy'
  params: {
    acrName: acrName
    location: location
  }
}

module aksModuleDev 'Modules/Kubernetes.bicep' = {
  name: 'aksDeployDev'
  params: {
    clusterName: clusterName
    dnsPrefix: clusterName
    location: location
  }
}

module aksRoleAssigment 'Modules/AksRoleAssignments.bicep' = {
  name: 'aksRoleAssigmentDev'
  params: {
    acrName: acrName
    aksPrincipalId: aksModuleDev.outputs.principalId
  }
}

module sqlModule 'Modules/SQLServer.bicep' = {
  name: 'sqlDeploy'
  params: {
    sqlserverName: sqlserverName
    location: location
    sqlAdministratorLogin: sqlAdminLogin
    sqlAdministratorLoginPassword: sqlAdminPassword
    databaseName: 'TodoItem_DB'
  }
}

var secrets =  {
  secrets: [
    {
      secretName: 'Secret01'
      secretValue: 'Value01'
    }
    {
      secretName: 'Secret02'
      secretValue: 'Value02'
    }
    {
      secretName: 'Secret03'
      secretValue: 'Value03'
    }
    //{
    //  secretName: 'ConnString'
    //  secretValue: sqlModule.outputs.connectionString
    //}
  ]
}

module keyVaultModule 'Modules/KeyVault.bicep' = {
  name: 'keyVaultDeploy'
  params: {
    keyVaultName: kvName
    location: location
    objectId: '051c760b-9769-4f0f-9828-09197ff28b7c'
    secretsObject: secrets
  }
}

module loadTestModule 'Modules/LoadTest.bicep' = {
  name: 'loadTestDeploy'
  params:{
    name: loadTestName
    location: location
  }
}
