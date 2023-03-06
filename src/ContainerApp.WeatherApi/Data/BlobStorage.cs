using System.Diagnostics.CodeAnalysis;

namespace ContainerApp.WeatherApi.Data
{
    public class BlobStorage
    {
        private string blobConnection = "DefaultEndpointsProtocol=https;AccountName=testeernestogaia;AccountKey=vckJ5MFD+VvmSwHRXX+9WBNDC0RQ8W6YdJerkJb1YfIcVbJtNEvkLjIXmRDbJ6EQA2uVeOv1Lax7+AStVFgNuQ==;EndpointSuffix=core.windows.net";

        public BlobStorage()
        {
        }

        public string Upload()
        {
            return blobConnection;
        }
    }
}