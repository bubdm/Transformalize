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
using System.Xml;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.RunTime
{
    /// <summary>Used to create Fixed Length fields and set their properties.</summary>
    public sealed class FixedFieldBuilder : FieldBuilder
    {
#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private int mFieldLength;

        internal FixedFieldBuilder(string fieldName, int length, Type fieldType) : this(fieldName, length, fieldType.FullName)
        {
        }

        internal FixedFieldBuilder(string fieldName, int length, string fieldType) : base(fieldName, fieldType)
        {
            mFieldLength = length;
        }

        /// <summary>The fixed length of the field.</summary>
        public int FieldLength
        {
            get { return mFieldLength; }
            set { mFieldLength = value; }
        }


#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private AlignMode mAlignMode = AlignMode.Left;

        /// <summary>The align of the field used for write operations.</summary>
        public AlignMode AlignMode
        {
            get { return mAlignMode; }
            set { mAlignMode = value; }
        }

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private char mAlignChar = ' ';

        /// <summary>The align char of the field used for write operations.</summary>
        public char AlignChar
        {
            get { return mAlignChar; }
            set { mAlignChar = value; }
        }

        internal override void AddAttributesCode(AttributesBuilder attbs, NetLanguage lang)
        {
            if (mFieldLength <= 0)
                throw new BadUsageException("The Length of each field must be grater than 0");
            else
                attbs.AddAttribute("FieldFixedLength(" + mFieldLength.ToString() + ")");

            if (mAlignMode != AlignMode.Left)
            {
                if (lang == NetLanguage.CSharp)
                    attbs.AddAttribute("FieldAlign(AlignMode." + mAlignMode.ToString() + ", '" + mAlignChar.ToString() + "')");

                else if (lang == NetLanguage.VbNet)
                    attbs.AddAttribute("FieldAlign(AlignMode." + mAlignMode.ToString() + ", \"" + mAlignChar.ToString() + "\"c)");
            }
        }

        internal override void WriteHeaderAttributes(XmlHelper writer)
        {
            writer.mWriter.WriteStartAttribute("Length", "");
            writer.mWriter.WriteString(mFieldLength.ToString());
            writer.mWriter.WriteEndAttribute();
        }

        internal override void WriteExtraElements(XmlHelper writer)
        {
            writer.WriteElement("AlignMode", AlignMode.ToString(), "Left");
            writer.WriteElement("AlignChar", AlignChar.ToString(), " ");
        }

        internal override void ReadFieldInternal(XmlNode node)
        {
            XmlNode ele;

            ele = node["AlignChar"];
            if (ele != null) AlignChar = ele.InnerText[0];

            ele = node["AlignMode"];
            if (ele != null) AlignMode = (AlignMode) Enum.Parse(typeof (AlignMode), ele.InnerText);
        }
    }
}