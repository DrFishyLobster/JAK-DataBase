<<<<<<< HEAD
﻿using Databaser;
=======
﻿using DatabaseProject;
>>>>>>> 412daee363f6afc3ae6fe4b206995fd04fe88c45
namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
<<<<<<< HEAD
            System.Console.WriteLine("HI");
=======
            Database test = new Database();
            test.Data.Add(new Column("Couch", typeof(byte)));
            test.Data[0].Data.Add(new Record(new byte[] { 1 }, typeof(byte)));
            test.Data[0].Data.Add(new Record(new byte[] { 3 }, typeof(byte)));
            test.Data[0].Data.Add(new Record(new byte[] { 2 }, typeof(byte)));
            test.Data[0].Data.Add(new Record(new byte[] { 4 }, typeof(byte)));
            test.ApplyPattern(test.Data[0].GenerateSortPattern(SortStyle.Descending));
            
            test.SaveDatabase();
            Database.LoadDatabase(@"C:\Users\User\Desktop\Delete Me.bin");
>>>>>>> 412daee363f6afc3ae6fe4b206995fd04fe88c45
        }
    }
}
