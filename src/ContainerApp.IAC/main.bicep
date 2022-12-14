param acrName string
param clusterName string
param sqlserverName string
param kvName string
param loadTestName string
param location string = resourceGroup().location

var aksDev = toLower('${clusterName}-dev')
var aksPrd = toLower('${clusterName}-prd')

module acrModule 'ContainerRegistry.bicep' = {
  name: 'acrDeploy'
  params: {
    acrName: acrName
    location: location
  }
}

module aksModuleDev 'Kubernetes.bicep' = {
  name: 'aksDeployDev'
  params: {
    clusterName: aksDev
    dnsPrefix: aksDev
    location: location
  }
}

module aksRoleAssigment 'AksRoleAssignments.bicep' = {
  name: 'aksRoleAssigmentDev'
  params: {
    acrName: acrName
    aksPrincipalId: aksModuleDev.outputs.principalId
  }
}

module aksModulePrd 'Kubernetes.bicep' = {
  name: 'aksDeployPrd'
  params: {
    clusterName: aksPrd
    dnsPrefix: aksPrd
    location: location
  }
}

module aksRoleAssigmentPrd 'AksRoleAssignments.bicep' = {
  name: 'aksRoleAssigmentPrd'
  params: {
    acrName: acrName
    aksPrincipalId: aksModulePrd.outputs.principalId
  }
}

module sqlModule 'SQLServer.bicep' = {
  name: 'sqlDeploy'
  params: {
    sqlserverName: sqlserverName
    location: location
    sqlAdministratorLogin: 'leandro'
    sqlAdministratorLoginPassword: '#P@ssw0rd1234#'
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
    {
      secretName: 'ConnString'
      secretValue: sqlModule.outputs.connectionString
    }
  ]
}

module keyVaultModule 'KeyVault.bicep' = {
  name: 'keyVaultDeploy'
  params: {
    keyVaultName: kvName
    location: location
    objectId: '051c760b-9769-4f0f-9828-09197ff28b7c'
    secretsObject: secrets
  }
}

module loadTestModule 'LoadTest.bicep' = {
  name: 'loadTestDeploy'
  params:{
    name: loadTestName
    location: location
  }
}
