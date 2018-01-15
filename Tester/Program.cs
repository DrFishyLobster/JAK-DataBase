using DatabaseProject;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Database test = new Database();
            test.Data.Add(new Column("Couch", typeof(string)));
            test.Data[0].Data.Add(new Record(new byte[] { 1, 2, 4, 8, 16, 32, 64, 3 }, typeof(string)));
            test.SaveDatabase();
            Database.LoadDatabase(@"C:\Users\User\Desktop\Delete Me.bin");
        }
    }
}
