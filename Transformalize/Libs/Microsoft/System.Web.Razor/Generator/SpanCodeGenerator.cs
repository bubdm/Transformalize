﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using Transformalize.Libs.Microsoft.System.Web.Razor.Parser.SyntaxTree;

namespace Transformalize.Libs.Microsoft.System.Web.Razor.Generator
{
    public abstract class SpanCodeGenerator : ISpanCodeGenerator
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This class has no instance state")]
        public static readonly ISpanCodeGenerator Null = new NullSpanCodeGenerator();

        public virtual void GenerateCode(Span target, CodeGeneratorContext context)
        {
        }

        public override bool Equals(object obj)
        {
            return (obj as ISpanCodeGenerator) != null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private class NullSpanCodeGenerator : ISpanCodeGenerator
        {
            public void GenerateCode(Span target, CodeGeneratorContext context)
            {
            }

            public override string ToString()
            {
                return "None";
            }
        }
    }
}
