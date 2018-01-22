using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;
namespace Databaser
{

    #region Database Manager
    public class DatabaseManager
    {
        Database TheDatabase;
        public string LastSave
        {
            get
            {
                return Converter.ByteToString(TheDatabase.DateOfLastSave, typeof(DateTime));
            }
        }
        public DatabaseManager()
        {
            if (!Directory.Exists(Database.FolderPath))
            {
                Directory.CreateDirectory(Database.FolderPath);
            }
            bool o = true;
            bool bts = false;
            #region Level 1
            while (o)
            {
                int res = TryToAskQuestion("0.Load Database\n1.New Database\n2.Close", 2);
                switch (res)
                {
                    case 0:

                        TheDatabase = Load_Database();
                        if (TheDatabase == null) bts = true;
                        break;
                    case 1:
                        //NewDataBase
                        break;
                    case 2:
                        o = false;
                        break;

                }
                if (bts) { bts = false; continue; }
                if (res == 0 || res == 1)
                {
                    bool o2 = true;
                    Console.WriteLine($"You are running JAK Database Solutions\nWelcome to {TheDatabase.FileName}!" +
                        $"\nThe last edit was made {LastSave}.\n");
                    #region Level 2
                    while (o2)
                    {
                        int resl2 = TryToAskQuestion("0.Display\n1.Edit,Add,Remove\n2.Query\n3.Save\n4.Close", 4);
                        switch (resl2)
                        {
                            case 0:
                                View_EntireDatabase();
                                break;
                            case 1:
                                //EDIT,ADD,REMOVE
                                break;
                            case 2:
                                QueryCopyOfDatabase();
                                break;
                            case 3:
                                TheDatabase.SaveDatabase();
                                Console.WriteLine("Save complete");
                                break;
                            case 4:
                                o2 = false;
                                break;

                        }
                    }
                    #endregion
                }
            }
            #endregion
        }

        public int TryToAskQuestion(string Q, int maxValue)
        {
            Console.WriteLine(Q);
            int res;
            string i = Console.ReadLine();
            while (!Int32.TryParse(i, out res) || res < 0 || res > maxValue)
            {
                Console.WriteLine("Incorrect Input. Reprompting");
                i = Console.ReadLine();
            }
            return res;

        }
        public void QueryCopyOfDatabase()
        {
            Database copy = TheDatabase.CopyDatabase();
            bool o = true;
            #region Level 3
            while (o)
            {
                int res = TryToAskQuestion("0.Sort\n1.Filter\n2.Display\n3.Save File\n4.Replace Database\n5.Return to Menu", 6);
                switch (res)
                {
                    case 0:
                        #region Sorter
                        for (int col = 0; col < copy.Data.Count; col++)
                        {
                            Console.WriteLine($"{col}.{copy[col].Name}");
                        }
                        int column = TryToAskQuestion("", copy.Data.Count);
                        copy.ApplyPattern(copy[column].GenerateSortPattern((SortStyle)
                            TryToAskQuestion("0.Ascending\n1.Descending", 2)));
                        #endregion
                        break;
                    case 1:
                        #region Filterer
                        for (int col = 0; col < copy.Data.Count; col++)
                        {
                            Console.WriteLine($"{col}.{copy[col].Name}");
                        }
                        int columnfilter = TryToAskQuestion("", copy.Data.Count);
                        FilterStyle res2 = (FilterStyle)TryToAskQuestion("0.Equal To\n1.Greater Than Or Equal To\n2.Less Than Or Equal To\n3.Greater Than\n4.Less Than", 5);
                        Console.WriteLine("Please insert your comparetor");
                        byte[] input = RequestData(copy[columnfilter].type);
                        copy.ApplyPattern(copy[columnfilter].GenerateFilterPattern((FilterStyle)res, new Record(input, copy[columnfilter].type)));
                        #endregion
                        break;
                    case 2:
                        #region Display
                        int res3 = TryToAskQuestion("Please insert the how many of the top elements you want?", copy.Data.Count);
                        for (int rec = 0; rec < res3; rec++)
                        {
                            //Print out record;
                        }
                        #endregion
                        break;
                    case 3:
                        #region Save File
                        //KARAN
                        #endregion
                        break;
                    case 4:
                        #region Replace Database
                        TheDatabase = copy;
                        Console.WriteLine("Database replaced");
                        #endregion
                        break;
                    case 5:
                        o = false;
                        break;

                }
            }
            #endregion
        }

