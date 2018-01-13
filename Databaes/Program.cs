using System.Collections;
using System.Collections.Generic;

namespace DatabaseProject
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }

    public partial class Column<T>
    {
        public List<T> DataPoints { get; set; }
        public Column<T> Clone()
        {
            Column<T> output = new Column<T>();
            foreach (var item in DataPoints)
            {
                output.DataPoints.Add(item);
            }
            return output;
        }



    }
    public partial class Column<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)DataPoints).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)DataPoints).GetEnumerator();
        }

    }

}
