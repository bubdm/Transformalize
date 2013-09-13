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

using Transformalize.Libs.FileHelpers.Engines;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.Enums
{
    /// <summary>
    ///     Indicates the behavior when the <see cref="FileHelperEngine" /> class found an error.
    /// </summary>
    public enum ErrorMode
    {
        /// <summary>Default value, this simple Rethrow the original exception.</summary>
        ThrowException = 0,

        /// <summary>
        ///     Add an <see cref="ErrorInfo" /> to the array of <see cref="ErrorManager.Errors" />.
        /// </summary>
        SaveAndContinue,

        /// <summary>Simply ignores the exception an continue.</summary>
        IgnoreAndContinue
    }
}