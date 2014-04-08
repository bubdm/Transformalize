﻿#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.Microsoft.System.Web.Razor.Generator;

namespace Transformalize.Libs.RazorEngine.Compilation.CSharp
{
    /// <summary>
    ///     Provides a razor code language that supports the C# language.
    /// </summary>
    public class CSharpRazorCodeLanguage : Microsoft.System.Web.Razor.CSharpRazorCodeLanguage
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance
        /// </summary>
        /// <param name="strictMode">Flag to determine whether strict mode is enabled.</param>
        public CSharpRazorCodeLanguage(bool strictMode)
        {
            StrictMode = strictMode;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets whether strict mode is enabled.
        /// </summary>
        public bool StrictMode { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the code generator.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="rootNamespaceName">Name of the root namespace.</param>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="host">The host.</param>
        /// <returns>
        ///     An instance of <see cref="RazorCodeGenerator" />.
        /// </returns>
        public override RazorCodeGenerator CreateCodeGenerator(string className, string rootNamespaceName, string sourceFileName, Microsoft.System.Web.Razor.RazorEngineHost host)
        {
            return new CSharpRazorCodeGenerator(className, rootNamespaceName, sourceFileName, host, StrictMode);
        }

        #endregion
    }
}