        private byte[] RequestData(Type type)
        {
            byte[] output;
            while (!Converter.TryParseToByte(Console.ReadLine(), type, out output))
            {
                Console.WriteLine("Invalid Input, Reprompting...");
            }
            return output;
        }

        public void View_EntireDatabase()
        {

            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            List<string> ColumnHeadings = TheDatabase.sColumnHeadings();

            for (int r = 0; r < ColumnHeadings.Count; r++)
                Console.Write(ColumnHeadings[r] + "\t");
            Console.ResetColor();

            //LEAVE THIS ALONE!!!!
            Console.WriteLine();

            for (int RecordNum = 0; RecordNum < TheDatabase.Data[0].Data.Count /*Number of records in database*/; RecordNum++)
            {
                //display current record

                List<string> RecordElements = TheDatabase.sRecord(RecordNum);
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.White;

                for (int i = 0; i < RecordElements.Count; i++)
                {
                    Console.Write(RecordElements[i] + "\t");
                }
                Console.ResetColor();

                //LEAVE THIS ALONE!!!!
                Console.WriteLine();
            }
        }
        public Database Load_Database()
        {
            Console.WriteLine("Here are the databases on this computer\n");
            string[] DbPaths = AvailblDbPaths();
            for (int NumOfDbs = 0; NumOfDbs < DbPaths.Length; NumOfDbs++)
                Console.WriteLine((NumOfDbs + 1) + ". " + DbPaths[NumOfDbs]);
            Console.WriteLine("\nIf you wouldn’t like to open any of these and would like to return to the main menu, type anything else");
            Console.WriteLine("Pick the menu number of the database that you’d like to load: ");
            string Entry = Console.ReadLine();
            int Choice;
            if (!Int32.TryParse(Entry, out Choice) || DbPaths.Length < Choice || Choice <= 0) return null;
            string path = DbPaths[--Choice];
            return Database.LoadDatabase(path);
        }
        string[] AvailblDbPaths()
        {
            return Directory.GetFiles(Database.FolderPath, "*.bin");
        }
        string GetFileName(string EnteredPath)
        {
            if (!EnteredPath.Contains(".bin")) EnteredPath += ".bin";
            string FilePath = EnteredPath;
            if (EnteredPath.Length < 13 || EnteredPath.Split(new char[] { EnteredPath.ToCharArray()[12] })[0] != Database.FolderPath)//checking if folderpath is included in the entered directory of not
                FilePath = Path.Combine(Database.FolderPath, EnteredPath);
            //FilePath is now in correct format to be used by functions
            Database database;
            try
            {
                database = Database.LoadDatabase(FilePath);
            }
            catch
            {
                return "\\";
            }
            return database.FileName;
        }
        public void Create_Column()
        {
            string HappyWithCreation = "";
            string iName;
            Type itype;
            do
            {
                Console.WriteLine("What would you like to name this field?");
                iName = Console.ReadLine();
                Console.WriteLine(@"Please enter the corresponding menu number for the desired type:
    0 - Text
    1 - Positive Byte
    2 - Byte
    3 - Positive Short Integer
    4 - Short Integer
    5 - Positive Integer
    6 - Integer
    7 - Positive Long Integer
    8 - Long Integer
    9 - Decimal/Fraction
    10 - Long Decimal/Fraction.
    11 - Date-Time");
                while (true)
                {
                    try
                    {
                        itype = Converter.types[Int32.Parse(Console.ReadLine())];

                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input, please enter a valid menu number:");
                    }
                }

                Console.WriteLine("Are you sure you’d like to create a " + TypeName(itype) + " field called " + iName + "? (yes/no)");

                while ((HappyWithCreation = Console.ReadLine()) != "yes" && HappyWithCreation != "no")
                {
                    Console.WriteLine("Enter either ‘yes’ or ‘no’");
                }
            } while (HappyWithCreation == "no");
            TheDatabase.Data.Add(new Column(iName, itype));
            if (TheDatabase.Data.Count > 0)//Database has some columns
            {
                if (TheDatabase.Data[0].Data.Count > 0)//Database isn’t empty
                {
                    Console.Clear();
                    Console.WriteLine("There are existing records in the database for which the new field must be filled out.\nPlease fill out the following information for the records already in the database:\n");
                    List<string> ColHeadings = TheDatabase.sColumnHeadings();   //Displaying column titles
                    for (int t = 0; t < ColHeadings.Count; t++)     //
                        Console.Write(ColHeadings[t] + "\t");       //column titles displayed
                    Console.Write("\n");//next line
                    List<Record> NewColsRecords = new List<Record>();
                    for (int i = 0; i < TheDatabase.Data[0].Data.Count; i++)//for every record, display existent data, then ask for additional data to be added to the new column
                    {
                        List<string> CurrRecord = TheDatabase.sRecord(i);
                        for (int t = 0; t < CurrRecord.Count; t++)
                            Console.Write(CurrRecord[t] + "\t");
                        NewColsRecords.Add(new Record(Converter.StringToByte(Console.ReadLine(), itype), itype));
                    } //after this loop, the list of records for the new column is ready
                    TheDatabase.Data[TheDatabase.Data.Count - 1].Data = NewColsRecords;
                }
            }
            Console.WriteLine("Your new column has now been added to the database!\tPress any key to continue...");
            Console.ReadKey();
            return;
        }

        string TypeName(Type T)
        {
            switch (T.ToString())
            {
                case "string":
                    return "text";
                case "Byte":
                    return "positive byte";
                case "SByte":
                    return "byte";
                case "ushort":
                    return "positive short integer";
                case "short":
                    return "short integer";
                case "uint":
                    return "positive integer";
                case "int":
                    return "integer";
                case "ulong":
                    return "positive long integer";
                case "long":
                    return "long integer";
                case "float":
                    return "decimal";
                case "double":
                    return "long decimal";
                case "DateTime":
                    return "date-time";
                default:
                    return "general";//was thinking we could have a construct which allows a general field which can store many things at once... dont really know the use of it but i have to enter something in the default case otherwise all paths wouldn’t return a value
            }
        }


    }
    #endregion

