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

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Mock target - useful for testing.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Debug_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Debug/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Debug/Simple/Example.cs" />
    /// </example>
    [Target("Debug")]
    public sealed class DebugTarget : TargetWithLayout
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DebugTarget" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public DebugTarget()
        {
            LastMessage = string.Empty;
            Counter = 0;
        }

        /// <summary>
        ///     Gets the number of times this target has been called.
        /// </summary>
        /// <docgen category='Debugging Options' order='10' />
        public int Counter { get; private set; }

        /// <summary>
        ///     Gets the last message rendered by this target.
        /// </summary>
        /// <docgen category='Debugging Options' order='10' />
        public string LastMessage { get; private set; }

        /// <summary>
        ///     Increases the number of messages.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            Counter++;
            LastMessage = Layout.Render(logEvent);
        }
    }
}