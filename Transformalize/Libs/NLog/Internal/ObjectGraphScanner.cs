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

using System.Collections;
using System.Collections.Generic;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Scans (breadth-first) the object graph following all the edges whose are
    ///     instances have <see cref="NLogConfigurationItemAttribute" /> attached and returns
    ///     all objects implementing a specified interfaces.
    /// </summary>
    internal class ObjectGraphScanner
    {
        /// <summary>
        ///     Finds the objects which have attached <see cref="NLogConfigurationItemAttribute" /> which are reachable
        ///     from any of the given root objects when traversing the object graph over public properties.
        /// </summary>
        /// <typeparam name="T">Type of the objects to return.</typeparam>
        /// <param name="rootObjects">The root objects.</param>
        /// <returns>Ordered list of objects implementing T.</returns>
        public static T[] FindReachableObjects<T>(params object[] rootObjects)
            where T : class
        {
            InternalLogger.Trace("FindReachableObject<{0}>:", typeof (T));
            var result = new List<T>();
            var visitedObjects = new Dictionary<object, int>();

            foreach (var rootObject in rootObjects)
            {
                ScanProperties(result, rootObject, 0, visitedObjects);
            }

            return result.ToArray();
        }

        private static void ScanProperties<T>(List<T> result, object o, int level, Dictionary<object, int> visitedObjects)
            where T : class
        {
            if (o == null)
            {
                return;
            }

            if (!o.GetType().IsDefined(typeof (NLogConfigurationItemAttribute), true))
            {
                return;
            }

            if (visitedObjects.ContainsKey(o))
            {
                return;
            }

            visitedObjects.Add(o, 0);

            var t = o as T;
            if (t != null)
            {
                result.Add(t);
            }

            if (InternalLogger.IsTraceEnabled)
            {
                InternalLogger.Trace("{0}Scanning {1} '{2}'", new string(' ', level), o.GetType().Name, o);
            }

            foreach (var prop in PropertyHelper.GetAllReadableProperties(o.GetType()))
            {
                if (prop.PropertyType.IsPrimitive || prop.PropertyType.IsEnum || prop.PropertyType == typeof (string))
                {
                    continue;
                }

                var value = prop.GetValue(o, null);
                if (value == null)
                {
                    continue;
                }

                var enumerable = value as IEnumerable;
                if (enumerable != null)
                {
                    foreach (var element in enumerable.OfType<object>().ToList())
                    {
                        ScanProperties(result, element, level + 1, visitedObjects);
                    }
                }
                else
                {
                    ScanProperties(result, value, level + 1, visitedObjects);
                }
            }
        }
    }
}