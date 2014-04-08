﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using Transformalize.Libs.Microsoft.System.Web.Razor.Common;

namespace Transformalize.Libs.Microsoft.System.Web.Razor
{
    /// <summary>
    /// Specifies a Razor directive that is rendered as an attribute on the generated class. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class RazorDirectiveAttribute : Attribute
    {
        private readonly object _typeId = new object();

        public RazorDirectiveAttribute(string name, string value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "name");
            }

            Name = name;
            Value = value;
        }

        public override object TypeId
        {
            get { return _typeId; }
        }

        public string Name { get; private set; }

        public string Value { get; private set; }

        public override bool Equals(object obj)
        {
            RazorDirectiveAttribute attribute = obj as RazorDirectiveAttribute;
            return attribute != null &&
                   Name.Equals(attribute.Name, StringComparison.OrdinalIgnoreCase) &&
                   StringComparer.OrdinalIgnoreCase.Equals(Value, attribute.Value);
        }

        public override int GetHashCode()
        {
            return (StringComparer.OrdinalIgnoreCase.GetHashCode(Name) * 31) +
                   (Value == null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(Value));
        }
    }
}
