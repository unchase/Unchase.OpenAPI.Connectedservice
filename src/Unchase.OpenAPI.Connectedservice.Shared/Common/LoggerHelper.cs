using System;
using System.Collections.Concurrent;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Unchase.OpenAPI.ConnectedService.Common
{
    public static class LoggerHelper
    {
        private static IVsOutputWindowPane _pane;

        private static IVsOutputWindow _outputService;

        private static string _name;

        private static Lazy<ConcurrentQueue<string>> _msgQueue = new Lazy<ConcurrentQueue<string>>(() => new ConcurrentQueue<string>());

        public static async Task InitializeAsync(AsyncPackage package, string name)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            _outputService = await package.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
            _name = name;
        }

        public static async Task LogAsync(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                string outputmsg = DateTime.Now + ": " + message + Environment.NewLine;
                if (EnsurePane())
                {
                    if (_msgQueue != null && _msgQueue.IsValueCreated)
                    {
                        while (_msgQueue.Value.TryDequeue(out var savedmsg))
                            _pane.OutputString(savedmsg);
                        _msgQueue = null;
                    }
                    _pane.OutputString(outputmsg);
                }
                else
                {
                    // Save the messages until the service is retrieved and pane is created.
                    _msgQueue.Value.Enqueue(outputmsg);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private static readonly string _faultEventName = $"{Constants.ProviderId.Replace('.', '/')}/{nameof(LoggerHelper)}/Log";
        public static void Log(string message)
        {
            // Need to use a separate async method here to work around a analyzer bug, see here:
            //https://github.com/microsoft/vs-threading/issues/993
            ThreadHelper.JoinableTaskFactory.RunAsync(() => LogAsync(message)).FileAndForget(_faultEventName);
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
                // Thread-safe here, because it is ensured above that the thread which sets this field
                // is always the same which executes this method, only the sequence may be different.
                _outputService?.CreatePane(ref guid, _name, 1, 1);
                _outputService?.GetPane(ref guid, out _pane);
            }

            return _pane != null;
        }
    }
}