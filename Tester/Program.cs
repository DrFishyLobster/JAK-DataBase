using DatabaseProject;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Database test = new Database();
            test.FilePath = @"C:\Users\User\Desktop\Delete Me.bin";
            test.Data.Add(new Column("Coach", typeof(string)));
            test.Data[0].Data.Add(new Record(new byte[] { 1, 2, 4, 8, 16, 32, 64, 3 }, typeof(string)));
            test.SaveDatabase();
            Database.LoadDatabase(@"C:\Users\User\Desktop\Delete Me.bin");
        }
    }
}
