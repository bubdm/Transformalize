﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;
using Transformalize.Libs.Microsoft.System.Web.Razor.Resources;

namespace Transformalize.Libs.Microsoft.System.Web.Razor.Editor
{
    internal static class RazorEditorTrace
    {
        private static bool? _enabled;

        private static bool IsEnabled()
        {
            if (_enabled == null)
            {
                bool enabled;
                if (Boolean.TryParse(Environment.GetEnvironmentVariable("RAZOR_EDITOR_TRACE"), out enabled))
                {
                    Trace.WriteLine(String.Format(
                        CultureInfo.CurrentCulture,
                        RazorResources.Trace_Startup,
                        enabled ? RazorResources.Trace_Enabled : RazorResources.Trace_Disabled));
                    _enabled = enabled;
                }
                else
                {
                    _enabled = false;
                }
            }
            return _enabled.Value;
        }

        [Conditional("EDITOR_TRACING")]
        public static void TraceLine(string format, params object[] args)
        {
            if (IsEnabled())
            {
                Trace.WriteLine(String.Format(
                    CultureInfo.CurrentCulture,
                    RazorResources.Trace_Format,
                    String.Format(CultureInfo.CurrentCulture, format, args)));
            }
        }
    }
}