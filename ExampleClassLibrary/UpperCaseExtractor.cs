using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//TODO To replicat bug uncomment this line 2 and line 
//using Ionic.Zip;


namespace ExampleClassLibrary
{
    [SqlUserDefinedExtractor(AtomicFileProcessing = true)]
    public class UpperCaseExtractor : IExtractor
    {
        private Encoding _encoding;
        private byte[] _row_delim;
        private char _col_delim;

        public UpperCaseExtractor(Encoding encoding, string row_delim = "\n", char col_delim = ',')
        {
            this._encoding = ((encoding == null) ? Encoding.UTF8 : encoding);
            this._row_delim = this._encoding.GetBytes(row_delim);
            this._col_delim = col_delim;
        }

        public override IEnumerable<IRow> Extract(IUnstructuredReader input, IUpdatableRow output)
        {
            /*The following line instantiates an object from a referenced library but the assembly cannot be found at runtime*/
            //ZipFile z = new ZipFile();
            string line;
            foreach (Stream current in input.Split(_encoding.GetBytes("\n")))
            {
                using (StreamReader streamReader = new StreamReader(current, this._encoding))
                {
                    line = streamReader.ReadToEnd().Trim();
                    string[] parts = line.Split(this._col_delim);
                    var count = 0;
                    foreach (string part in parts)
                    {

                        output.Set<string>(count, part.ToUpper());
                        count++;
                    }
                }
                yield return output.AsReadOnly();
            }
            yield break;
        }
    }
}