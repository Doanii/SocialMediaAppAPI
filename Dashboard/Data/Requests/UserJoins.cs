namespace Dashboard.Data.Requests
{
    public class UserJoins
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        
        public UserJoins(DateTime date, int count)
        {
            Date = date;
            Count = count;
        }
    }
}
