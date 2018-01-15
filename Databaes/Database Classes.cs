using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace DatabaseProject
{

    #region Column and Record Constructs

    public class Record
    {
        public Type type;
        public byte[] Data;
        public Record(byte[] d, Type t)
        {
            Data = d;
            type = t;
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
    }
    #endregion

    #region Database Constructs

    public class Database
    {
        public List<Column> Data = new List<Column>();
        public string FilePath = "";
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
            BinaryReader binaryReader = new BinaryReader(new FileStream(FilePath, FileMode.Open));

            //Checks to see if in correct format
            if (binaryReader.BaseStream.Length < 8) throw new NotValidFormatException("Less than 8 bytes");
            else
            {
                if (binaryReader.ReadUInt64() != FormatID) throw new NotValidFormatException("Not Correct Format ID");
            }
            Database database = new Database();
            for (int col = 0; col < binaryReader.ReadByte(); col++)
            {
                Column column = new Column();
                column.type = Converter.ByteToType(binaryReader.ReadByte());
                column.Name = Converter.ByteToString(binaryReader.ReadNextRecord(typeof(String)), typeof(String));
                database.Data.Add(column);
            }
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
            BinaryWriter binaryWriter = new BinaryWriter(new FileStream(FilePath, FileMode.Truncate));
            binaryWriter.Write(FormatID);
            binaryWriter.Write((byte)Data.Count);
            foreach (var col in Data)
            {
                binaryWriter.Write(Converter.TypeToByte(col.type));
                binaryWriter.Write(Converter.StringToByte(col.Name, typeof(string)));

            }
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
            else
            {
                throw new NotImplementedException();
            }
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
