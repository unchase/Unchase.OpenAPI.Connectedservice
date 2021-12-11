using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Unchase.OpenAPI.ConnectedService;

public class Options : DialogPage
{
    [Category("General")]
    [DisplayName("Path to NSwagStudio.exe")]
    [Description("Specify the path to NSwagStudio.exe.")]
    public string PathToNSwagStudioExe { get; set; } = NswagStudioExePath();

    [Category("General")]
    [DisplayName("OpenAPI (Swagger) specification enpoint")]
    [Description("Specify the OpenAPI (Swagger) specification enpoint to diff.")]
    public string OpenApiSpecificationEndpoint { get; set; }

    protected override void OnApply(PageApplyEventArgs e)
    {
        if (!File.Exists(PathToNSwagStudioExe))
        {
            e.ApplyBehavior = ApplyKind.Cancel;
            MessageBox.Show($"The file \"{PathToNSwagStudioExe}\" doesn't exist.", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        if (!string.IsNullOrWhiteSpace(OpenApiSpecificationEndpoint) && !OpenApiSpecificationEndpoint.EndsWith(".json"))
        {
            e.ApplyBehavior = ApplyKind.Cancel;
            MessageBox.Show($"The OpenAPI (Swagger) specification endpoint \"{OpenApiSpecificationEndpoint}\" should ends with \".json\".", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        //todo: тут проверять доступность endpoint'а, если недоступен, то все плохо

        base.OnApply(e);
    }

    internal static string NswagStudioExePath()
    {
        var path = string.Empty;
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\NSwagFile\shell\open\command"))
        {
            if (key != null)
            {
                var commandParts = key
                    .GetValue(null)
                    .ToString()
                    .Split(new[] { '"' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .ToArray();

                if (commandParts.Length == 2)
                    path = commandParts.First();
            }

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                path = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%\\Rico Suter\\NSwagStudio\\NSwagStudio.exe");

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                path = Environment.ExpandEnvironmentVariables("%ProgramW6432%\\Rico Suter\\NSwagStudio\\NSwagStudio.exe");

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                path = string.Empty;

            return path;
        }
    }
}
