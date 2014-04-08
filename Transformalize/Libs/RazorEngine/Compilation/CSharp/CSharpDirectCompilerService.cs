﻿#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp;
using Microsoft.CSharp.RuntimeBinder;
using Transformalize.Libs.Microsoft.System.Web.Razor.Parser;

namespace Transformalize.Libs.RazorEngine.Compilation.CSharp
{
    /// <summary>
    ///     Defines a direct compiler service for the C# syntax.
    /// </summary>
    public class CSharpDirectCompilerService : DirectCompilerServiceBase
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="CSharpDirectCompilerService" />.
        /// </summary>
        /// <param name="strictMode">Specifies whether the strict mode parsing is enabled.</param>
        /// <param name="markupParserFactory">The markup parser factory to use.</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"), SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in base class: DirectCompilerServiceBase")]
        public CSharpDirectCompilerService(bool strictMode = true, Func<ParserBase> markupParserFactory = null)
            : base(
                new CSharpRazorCodeLanguage(strictMode),
                new CSharpCodeProvider(),
                markupParserFactory)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns a set of assemblies that must be referenced by the compiled template.
        /// </summary>
        /// <returns>The set of assemblies.</returns>
        public override IEnumerable<string> IncludeAssemblies()
        {
            // Ensure the Microsoft.CSharp assembly is referenced to support dynamic typing.
            return new[] {typeof (Binder).Assembly.Location};
        }

        #endregion
    }
}