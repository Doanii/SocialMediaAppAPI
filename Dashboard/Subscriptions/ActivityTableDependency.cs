using Dashboard.Hubs;
using SocialMediaAppAPI.Models;
using TableDependency.SqlClient;

namespace Dashboard.Subscriptions
{
    public class ActivityTableDependency(DashboardHub dashboardHub, IConfiguration configuration) : ISubscribeTable, IDisposable
    {
        SqlTableDependency<Activity> _tableDependency;

        public void SubscribeTableDependency(string connectionString)
        {
            _tableDependency = new SqlTableDependency<Activity>(connectionString, "Activities");
            _tableDependency.OnChanged += TableDependency_OnChanged;
            _tableDependency.OnError += TableDependency_OnError;
            _tableDependency.Start();
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.Write($"{nameof(Post)} SqlTableDependency error: {e.Error.Message}");
        }

        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Activity> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                await dashboardHub.MostPopularUsers();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
