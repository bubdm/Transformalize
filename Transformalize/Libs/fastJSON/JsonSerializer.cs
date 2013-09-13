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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

#if !SILVERLIGHT
#endif

namespace Transformalize.Libs.fastJSON
{
    internal sealed class JSONSerializer
    {
        private StringBuilder _output = new StringBuilder();
        private StringBuilder _before = new StringBuilder();
        private const int MAX_DEPTH = 10;
        private int _currentDepth;
        private readonly Dictionary<string, int> _globalTypes = new Dictionary<string, int>();
        private readonly JSONParameters _params;
        private readonly bool _useEscapedUnicode;

        internal JSONSerializer(JSONParameters param)
        {
            _params = param;
            _useEscapedUnicode = _params.UseEscapedUnicode;
        }

        internal string ConvertToJSON(object obj)
        {
            WriteValue(obj);

            var str = "";
            if (_params.UsingGlobalTypes && _globalTypes != null && _globalTypes.Count > 0)
            {
                var sb = _before;
                sb.Append("\"$types\":{");
                var pendingSeparator = false;
                foreach (var kv in _globalTypes)
                {
                    if (pendingSeparator) sb.Append(',');
                    pendingSeparator = true;
                    sb.Append('\"');
                    sb.Append(kv.Key);
                    sb.Append("\":\"");
                    sb.Append(kv.Value);
                    sb.Append('\"');
                }
                sb.Append("},");
                sb.Append(_output);
                str = sb.ToString();
            }
            else
                str = _output.ToString();

            return str;
        }

        private void WriteValue(object obj)
        {
            if (obj == null || obj is DBNull)
                _output.Append("null");

            else if (obj is string || obj is char)
                WriteString(obj.ToString());

            else if (obj is Guid)
                WriteGuid((Guid) obj);

            else if (obj is bool)
                _output.Append(((bool) obj) ? "true" : "false"); // conform to standard

            else if (
                obj is int || obj is long || obj is double ||
                obj is decimal || obj is float ||
                obj is byte || obj is short ||
                obj is sbyte || obj is ushort ||
                obj is uint || obj is ulong
                )
                _output.Append(((IConvertible) obj).ToString(NumberFormatInfo.InvariantInfo));

            else if (obj is DateTime)
                WriteDateTime((DateTime) obj);

            else if (obj is IDictionary && obj.GetType().IsGenericType &&
                     obj.GetType().GetGenericArguments()[0] == typeof (string))
                WriteStringDictionary((IDictionary) obj);

            else if (obj is IDictionary)
                WriteDictionary((IDictionary) obj);
#if !SILVERLIGHT
            else if (obj is DataSet)
                WriteDataset((DataSet) obj);

            else if (obj is DataTable)
                WriteDataTable((DataTable) obj);
#endif
            else if (obj is byte[])
                WriteBytes((byte[]) obj);

            else if (obj is IEnumerable) // Array || obj is IList || obj is ICollection)
                WriteArray((IEnumerable) obj);

            else if (obj is Enum)
                WriteEnum((Enum) obj);

            else if (JSON.Instance.IsTypeRegistered(obj.GetType()))
                WriteCustom(obj);

            else
                WriteObject(obj);
        }

        private void WriteCustom(object obj)
        {
            Serialize s;
            JSON.Instance._customSerializer.TryGetValue(obj.GetType(), out s);
            WriteStringFast(s(obj));
        }

        private void WriteEnum(Enum e)
        {
            // TODO : optimize enum write
            WriteStringFast(e.ToString());
        }

        private void WriteGuid(Guid g)
        {
            if (_params.UseFastGuid == false)
                WriteStringFast(g.ToString());
            else
                WriteBytes(g.ToByteArray());
        }

        private void WriteBytes(byte[] bytes)
        {
#if !SILVERLIGHT
            WriteStringFast(Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None));
#else
            WriteStringFast(Convert.ToBase64String(bytes, 0, bytes.Length));
#endif
        }

