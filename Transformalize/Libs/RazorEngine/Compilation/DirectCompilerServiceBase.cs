﻿#region License

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
using System.CodeDom.Compiler;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Web.Razor;
using System.Web.Razor.Parser;
using Transformalize.Libs.NLog;

namespace Transformalize.Libs.RazorEngine.Compilation
{
    /// <summary>
    ///     Provides a base implementation of a direct compiler service.
    /// </summary>
    public abstract class DirectCompilerServiceBase : CompilerServiceBase, IDisposable
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region Fields

        private readonly CodeDomProvider _codeDomProvider;
        private bool _disposed;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="DirectCompilerServiceBase" />.
        /// </summary>
        /// <param name="codeLanguage">The razor code language.</param>
        /// <param name="codeDomProvider">The code dom provider used to generate code.</param>
        /// <param name="markupParserFactory">The markup parser factory.</param>
        protected DirectCompilerServiceBase(RazorCodeLanguage codeLanguage, CodeDomProvider codeDomProvider, Func<ParserBase> markupParserFactory)
            : base(codeLanguage, markupParserFactory)
        {
            _codeDomProvider = codeDomProvider;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Releases managed resourced used by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Creates the compile results for the specified <see cref="TypeContext" />.
        /// </summary>
        /// <param name="context">The type context.</param>
        /// <returns>The compiler results.</returns>
        [Pure]
        private Tuple<CompilerResults, string> Compile(TypeContext context)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            var compileUnit = GetCodeCompileUnit(context.ClassName, context.TemplateContent, context.Namespaces,
                                                 context.TemplateType, context.ModelType);

            var @params = new CompilerParameters
                              {
                                  GenerateInMemory = true,
                                  GenerateExecutable = false,
                                  IncludeDebugInformation = false,
                                  CompilerOptions = "/target:library /optimize"
                              };

            var assemblies = CompilerServicesUtility
                .GetLoadedAssemblies()
                .Where(a => !a.IsDynamic && File.Exists(a.Location))
                .GroupBy(a => a.GetName().Name).Select(grp => grp.First(y => y.GetName().Version == grp.Max(x => x.GetName().Version))) // only select distinct assemblies based on FullName to avoid loading duplicate assemblies
                .Select(a => a.Location);

            var includeAssemblies = (IncludeAssemblies() ?? Enumerable.Empty<string>());
            assemblies = assemblies.Concat(includeAssemblies)
                                   .Where(a => !string.IsNullOrWhiteSpace(a))
                                   .Distinct(StringComparer.InvariantCultureIgnoreCase);

            @params.ReferencedAssemblies.AddRange(assemblies.ToArray());

            string sourceCode = null;
            if (Debug)
            {
                var builder = new StringBuilder();
                using (var writer = new StringWriter(builder, CultureInfo.InvariantCulture))
                {
                    _codeDomProvider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
                    sourceCode = builder.ToString();
                }
            }

            return Tuple.Create(_codeDomProvider.CompileAssemblyFromDom(@params, compileUnit), sourceCode);
        }

        /// <summary>
        ///     Compiles the type defined in the specified type context.
        /// </summary>
        /// <param name="context">The type context which defines the type to compile.</param>
        /// <returns>The compiled type.</returns>
        [Pure, SecurityCritical]
        public override Tuple<Type, Assembly> CompileType(TypeContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var result = Compile(context);
            var compileResult = result.Item1;

            if (compileResult.Errors != null && compileResult.Errors.Count > 0)
            {
                _log.Warn("The following template content will not compile:");
                _log.Info(Environment.NewLine + context.TemplateContent);
                foreach (var error in compileResult.Errors)
                {
                    _log.Error(error.ToString().Split(':').Last().Trim(' '));
                }
                Environment.Exit(1);
                //throw new TemplateCompilationException(compileResult.Errors, result.Item2, context.TemplateContent);
            }

            return Tuple.Create(
                compileResult.CompiledAssembly.GetType("CompiledRazorTemplates.Dynamic." + context.ClassName),
                compileResult.CompiledAssembly);
        }

        /// <summary>
        ///     Releases managed resources used by this instance.
        /// </summary>
        /// <param name="disposing">Are we explicily disposing of this instance?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _codeDomProvider.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}