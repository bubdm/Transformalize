﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.CSharp;
using Transformalize.Libs.Microsoft.System.Web.Razor.Generator;
using Transformalize.Libs.Microsoft.System.Web.Razor.Parser;

namespace Transformalize.Libs.Microsoft.System.Web.Razor
{
    /// <summary>
    /// Defines the C# Code Language for Razor
    /// </summary>
    public class CSharpRazorCodeLanguage : RazorCodeLanguage
    {
        private const string CSharpLanguageName = "csharp";

        /// <summary>
        /// Returns the name of the language: "csharp"
        /// </summary>
        public override string LanguageName
        {
            get { return CSharpLanguageName; }
        }

        /// <summary>
        /// Returns the type of the CodeDOM provider for this language
        /// </summary>
        public override Type CodeDomProviderType
        {
            get { return typeof(CSharpCodeProvider); }
        }

        /// <summary>
        /// Constructs a new instance of the code parser for this language
        /// </summary>
        public override ParserBase CreateCodeParser()
        {
            return new CSharpCodeParser();
        }

        /// <summary>
        /// Constructs a new instance of the code generator for this language with the specified settings
        /// </summary>
        public override RazorCodeGenerator CreateCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
        {
            return new CSharpRazorCodeGenerator(className, rootNamespaceName, sourceFileName, host);
        }
    }
}
