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

using System;
using System.Reflection;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Targets
{
    /// <summary>
    ///     Represents an injection target for a <see cref="PropertyInfo" />.
    /// </summary>
    public class PropertyTarget : Target<PropertyInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyTarget" /> class.
        /// </summary>
        /// <param name="site">The property that this target represents.</param>
        public PropertyTarget(PropertyInfo site) : base(site, site)
        {
        }

        /// <summary>
        ///     Gets the name of the target.
        /// </summary>
        public override string Name
        {
            get { return Site.Name; }
        }

        /// <summary>
        ///     Gets the type of the target.
        /// </summary>
        public override Type Type
        {
            get { return Site.PropertyType; }
        }
    }
}