    #region Column and Record Constructs

    public class Record
    {
        public Type type;
        public byte[] Data;
        public Record()
        {

        }
        public Record(byte[] d, Type t)
        {
            Data = d;
            type = t;
        }
        public Record CopyRecord()
        {
            Record output = new Record();
            output.type = type;
            byte[] temp = new byte[Data.Length];
            Array.Copy(Data, temp, Data.Length);
            output.Data = temp;
            return output;
        }
        public int CompareTo(Record other)
        {
            if (type.Equals(typeof(string)))
            {
                UTF8Encoding encoder = new UTF8Encoding();
                return encoder.GetString(Data, 0, Data.Length - 1).CompareTo(encoder.GetString(other.Data, 0, Data.Length - 1));
            }
            else if (type.Equals(typeof(byte)))
            {
                return Data[0].CompareTo(other.Data[0]);
            }
            else if (type.Equals(typeof(sbyte)))
            {
                return unchecked(((sbyte)Data[0])).CompareTo(unchecked(((sbyte)other.Data[0])));
            }
            else if (type.Equals(typeof(Int16)))
            {
                return BitConverter.ToInt16(Data, 0).CompareTo(BitConverter.ToInt16(other.Data, 0));
            }
            else if (type.Equals(typeof(UInt16)))
            {
                return BitConverter.ToUInt16(Data, 0).CompareTo(BitConverter.ToUInt16(other.Data, 0));
            }
            else if (type.Equals(typeof(Int32)))
            {
                return BitConverter.ToInt32(Data, 0).CompareTo(BitConverter.ToInt32(other.Data, 0));
            }
            else if (type.Equals(typeof(UInt32)))
            {
                return BitConverter.ToUInt32(Data, 0).CompareTo(BitConverter.ToUInt32(other.Data, 0));
            }
            else if (type.Equals(typeof(Int64)))
            {
                return BitConverter.ToInt64(Data, 0).CompareTo(BitConverter.ToInt64(other.Data, 0));
            }
            else if (type.Equals(typeof(UInt64)))
            {
                return BitConverter.ToUInt64(Data, 0).CompareTo(BitConverter.ToUInt64(other.Data, 0));
            }
            else if (type.Equals(typeof(Single)))
            {
                return BitConverter.ToSingle(Data, 0).CompareTo(BitConverter.ToSingle(other.Data, 0));
            }
            else if (type.Equals(typeof(Double)))
            {
                return BitConverter.ToDouble(Data, 0).CompareTo(BitConverter.ToDouble(other.Data, 0));
            }
            else if (type.Equals(typeof(DateTime)))
            {
                return Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", Data[0], Data[1], BitConverter.ToUInt16(new byte[] { Data[2], Data[3] }, 0), Data[4], Data[5], Data[6]))
                    .CompareTo(Convert.ToDateTime(string.Format("{0}/{1}/{2} {3}:{4}:{5}", other.Data[0], other.Data[1], BitConverter.ToUInt16(new byte[] { other.Data[2], other.Data[3] }, 0), other.Data[4], other.Data[5], other.Data[6])));

            }
            return 0;
        }
    }
    public class Column
    {
        public Type type;
        public string Name;
        public List<Record> Data = new List<Record>();
        public Column(string name, Type type)
        {
            this.type = type;
            Name = name;
        }
        public Column()
        {

        }
        public List<int> GenerateSortPattern(SortStyle sortStyle) //Bubble Sort UNTESTED
        {
            List<int> output = new List<int>();
            for (int num = 0; num < Data.Count; num++) output.Add(num);
            bool switched = false;

            for (int endpos = output.Count - 1; endpos >= 1; endpos--)
            {
                for (int pos = 0; pos < endpos - 1; pos++)
                {
                    int res = Data[output[pos]].CompareTo(Data[output[pos + 1]]);
                    if (sortStyle == SortStyle.Descending) res = -res;
                    if (res > 0)
                    {
                        int temp = output[pos];
                        output[pos] = output[endpos];
                        output[endpos] = temp;
                        switched = true;
                    }
                }
                if (!switched) break;
            }
            return output;
        }
        public List<int> GenerateFilterPattern(FilterStyle filterStyle, Record Comparison)
        {
            List<int> output = new List<int>();
            for (int item = 0; item < Data.Count; item++)
            {
                if (Comparison.CompareTo(Data[item]) == 0 && (filterStyle == FilterStyle.Equal
                    || filterStyle == FilterStyle.GreaterThanOrEqual || filterStyle == FilterStyle.LessThanOrEqual))
                    output.Add(item);
                else if (Comparison.CompareTo(Data[item]) > 0 && (filterStyle == FilterStyle.GreaterThan || filterStyle == FilterStyle.GreaterThanOrEqual))
                    output.Add(item);
                else if (Comparison.CompareTo(Data[item]) < 0 && (filterStyle == FilterStyle.LessThan || filterStyle == FilterStyle.LessThanOrEqual))
                    output.Add(item);
            }
            return output;
        }
        public Column CopyColumn()
        {
            Column output = new Column();
            output.type = type;
            output.Name = Name;
            List<Record> temp = new List<Record>();
            foreach (var item in Data)
            {
                temp.Add(item.CopyRecord());
            }
            output.Data = temp;
            return output;
        }
        public Record this[int index]
        {
            get { return Data[index]; }
        }

    }
    #endregion

