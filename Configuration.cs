namespace eventHub_group_of_appInsights_requests
{
    public static class Configuration
    {
        public static readonly string StorageConnectionString = "<< CONNECTION STRING FOR THE STORAGE ACCOUNT >>";
        public static readonly string BlobContainerName = "<< NAME OF THE BLOB CONTAINER >>";

        public static readonly string EventHubsConnectionString = "<< CONNECTION STRING FOR THE EVENT HUBS NAMESPACE >>";
        public static readonly string EventHubName = "<< NAME OF THE EVENT HUB >>";

        public static readonly string InstrumentationKey = "VALUE GOES HERE";
    }
}