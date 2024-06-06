using Dashboard.Hubs;
using SocialMediaAppAPI.Models;
using System.Runtime.CompilerServices;
using TableDependency.SqlClient;

namespace Dashboard.Subscriptions
{
    public class UserTableDependency(DashboardHub dashboardHub, IConfiguration configuration) : ISubscribeTable, IDisposable
    {
        SqlTableDependency<User> _tableDependency;

        public void SubscribeTableDependency(string connectionString)
        {
            _tableDependency = new SqlTableDependency<User>(connectionString, "Users");
            _tableDependency.OnChanged += TableDependency_OnChanged;
            _tableDependency.OnError += TableDependency_OnError;
            _tableDependency.Start();
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.Write($"{nameof(User)} SqlTableDependency error: {e.Error.Message}");
        }

        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<User> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                await dashboardHub.UserCount();
                await dashboardHub.UserJoinsPerDay();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