    #region Database Constructs
    public enum SortStyle { Ascending, Descending };
    public enum FilterStyle { Equal, GreaterThanOrEqual, LessThanOrEqual, GreaterThan, LessThan }
    public class Database
    {
        public List<Column> Data = new List<Column>();
        public byte[] DateOfLastSave;
        public static string FolderPath = @"C:\Users\User\Documents\JAK";
        public string FilePath = "NameOfDatabase.bin";
        public string FileName = "TestFile";
        const byte ETX = 3;  //byte	00000011	ETX end of text
        const ulong FormatID = 18263452859329828488L;

        /// <summary>
        /// Loads a Database from a binary file
        /// </summary>
        /// <param name="FilePath">File Location</param>
        /// <returns></returns>
        public static Database LoadDatabase(string FilePath)
        {
            if (!FilePath.Contains(".bin")) FilePath += ".bin";
            BinaryReader binaryReader = new BinaryReader(new FileStream(Path.Combine(FolderPath, FilePath), FileMode.Open));

            //Checks to see if in correct format
            if (binaryReader.BaseStream.Length < 8) throw new NotValidFormatException("Less than 8 bytes");
            else
            {
                if (binaryReader.ReadUInt64() != FormatID) throw new NotValidFormatException("Not Correct Format ID");
            }
            Database database = new Database();
            byte CCount = binaryReader.ReadByte();
            for (int col = 0; col < CCount; col++)
            {
                Column column = new Column();
                column.type = Converter.ByteToType(binaryReader.ReadByte());
                column.Name = Converter.ByteToString(binaryReader.ReadNextRecord(typeof(String)), typeof(String));
                database.Data.Add(column);
            }
            database.DateOfLastSave = binaryReader.ReadNextRecord(typeof(DateTime));
            database.FileName = Converter.ByteToString(binaryReader.ReadNextRecord(typeof(string)), typeof(string));
            //Reads in and creates the Empty Columns
            while (!binaryReader.EOF())
            {
                for (int col = 0; col < database.Data.Count; col++)
                {
                    database.Data[col].Data.Add(new Record(binaryReader.ReadNextRecord(database.Data[col].type),
                        database.Data[col].type));
                }
            }
            database.FilePath = FilePath;
            binaryReader.Close();
            return database;
        }
        public void SaveDatabase()
        {
            DateOfLastSave = Converter.StringToByte((Convert.ToString(DateTime.Now)), typeof(DateTime));
            BinaryWriter binaryWriter = new BinaryWriter(new FileStream(FilePath, FileMode.Truncate));
            binaryWriter.Write(FormatID);
            binaryWriter.Write((byte)Data.Count);
            foreach (var col in Data)
            {
                binaryWriter.Write(Converter.TypeToByte(col.type));
                binaryWriter.Write(Converter.StringToByte(col.Name, typeof(string)));

            }
            binaryWriter.Write(DateOfLastSave);
            binaryWriter.Write(Converter.StringToByte(FileName, typeof(string)));
            if (Data.Count != 0)
            {
                for (int record = 0; record < Data[0].Data.Count; record++)
                {
                    for (int col = 0; col < Data.Count; col++)
                    {
                        binaryWriter.Write(Data[col].Data[record].Data);
                    }
                }
            }
            binaryWriter.Close();
        }
        public void ApplyPattern(List<int> pattern)
        {
            for (int col = 0; col < Data.Count; col++)
            {
                List<Record> temp = new List<Record>();
                foreach (var item in pattern)
                {
                    temp.Add(Data[col].Data[item]);
                }
                Data[col].Data = temp;
            }
        }
        public Database CopyDatabase()
        {
            Database output = new Database();
            output.DateOfLastSave = DateOfLastSave;
            output.FileName = FileName;
            output.FilePath = FilePath;
            List<Column> temp = new List<Column>();
            foreach (var item in Data)
            {
                temp.Add(item.CopyColumn());
            }
            output.Data = temp;
            return output;
        }

