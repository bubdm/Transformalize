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
using System.Linq;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Disposal;
using Transformalize.Libs.Ninject.Infrastructure.Introspection;
using Transformalize.Libs.Ninject.Planning.Bindings;

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Provides a path to register bindings.
    /// </summary>
    public abstract class BindingRoot : DisposableObject, IBindingRoot
    {
        /// <summary>
        ///     Gets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        protected abstract IKernel KernelInstance { get; }

        /// <summary>
        ///     Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to bind.</typeparam>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<T> Bind<T>()
        {
            var service = typeof (T);

            var binding = new Binding(service);
            AddBinding(binding);

            return new BindingBuilder<T>(binding, KernelInstance, service.Format());
        }

        /// <summary>
        ///     Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<T1, T2> Bind<T1, T2>()
        {
            var firstBinding = new Binding(typeof (T1));
            AddBinding(firstBinding);
            AddBinding(new Binding(typeof (T2), firstBinding.BindingConfiguration));
            var servceNames = new[] {typeof (T1).Format(), typeof (T2).Format()};

            return new BindingBuilder<T1, T2>(firstBinding.BindingConfiguration, KernelInstance, string.Join(", ", servceNames));
        }

        /// <summary>
        ///     Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>()
        {
            var firstBinding = new Binding(typeof (T1));
            AddBinding(firstBinding);
            AddBinding(new Binding(typeof (T2), firstBinding.BindingConfiguration));
            AddBinding(new Binding(typeof (T3), firstBinding.BindingConfiguration));
            var servceNames = new[] {typeof (T1).Format(), typeof (T2).Format(), typeof (T3).Format()};

            return new BindingBuilder<T1, T2, T3>(firstBinding.BindingConfiguration, KernelInstance, string.Join(", ", servceNames));
        }

        /// <summary>
        ///     Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <typeparam name="T4">The fourth service to bind.</typeparam>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>()
        {
            var firstBinding = new Binding(typeof (T1));
            AddBinding(firstBinding);
            AddBinding(new Binding(typeof (T2), firstBinding.BindingConfiguration));
            AddBinding(new Binding(typeof (T3), firstBinding.BindingConfiguration));
            AddBinding(new Binding(typeof (T4), firstBinding.BindingConfiguration));
            var servceNames = new[] {typeof (T1).Format(), typeof (T2).Format(), typeof (T3).Format(), typeof (T4).Format()};

            return new BindingBuilder<T1, T2, T3, T4>(firstBinding.BindingConfiguration, KernelInstance, string.Join(", ", servceNames));
        }

        /// <summary>
        ///     Declares a binding for the specified service.
        /// </summary>
        /// <param name="services">The services to bind.</param>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<object> Bind(params Type[] services)
        {
            Ensure.ArgumentNotNull(services, "service");
            if (services.Length == 0)
            {
                throw new ArgumentException("The services must contain at least one type", "services");
            }

            var firstBinding = new Binding(services[0]);
            AddBinding(firstBinding);

            foreach (var service in services.Skip(1))
            {
                AddBinding(new Binding(service, firstBinding.BindingConfiguration));
            }

            return new BindingBuilder<object>(firstBinding, KernelInstance, string.Join(", ", services.Select(service => service.Format()).ToArray()));
        }

        /// <summary>
        ///     Unregisters all bindings for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to unbind.</typeparam>
        public void Unbind<T>()
        {
            Unbind(typeof (T));
        }

        /// <summary>
        ///     Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        public abstract void Unbind(Type service);

        /// <summary>
        ///     Removes any existing bindings for the specified service, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<T1> Rebind<T1>()
        {
            Unbind<T1>();
            return Bind<T1>();
        }

        /// <summary>
        ///     Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2> Rebind<T1, T2>()
        {
            Unbind<T1>();
            Unbind<T2>();
            return Bind<T1, T2>();
        }

        /// <summary>
        ///     Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <typeparam name="T3">The third service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2, T3> Rebind<T1, T2, T3>()
        {
            Unbind<T1>();
            Unbind<T2>();
            Unbind<T3>();
            return Bind<T1, T2, T3>();
        }

        /// <summary>
        ///     Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <typeparam name="T3">The third service to re-bind.</typeparam>
        /// <typeparam name="T4">The fourth service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2, T3, T4> Rebind<T1, T2, T3, T4>()
        {
            Unbind<T1>();
            Unbind<T2>();
            Unbind<T3>();
            Unbind<T4>();
            return Bind<T1, T2, T3, T4>();
        }

        /// <summary>
        ///     Removes any existing bindings for the specified service, and declares a new one.
        /// </summary>
        /// <param name="services">The services to re-bind.</param>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<object> Rebind(params Type[] services)
        {
            foreach (var service in services)
            {
                Unbind(service);
            }

            return Bind(services);
        }

        /// <summary>
        ///     Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public abstract void AddBinding(IBinding binding);

        /// <summary>
        ///     Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        public abstract void RemoveBinding(IBinding binding);
    }
}