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
using System.Collections.Generic;
using System.IO;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Defines a context for tracking template execution.
    /// </summary>
    public class ExecuteContext
    {
        #region Constructors

        /// <summary>
        ///     Creates a new instance of ExecuteContext with an empty ViewBag.
        /// </summary>
        public ExecuteContext()
        {
            _viewBag = new DynamicViewBag();
        }

        /// <summary>
        ///     Creates a new instance of DynamicViewBag, setting initial values in the ViewBag.
        /// </summary>
        /// <param name="viewBag">The initial view bag data or NULL for an empty ViewBag.</param>
        public ExecuteContext(DynamicViewBag viewBag)
        {
            if (viewBag == null)
                _viewBag = new DynamicViewBag();
            else
                _viewBag = viewBag;
        }

        #endregion

        #region Fields

        private readonly Stack<TemplateWriter> _bodyWriters = new Stack<TemplateWriter>();
        private readonly IDictionary<string, Action> _definedSections = new Dictionary<string, Action>();
        private readonly dynamic _viewBag;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current writer.
        /// </summary>
        //internal TextWriter CurrentWriter { get { return _writers.Peek(); } }
        internal TextWriter CurrentWriter { get; set; }

        /// <summary>
        ///     Gets the viewbag that allows sharing state.
        /// </summary>
        public dynamic ViewBag
        {
            get { return _viewBag; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Defines a section used in layouts.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="action">The delegate action used to write the section at a later stage in the template execution.</param>
        public void DefineSection(string name, Action action)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A name is required to define a section.");

            if (_definedSections.ContainsKey(name))
                throw new ArgumentException("A section has already been defined with name '" + name + "'");

            _definedSections.Add(name, action);
        }

        /// <summary>
        ///     Gets the section delegate.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <returns>The section delegate.</returns>
        public Action GetSectionDelegate(string name)
        {
            if (_definedSections.ContainsKey(name))
                return _definedSections[name];

            return null;
        }

        /// <summary>
        ///     Pops the template writer helper off the stack.
        /// </summary>
        /// <returns>The template writer helper.</returns>
        internal TemplateWriter PopBody()
        {
            return _bodyWriters.Pop();
        }

        /// <summary>
        ///     Pushes the specified template writer helper onto the stack.
        /// </summary>
        /// <param name="bodyWriter">The template writer helper.</param>
        internal void PushBody(TemplateWriter bodyWriter)
        {
            if (bodyWriter == null)
                throw new ArgumentNullException("bodyWriter");

            _bodyWriters.Push(bodyWriter);
        }

        #endregion
    }
}