        private void WriteDateTime(DateTime dateTime)
        {
            // datetime format standard : yyyy-MM-dd HH:mm:ss
            var dt = dateTime;
            if (_params.UseUTCDateTime)
                dt = dateTime.ToUniversalTime();

            _output.Append('\"');
            _output.Append(dt.Year.ToString("0000", NumberFormatInfo.InvariantInfo));
            _output.Append('-');
            _output.Append(dt.Month.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append('-');
            _output.Append(dt.Day.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(' ');
            _output.Append(dt.Hour.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(':');
            _output.Append(dt.Minute.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(':');
            _output.Append(dt.Second.ToString("00", NumberFormatInfo.InvariantInfo));

            if (_params.UseUTCDateTime)
                _output.Append('Z');

            _output.Append('\"');
        }

#if !SILVERLIGHT
        private DatasetSchema GetSchema(DataTable ds)
        {
            if (ds == null) return null;

            var m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.TableName;

            foreach (DataColumn c in ds.Columns)
            {
                m.Info.Add(ds.TableName);
                m.Info.Add(c.ColumnName);
                m.Info.Add(c.DataType.ToString());
            }
            // FEATURE : serialize relations and constraints here

            return m;
        }

        private DatasetSchema GetSchema(DataSet ds)
        {
            if (ds == null) return null;

            var m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.DataSetName;

            foreach (DataTable t in ds.Tables)
            {
                foreach (DataColumn c in t.Columns)
                {
                    m.Info.Add(t.TableName);
                    m.Info.Add(c.ColumnName);
                    m.Info.Add(c.DataType.ToString());
                }
            }
            // FEATURE : serialize relations and constraints here

            return m;
        }

        private string GetXmlSchema(DataTable dt)
        {
            using (var writer = new StringWriter())
            {
                dt.WriteXmlSchema(writer);
                return dt.ToString();
            }
        }

        private void WriteDataset(DataSet ds)
        {
            _output.Append('{');
            if (_params.UseExtensions)
            {
                WritePair("$schema", _params.UseOptimizedDatasetSchema ? (object) GetSchema(ds) : ds.GetXmlSchema());
                _output.Append(',');
            }
            var tablesep = false;
            foreach (DataTable table in ds.Tables)
            {
                if (tablesep) _output.Append(',');
                tablesep = true;
                WriteDataTableData(table);
            }
            // end dataset
            _output.Append('}');
        }

        private void WriteDataTableData(DataTable table)
        {
            _output.Append('\"');
            _output.Append(table.TableName);
            _output.Append("\":[");
            var cols = table.Columns;
            var rowseparator = false;
            foreach (DataRow row in table.Rows)
            {
                if (rowseparator) _output.Append(',');
                rowseparator = true;
                _output.Append('[');

                var pendingSeperator = false;
                foreach (DataColumn column in cols)
                {
                    if (pendingSeperator) _output.Append(',');
                    WriteValue(row[column]);
                    pendingSeperator = true;
                }
                _output.Append(']');
            }

            _output.Append(']');
        }

        private void WriteDataTable(DataTable dt)
        {
            _output.Append('{');
            if (_params.UseExtensions)
            {
                WritePair("$schema", _params.UseOptimizedDatasetSchema ? (object) GetSchema(dt) : GetXmlSchema(dt));
                _output.Append(',');
            }

            WriteDataTableData(dt);

            // end datatable
            _output.Append('}');
        }
#endif

        private bool _TypesWritten;

        private void WriteObject(object obj)
        {
            if (_params.UsingGlobalTypes == false)
                _output.Append('{');
            else
            {
                if (_TypesWritten == false)
                {
                    _output.Append('{');
                    _before = _output;
                    _output = new StringBuilder();
                }
                else
                    _output.Append('{');
            }
            _TypesWritten = true;
            _currentDepth++;
            if (_currentDepth > MAX_DEPTH)
                throw new Exception("Serializer encountered maximum depth of " + MAX_DEPTH);


            var map = new Dictionary<string, string>();
            var t = obj.GetType();
            var append = false;
            if (_params.UseExtensions)
            {
                if (_params.UsingGlobalTypes == false)
                    WritePairFast("$type", Reflection.Instance.GetTypeAssemblyName(t));
                else
                {
                    var dt = 0;
                    var ct = Reflection.Instance.GetTypeAssemblyName(t);
                    if (_globalTypes.TryGetValue(ct, out dt) == false)
                    {
                        dt = _globalTypes.Count + 1;
                        _globalTypes.Add(ct, dt);
                    }
                    WritePairFast("$type", dt.ToString());
                }
                append = true;
            }

            var g = Reflection.Instance.GetGetters(t);

            foreach (var p in g)
            {
                var o = p.Getter(obj);
                if ((o == null || o is DBNull) && _params.SerializeNullValues == false)
                {
                    //append = false;
                }
                else
                {
                    if (append)
                        _output.Append(',');

                    WritePair(p.Name, o);
                    if (o != null && _params.UseExtensions)
                    {
                        var tt = o.GetType();
                        if (tt == typeof (Object))
                            map.Add(p.Name, tt.ToString());
                    }
                    append = true;
                }
            }
            if (map.Count > 0 && _params.UseExtensions)
            {
                _output.Append(",\"$map\":");
                WriteStringDictionary(map);
            }
            _currentDepth--;
            _output.Append('}');
            _currentDepth--;
        }

        private void WritePairFast(string name, string value)
        {
            if ((value == null) && _params.SerializeNullValues == false)
                return;
            WriteStringFast(name);

            _output.Append(':');

            WriteStringFast(value);
        }

        private void WritePair(string name, object value)
        {
            if ((value == null || value is DBNull) && _params.SerializeNullValues == false)
                return;
            WriteStringFast(name);

            _output.Append(':');

            WriteValue(value);
        }

        private void WriteArray(IEnumerable array)
        {
            _output.Append('[');

            var pendingSeperator = false;

            foreach (var obj in array)
            {
                if (pendingSeperator) _output.Append(',');

                WriteValue(obj);

                pendingSeperator = true;
            }
            _output.Append(']');
        }

        private void WriteStringDictionary(IDictionary dic)
        {
            _output.Append('{');

            var pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) _output.Append(',');

                WritePair((string) entry.Key, entry.Value);

                pendingSeparator = true;
            }
            _output.Append('}');
        }

        private void WriteDictionary(IDictionary dic)
        {
            _output.Append('[');

            var pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) _output.Append(',');
                _output.Append('{');
                WritePair("k", entry.Key);
                _output.Append(",");
                WritePair("v", entry.Value);
                _output.Append('}');

                pendingSeparator = true;
            }
            _output.Append(']');
        }

