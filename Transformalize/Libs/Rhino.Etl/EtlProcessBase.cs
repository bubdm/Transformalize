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

using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Libs.Rhino.Etl
{
    /// <summary>
    ///     Base class for etl processes, provider registration and management
    ///     services for the pipeline
    /// </summary>
    /// <typeparam name="TDerived">The type of the derived.</typeparam>
    public class EtlProcessBase<TDerived> : WithLoggingMixin
        where TDerived : EtlProcessBase<TDerived>
    {
        /// <summary>
        ///     Ordered list of the operations in this process that will be added to the
        ///     operations list after the initialization is completed.
        /// </summary>
        private readonly List<IOperation> _lastOperations = new List<IOperation>();

        /// <summary>
        ///     Ordered list of the operations in this process
        /// </summary>
        protected readonly List<IOperation> operations = new List<IOperation>();

        /// <summary>
        ///     Gets the name of this instance
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        ///     Gets or sets whether we are using a transaction
        /// </summary>
        /// <value>True or value.</value>
        public bool UseTransaction { get; set; }

        /// <summary>
        ///     Registers the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public TDerived Register(IOperation operation)
        {
            operation.UseTransaction = UseTransaction;
            operations.Add(operation);
            Debug("Register {0} in {1}", operation.Name, Name);
            return (TDerived) this;
        }

        /// <summary>
        ///     Registers the operation at the end of the operations queue
        /// </summary>
        /// <param name="operation">The operation.</param>
        public TDerived RegisterLast(IOperation operation)
        {
            _lastOperations.Add(operation);
            Debug("RegisterLast {0} in {1}", operation.Name, Name);
            return (TDerived) this;
        }

        /// <summary>
        ///     Merges the last operations to the operations list.
        /// </summary>
        protected void MergeLastOperationsToOperations()
        {
            operations.AddRange(_lastOperations);
        }
    }
}