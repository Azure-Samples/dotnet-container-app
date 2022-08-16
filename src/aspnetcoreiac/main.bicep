param acrName string
param clusterName string
param sqlserverName string

module acrModule 'ContainerRegistry.bicep' = {
  name: 'acrDeploy'
  params: {
    acrName: acrName
  }
}

module aksModule 'Kubernetes.bicep' = {
  name: 'aksDeploy'
  params: {
    clusterName: clusterName
    dnsPrefix: clusterName
    linuxAdminUsername: 'leandroprado'
    sshRSAPublicKey: 'ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQD7N8ZXEi+08cUSEVfxrkUU6jfwSQgcDxGEGdxWAz4hYHkHnv7qC+Xe2mvSitmSxUTXjVBprCGsKxvckAJl1xaeGKf0eC1gX+fVxcaAICy19MHYXE1ChTP+lqzRLeTqEGXLwOxWdrZgQmwITBza/9Yw7MHnc+VFN7xFolp0eG0cZMKMI3JyyJpODBBFZW0MTyMcHMVKzPMOo7Od7EUnK2+o/2bSOUkyc5b2qweKWparoEWUA7w5Zp9fOBL1cr9mRYFBhpnQkvgovbmSGgKn1XuN9DJRwlKDrr9o9SvKjSYIYgPEemMzivKhFUl4Okk3iOxuuQbhXSalFjpy9zvzRMl7 southamerica\\leadro@leadro-2021'
  }
}

module sqlModule 'SQLServer.bicep' = {
  name: 'sqlDeploy'
  params: {
    sqlserverName: sqlserverName
    sqlAdministratorLogin: 'leandro'
    sqlAdministratorLoginPassword: '#P@ssw0rd1234#'
    databaseName: 'TodoItem_DB'
  }
}