        private void WriteStringFast(string s)
        {
            _output.Append('\"');
            _output.Append(s);
            _output.Append('\"');
        }

        private void WriteString(string s)
        {
            _output.Append('\"');

            var runIndex = -1;

            for (var index = 0; index < s.Length; ++index)
            {
                var c = s[index];

                if (_useEscapedUnicode)
                {
                    if (c >= ' ' && c < 128 && c != '\"' && c != '\\')
                    {
                        if (runIndex == -1)
                            runIndex = index;

                        continue;
                    }
                }
                else
                {
                    if (c != '\t' && c != '\n' && c != '\r' && c != '\"' && c != '\\') // && c != ':' && c!=',')
                    {
                        if (runIndex == -1)
                            runIndex = index;

                        continue;
                    }
                }

                if (runIndex != -1)
                {
                    _output.Append(s, runIndex, index - runIndex);
                    runIndex = -1;
                }

                switch (c)
                {
                    case '\t':
                        _output.Append("\\t");
                        break;
                    case '\r':
                        _output.Append("\\r");
                        break;
                    case '\n':
                        _output.Append("\\n");
                        break;
                    case '"':
                    case '\\':
                        _output.Append('\\');
                        _output.Append(c);
                        break;
                    default:
                        if (_useEscapedUnicode)
                        {
                            _output.Append("\\u");
                            _output.Append(((int) c).ToString("X4", NumberFormatInfo.InvariantInfo));
                        }
                        else
                            _output.Append(c);

                        break;
                }
            }

            if (runIndex != -1)
                _output.Append(s, runIndex, s.Length - runIndex);


            _output.Append('\"');
        }
    }
}