        public List<string> sRecord(int RecordNumber_startingat0)
        {
            int RecordNumber = RecordNumber_startingat0;//is this superfluous?
            List<string> ToReturn = new List<string>();
            for (int ColNum = 0; ColNum < Data.Count; ColNum++)
            {
                if (this[ColNum].Data.Count <= RecordNumber)//why not throw an exception? //new column being created, so no data in the record of that column
                    ToReturn.Add("");//Adding empty string so that indexing of ToReturn doesn’t get messed up
                else
                    ToReturn.Add(Converter.ByteToString(this[ColNum][RecordNumber].Data, this[ColNum].type));
            }
            return ToReturn;
        }
        public List<string> sColumnHeadings()
        {
            List<string> ToReturn = new List<string>();
            for (int i = 0; i < this.Data.Count; i++)
            {
                ToReturn.Add(this.Data[i].Name);
            }
            return ToReturn;
        }

        [Serializable]
        public class NotValidFormatException : Exception
        {
            public NotValidFormatException() { }
            public NotValidFormatException(string message) : base(message) { }
            public NotValidFormatException(string message, Exception inner) : base(message, inner) { }
            protected NotValidFormatException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        public Column this[int index]
        {
            get { return Data[index]; }

        }

    }

    #endregion

    #region BinaryFile Constrcuts

