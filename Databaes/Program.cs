using System;
using System.IO;

namespace DatabaseProject
{

    #region Column and Record Constructs

    #endregion

    #region Database Constructs

    public class Database
    {
        //byte	00000010	STX	start of text
        //byte	00000011	ETX end of text

        /// <summary>
        /// Loads a Database from a binary file
        /// </summary>
        /// <param name="FilePath">File Location</param>
        /// <returns></returns>
        public static Database LoadDatabase(string FilePath)
        {

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
    public static class BinaryToRecord
    {
        public static byte[] ReadNextRecord(Type t, BinaryReader b)
        {
            if (t.Equals(typeof(int)))
            {
                return b.ReadBytes(4);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
    #endregion

}
