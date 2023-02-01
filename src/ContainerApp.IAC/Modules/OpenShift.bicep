// https://learn.microsoft.com/en-us/azure/openshift/quickstart-openshift-arm-bicep-template?pivots=aro-bicep

@description('Location')
param location string

@description('Domain Prefix')
param domain string

@description('Name of ARO vNet')
param clusterVnetName string = 'aro-vnet'

@description('ARO vNet Address Space')
param clusterVnetCidr string = '10.100.0.0/15'

@description('Worker node subnet address space')
param workerSubnetCidr string = '10.100.70.0/23'

@description('Master node subnet address space')
param masterSubnetCidr string = '10.100.76.0/24'

@description('Master Node VM Type')
param masterVmSize string = 'Standard_D8s_v3'

@description('Worker Node VM Type')
param workerVmSize string = 'Standard_D4s_v3'

@description('Worker Node Disk Size in GB')
@minValue(128)
param workerVmDiskSize int = 128

@description('Number of Worker Nodes')
@minValue(3)
param workerCount int = 3

@description('Cidr for Pods')
param podCidr string = '10.128.0.0/14'

@description('Cidr of service')
param serviceCidr string = '172.30.0.0/16'

@description('Unique name for the cluster')
param clusterName string

@description('Api Server Visibility')
@allowed([
  'Private'
  'Public'
])
param apiServerVisibility string = 'Public'

@description('Ingress Visibility')
@allowed([
  'Private'
  'Public'
])
param ingressVisibility string = 'Public'

@description('The Application ID of an Azure Active Directory client application')
param aadClientId string

@description('The Object ID of an Azure Active Directory client application')
param aadObjectId string

@description('The secret of an Azure Active Directory client application')
@secure()
param aadClientSecret string

@description('The ObjectID of the Resource Provider Service Principal')
param rpObjectId string

var contributorRoleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
var resourceGroupId = '/subscriptions/${subscription().subscriptionId}/resourceGroups/aro-${domain}-${location}'
var masterSubnetId=resourceId('Microsoft.Network/virtualNetworks/subnets', clusterVnetName, 'master')
var workerSubnetId=resourceId('Microsoft.Network/virtualNetworks/subnets', clusterVnetName, 'worker')

resource clusterVnetName_resource 'Microsoft.Network/virtualNetworks@2020-05-01' = {
  name: clusterVnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        clusterVnetCidr
      ]
    }
    subnets: [
      {
        name: 'master'
        properties: {
          addressPrefix: masterSubnetCidr
          serviceEndpoints: [
            {
              service: 'Microsoft.ContainerRegistry'
            }
          ]
          privateLinkServiceNetworkPolicies: 'Disabled'
        }
      }
      {
        name: 'worker'
        properties: {
          addressPrefix: workerSubnetCidr
          serviceEndpoints: [
            {
              service: 'Microsoft.ContainerRegistry'
            }
          ]
        }
      }
    ]
  }
}

resource clusterVnetName_Microsoft_Authorization_id_name_aadObjectId 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(aadObjectId, clusterVnetName_resource.id, contributorRoleDefinitionId)
  scope: clusterVnetName_resource
  properties: {
    roleDefinitionId: contributorRoleDefinitionId
    principalId: aadObjectId
    principalType: 'ServicePrincipal'
  }
}

resource clusterVnetName_Microsoft_Authorization_id_name_rpObjectId 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: guid(rpObjectId, clusterVnetName_resource.id, contributorRoleDefinitionId)
  scope: clusterVnetName_resource
  properties: {
    roleDefinitionId: contributorRoleDefinitionId
    principalId: rpObjectId
    principalType: 'ServicePrincipal'
  }
}

resource clusterName_resource 'Microsoft.RedHatOpenShift/OpenShiftClusters@2020-04-30' = {
  name: clusterName
  location: location
  properties: {
    clusterProfile: {
      domain: domain
      resourceGroupId: resourceGroupId
    }
    networkProfile: {
      podCidr: podCidr
      serviceCidr: serviceCidr
    }
    servicePrincipalProfile: {
      clientId: aadClientId
      clientSecret: aadClientSecret
    }
    masterProfile: {
      vmSize: masterVmSize
      subnetId: masterSubnetId
    }
    workerProfiles: [
      {
        name: 'worker'
        vmSize: workerVmSize
        diskSizeGB: workerVmDiskSize
        subnetId: workerSubnetId
        count: workerCount
      }
    ]
    apiserverProfile: {
      visibility: apiServerVisibility
    }
    ingressProfiles: [
      {
        name: 'default'
        visibility: ingressVisibility
      }
    ]
  }
  dependsOn: [
    clusterVnetName_resource
  ]
}