    public static class StreamExtensionMethods
    {
        /// <summary>
        /// Checks to see if EndOfFile
        /// </summary>
        /// <param name="binaryReader">Binary Reader</param>
        /// <returns></returns>
        public static bool EOF(this BinaryReader binaryReader)
        {
            var bs = binaryReader.BaseStream;
            return (bs.Position == bs.Length);
        }
    }
    public static class Converter
    {
        public static Type[] types = new Type[] {
            typeof(String),
            typeof(Byte),
            typeof(SByte),
            typeof(UInt16),
            typeof(Int16),
            typeof(UInt32),
            typeof(Int32),
            typeof(UInt64),
            typeof(Int64),
            typeof(Single),
            typeof(Double),
            typeof(DateTime)};
        public static byte[] ReadNextRecord(this BinaryReader binaryReader, Type type) //ADD MORE DATATYPES
        {
            if (type.Equals(typeof(string)))
            {
                List<byte> output = new List<byte>();
                while (true)
                {
                    byte test = binaryReader.ReadByte();
                    output.Add(test);
                    if (test == 3)
                    {
                        return output.ToArray();
                    }
                }
            }
            else if (type.Equals(typeof(byte)) || type.Equals(typeof(sbyte)))
            {
                return binaryReader.ReadBytes(1);
            }
            else if (type.Equals(typeof(Int16)) || type.Equals(typeof(UInt16)))
            {
                return binaryReader.ReadBytes(2);
            }
            else if (type.Equals(typeof(Int32)) || type.Equals(typeof(UInt32)) || type.Equals(typeof(Single)))
            {
                return binaryReader.ReadBytes(4);
            }
            else if (type.Equals(typeof(DateTime)))
            {
                return binaryReader.ReadBytes(7);
            }
            else if (type.Equals(typeof(Int64)) || type.Equals(typeof(UInt64)) || type.Equals(typeof(Double)))
            {
                return binaryReader.ReadBytes(8);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        public static Type ByteToType(byte B)
        {
            if (B > types.Length - 1 || B < 0) throw new InvalidDataException("Note a type ID");
            return types[B];
        }
        public static Byte TypeToByte(Type T)
        {
            return (byte)Array.IndexOf<Type>(types, T);
        }

        public static string ByteToString(byte[] B, Type type) //ADD MORE DATATYPES
        {
            if (type.Equals(typeof(string)))
            {
                UTF8Encoding encoder = new UTF8Encoding();
                return encoder.GetString(B, 0, B.Length - 1);
            }
            else if (type.Equals(typeof(byte)))
            {
                return B[0].ToString();
            }
            else if (type.Equals(typeof(sbyte)))
            {
                return unchecked(((sbyte)B[0])).ToString();
            }
            else if (type.Equals(typeof(Int16)))
            {
                return BitConverter.ToInt16(B, 0).ToString();
            }
            else if (type.Equals(typeof(UInt16)))
            {
                return BitConverter.ToUInt16(B, 0).ToString();
            }
            else if (type.Equals(typeof(Int32)))
            {
                return BitConverter.ToInt32(B, 0).ToString();
            }
            else if (type.Equals(typeof(UInt32)))
            {
                return BitConverter.ToUInt32(B, 0).ToString();
            }
            else if (type.Equals(typeof(Int64)))
            {
                return BitConverter.ToInt64(B, 0).ToString();
            }
            else if (type.Equals(typeof(UInt64)))
            {
                return BitConverter.ToUInt64(B, 0).ToString();
            }
            else if (type.Equals(typeof(Single)))
            {
                return BitConverter.ToSingle(B, 0).ToString();
            }
            else if (type.Equals(typeof(Double)))
            {
                return BitConverter.ToDouble(B, 0).ToString();
            }
            else if (type.Equals(typeof(DateTime)))
            {
                return string.Format("{0}/{1}/{2} {3}:{4}:{5}", B[0], B[1], BitConverter.ToUInt16(new byte[] { B[2], B[3] }, 0), B[4], B[5], B[6]);

            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static bool TryParseToByte(string s, Type type, out byte[] B)
        {
            try
            {
                B = StringToByte(s, type);
            }
            catch (Exception)
            {
                B = null;
                return false;
            }
            return true;
        }
        public static byte[] StringToByte(string s, Type type) //ADD MORE DATATYPES
        {
            if (type.Equals(typeof(string)))
            {
                UTF8Encoding encoder = new UTF8Encoding();
                return encoder.GetBytes(s).Add(3);
            }
            else if (type.Equals(typeof(byte)))
            {
                return BitConverter.GetBytes(byte.Parse(s));
            }
            else if (type.Equals(typeof(sbyte)))
            {
                return BitConverter.GetBytes(sbyte.Parse(s));
            }
            else if (type.Equals(typeof(Int16)))
            {
                return BitConverter.GetBytes(Int16.Parse(s));
            }
            else if (type.Equals(typeof(UInt16)))
            {
                return BitConverter.GetBytes(UInt16.Parse(s));
            }
            else if (type.Equals(typeof(Int32)))
            {
                return BitConverter.GetBytes(Int32.Parse(s));
            }
            else if (type.Equals(typeof(UInt32)))
            {
                return BitConverter.GetBytes(UInt32.Parse(s));
            }
            else if (type.Equals(typeof(Int64)))
            {
                return BitConverter.GetBytes(Int64.Parse(s));
            }
            else if (type.Equals(typeof(UInt64)))
            {
                return BitConverter.GetBytes(UInt64.Parse(s));
            }
            else if (type.Equals(typeof(Single)))
            {
                return BitConverter.GetBytes(Single.Parse(s));
            }
            else if (type.Equals(typeof(Double)))
            {
                return BitConverter.GetBytes(Double.Parse(s));
            }
            else if (type.Equals(typeof(DateTime)))
            {
                DateTime tester = DateTime.Parse(s);
                byte[] output = new byte[7]{
                    (byte)(tester.Day),
                    (byte)(tester.Month),
                    BitConverter.GetBytes((short)(tester.Year))[0],
                    BitConverter.GetBytes((short)(tester.Year))[1],
                    (byte)(tester.Hour),
                (byte)(tester.Minute),
                (byte)(tester.Second)
                };
                return output;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    #endregion

    #region GeneralPurpose Extension Methods

    public static class CPExtensionMethods
    {
        public static byte[] Add(this byte[] b, byte b2)
        {
            byte[] output = new byte[b.Length + 1];
            b.CopyTo(output, 0);
            output[b.Length] = b2;
            return output;
        }
    }

    #endregion
}
