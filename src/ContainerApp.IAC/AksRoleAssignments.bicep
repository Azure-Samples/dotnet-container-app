// https://github.com/Azure/bicep/discussions/3181
// https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/scenarios-rbac
// https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
// https://learn.microsoft.com/en-us/answers/questions/519799/34badrequestformat34-deploying-bicep-template-at-t.html

param acrName string
param aksPrincipalId string

resource acr 'Microsoft.ContainerRegistry/registries@2020-11-01-preview' existing = {
  name: acrName
}

resource AssignAcrPullToAks 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  //name: guid(resourceGroup().id, 'AssignAcrPullToAks')
  name: guid(resourceGroup().id, acrName, aksPrincipalId, 'AssignAcrPullToAks')   // want consistent GUID on each run
  scope: acr
  properties: {
    description: 'Assign AcrPull role to AKS'
    principalId: aksPrincipalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d') //AcrPull
  }
}
