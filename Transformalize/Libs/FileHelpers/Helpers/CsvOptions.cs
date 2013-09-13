#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Text;
using Transformalize.Libs.FileHelpers.Converters;
using Transformalize.Libs.FileHelpers.Engines;
using Transformalize.Libs.FileHelpers.RunTime;

namespace Transformalize.Libs.FileHelpers.Helpers
{
    /// <summary>
    ///     Class used to pass information to the <see cref="FileHelpers.RunTime.CsvClassBuilder" /> and the
    ///     <see
    ///         cref="CsvEngine" />
    /// </summary>
    public sealed class CsvOptions
    {
        private ConvertHelpers.DateTimeConverter mDateConv;
        private string mDateFormat = "dd/MM/yyyy";
        private ConvertHelpers.DecimalConverter mDecimalConv;
        private string mDecimalSeparator = ".";
        private char mDelimiter = ',';
        private ConvertHelpers.DoubleConverter mDoubleConv;
        private Encoding mEncoding = Encoding.Default;
        private string mFieldsPrefix = "Field_";
        private char mHeaderDelimiter = char.MinValue;
        private int mHeaderLines = 1;
        private int mNumberOfFields = -1;
        private string mRecordClassName = string.Empty;
        private string mSampleFileName = string.Empty;
        private ConvertHelpers.SingleConverter mSingleConv;

        /// <summary>Create a Csv Wrapper using the specified number of fields.</summary>
        /// <param name="className">The name of the record class</param>
        /// <param name="delimiter">The delimiter for each field</param>
        /// <param name="numberOfFields">The number of fields of each record</param>
        public CsvOptions(string className, char delimiter, int numberOfFields)
        {
            mRecordClassName = className;
            mDelimiter = delimiter;
            mNumberOfFields = numberOfFields;
        }

        /// <summary>Create a Csv Wrapper using the specified sample file with their headers.</summary>
        /// <param name="className">The name of the record class</param>
        /// <param name="delimiter">The delimiter for each field</param>
        /// <param name="sampleFile">A sample file with a header that contains the names of the fields.</param>
        public CsvOptions(string className, char delimiter, string sampleFile)
        {
            mRecordClassName = className;
            mDelimiter = delimiter;
            mSampleFileName = sampleFile;
        }


        /// <summary>A sample file from where to read the field names and number.</summary>
        public string SampleFileName
        {
            get { return mSampleFileName; }
            set { mSampleFileName = value; }
        }

        /// <summary>The delimiter for each field.</summary>
        public char Delimiter
        {
            get { return mDelimiter; }
            set { mDelimiter = value; }
        }

        /// <summary>The delimiter for each fiel name in the header.</summary>
        public char HeaderDelimiter
        {
            get { return mHeaderDelimiter; }
            set { mHeaderDelimiter = value; }
        }

        /// <summary>The name used for the record class (a valid .NET class).</summary>
        public string RecordClassName
        {
            get { return mRecordClassName; }
            set { mRecordClassName = value; }
        }

        /// <summary>The prefix used when you only specified the number of fields</summary>
        public string FieldsPrefix
        {
            get { return mFieldsPrefix; }
            set { mFieldsPrefix = ClassBuilder.StringToIdentifier(value); }
        }

        /// <summary>The number of fields that the file contains.</summary>
        public int NumberOfFields
        {
            get { return mNumberOfFields; }
            set { mNumberOfFields = value; }
        }

        /// <summary>The number of header lines</summary>
        public int HeaderLines
        {
            get { return mHeaderLines; }
            set { mHeaderLines = value; }
        }

        /// <summary>The DateFormat used to read and write datetime values</summary>
        public string DateFormat
        {
            get { return mDateFormat; }
            set { mDateFormat = value; }
        }

        /// <summary>The Decimal Separator used to read and write doubles, singles and decimal values</summary>
        public string DecimalSeparator
        {
            get { return mDecimalSeparator; }
            set { mDecimalSeparator = value; }
        }


        /// <summary>
        ///     Encoding used when handling the CSV files.
        /// </summary>
        public Encoding Encoding
        {
            get { return mEncoding; }
            set { mEncoding = value; }
        }

        internal string ValueToString(object o)
        {
            if (mDecimalConv == null)
            {
                mDecimalConv = new ConvertHelpers.DecimalConverter(DecimalSeparator);
                mDoubleConv = new ConvertHelpers.DoubleConverter(DecimalSeparator);
                mSingleConv = new ConvertHelpers.SingleConverter(DecimalSeparator);
                mDateConv = new ConvertHelpers.DateTimeConverter(DateFormat);
            }

            if (o == null)
                return string.Empty;
            else if (o is DateTime)
                return mDateConv.FieldToString(o);
            else if (o is Decimal)
                return mDecimalConv.FieldToString(o);
            else if (o is Double)
                return mDoubleConv.FieldToString(o);
            else if (o is Single)
                return mSingleConv.FieldToString(o);
            else
                return o.ToString();
        }
    }
}