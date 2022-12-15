@description('Specifies the name of the key vault.')
param name string

@description('Specifies the Azure location where the key vault should be created.')
param location string

resource loadTesting 'Microsoft.LoadTestService/loadTests@2022-12-01' = {
  name: name
  location: location
}
