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
using System.Collections.Generic;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Parameters;

namespace Transformalize.Libs.Ninject.Planning.Bindings
{
    /// <summary>
    ///     The configuration of a binding.
    /// </summary>
    public interface IBindingConfiguration
    {
        /// <summary>
        ///     Gets the binding's metadata.
        /// </summary>
        IBindingMetadata Metadata { get; }

        /// <summary>
        ///     Gets or sets the type of target for the binding.
        /// </summary>
        BindingTarget Target { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the binding was implicitly registered.
        /// </summary>
        bool IsImplicit { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the binding has a condition associated with it.
        /// </summary>
        bool IsConditional { get; }

        /// <summary>
        ///     Gets or sets the condition defined for the binding.
        /// </summary>
        Func<IRequest, bool> Condition { get; set; }

        /// <summary>
        ///     Gets or sets the callback that returns the provider that should be used by the binding.
        /// </summary>
        Func<IContext, IProvider> ProviderCallback { get; set; }

        /// <summary>
        ///     Gets or sets the callback that returns the object that will act as the binding's scope.
        /// </summary>
        Func<IContext, object> ScopeCallback { get; set; }

        /// <summary>
        ///     Gets the parameters defined for the binding.
        /// </summary>
        ICollection<IParameter> Parameters { get; }

        /// <summary>
        ///     Gets the actions that should be called after instances are activated via the binding.
        /// </summary>
        ICollection<Action<IContext, object>> ActivationActions { get; }

        /// <summary>
        ///     Gets the actions that should be called before instances are deactivated via the binding.
        /// </summary>
        ICollection<Action<IContext, object>> DeactivationActions { get; }

        /// <summary>
        ///     Gets the provider for the binding.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The provider to use.</returns>
        IProvider GetProvider(IContext context);

        /// <summary>
        ///     Gets the scope for the binding, if any.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     The object that will act as the scope, or <see langword="null" /> if the service is transient.
        /// </returns>
        object GetScope(IContext context);

        /// <summary>
        ///     Determines whether the specified request satisfies the condition defined on the binding,
        ///     if one was defined.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     <c>True</c> if the request satisfies the condition; otherwise <c>false</c>.
        /// </returns>
        bool Matches(IRequest request);
    }
}