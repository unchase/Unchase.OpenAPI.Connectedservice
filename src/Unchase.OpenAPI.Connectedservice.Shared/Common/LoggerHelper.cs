using System;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Unchase.OpenAPI.ConnectedService.Common
{
    public static class LoggerHelper
    {
        private static IVsOutputWindowPane _pane;

        private static IServiceProvider _provider;

        private static string _name;

        public static void Initialize(Package provider, string name)
        {
            _provider = provider;
            _name = name;
        }

        public static void Log(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                if (EnsurePane())
                {
                    _pane.OutputString(DateTime.Now + ": " + message + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public static void Log(Exception ex)
        {
            if (ex != null)
            {
                Log(ex.ToString());
            }
        }

        private static bool EnsurePane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_pane == null)
            {
                var guid = Guid.NewGuid();
                var output = (IVsOutputWindow)_provider.GetService(typeof(SVsOutputWindow));
                output?.CreatePane(ref guid, _name, 1, 1);
                output?.GetPane(ref guid, out _pane);
            }

            return _pane != null;
        }
    }
}