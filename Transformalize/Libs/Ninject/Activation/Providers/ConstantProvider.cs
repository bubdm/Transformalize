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

#region Using Directives

#endregion

namespace Transformalize.Libs.Ninject.Activation.Providers
{
    /// <summary>
    ///     A provider that always returns the same constant value.
    /// </summary>
    /// <typeparam name="T">The type of value that is returned.</typeparam>
    public class ConstantProvider<T> : Provider<T>
    {
        /// <summary>
        ///     Initializes a new instance of the ConstantProvider&lt;T&gt; class.
        /// </summary>
        /// <param name="value">The value that the provider should return.</param>
        public ConstantProvider(T value)
        {
            Value = value;
        }

        /// <summary>
        ///     Gets the value that the provider will return.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        ///     Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The constant value this provider returns.</returns>
        protected override T CreateInstance(IContext context)
        {
            return Value;
        }
    }
}