using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Unchase.OpenAPI.ConnectedService.Common
{
    public static class LoggerHelper
    {
        private static IVsOutputWindowPane pane;

        private static readonly object _syncRoot = new object();

        private static IServiceProvider _provider;

        private static string _name;

        public static void Initialize(Package provider, string name)
        {
            _provider = provider;
            _name = name;
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsOutputWindowPane.OutputString(System.String)")]
        public static void Log(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                if (EnsurePane())
                {
                    pane.OutputString(DateTime.Now.ToString() + ": " + message + Environment.NewLine);
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
            if (pane == null)
            {
                var guid = Guid.NewGuid();
                var output = (IVsOutputWindow)_provider.GetService(typeof(SVsOutputWindow));
                output?.CreatePane(ref guid, _name, 1, 1);
                output?.GetPane(ref guid, out pane);
            }

            return pane != null;
        }
    }
}
