using Databaser;
namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Database database = new Database();
            database.Data.Add(new Column("Cars", typeof(string)));
            database.SaveDatabase();
        }
    }
}
