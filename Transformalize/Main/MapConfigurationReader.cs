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
using Transformalize.Configuration;

namespace Transformalize.Main
{
    public class MapConfigurationReader : IMapReader
    {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly ItemElementCollection _items;
        private readonly string _operator;

        public MapConfigurationReader(ItemElementCollection items, string @operator)
        {
            _items = items;
            _operator = @operator;
        }

        public Map Read()
        {
            var map = new Map();
            foreach (
                var i in
                    _items.Cast<ItemConfigurationElement>().Where(i => i.Operator.Equals(_operator, IC)))
            {
                map[i.From] = new Item(i.Parameter, i.To);
            }
            return map;
        }
    }
}