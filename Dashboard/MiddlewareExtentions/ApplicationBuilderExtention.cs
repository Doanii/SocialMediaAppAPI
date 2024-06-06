using Dashboard.Subscriptions;

namespace Dashboard.MiddlewareExtentions
{
    public static class ApplicationBuilderExtension
    {
        // Initialize the UseProductTableDependency and connect it to the database with the connection string
        public static void UsePhotoTableDependency(this IApplicationBuilder applicationBuilder, string connectionString)
        {
            // Initialize the serviceProvider with the ApplicationServices
            var serviceProvider = applicationBuilder.ApplicationServices;

            // Initialize a service with the serverProvider
            var service = serviceProvider.GetService<PostTableDependency>();
            var serviceUser = serviceProvider.GetService<UserTableDependency>();
            var serviceActivity = serviceProvider.GetService<ActivityTableDependency>();

            // Connect the service to the database with the connection string
            service!.SubscribeTableDependency(connectionString);
            serviceUser!.SubscribeTableDependency(connectionString);
            serviceActivity!.SubscribeTableDependency(connectionString);
        }
    }
}
