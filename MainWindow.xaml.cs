using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Gluon;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string ProcessName = "gmod";
    private static HttpClient _httpClient = new();
    private static ProcessStartInfo _injectorStartInfo = new("bin/injector.exe", $"{ProcessName}.exe bin/gluonapi.dll");
    public MainWindow()
    {
        InitializeComponent();
        using var s = File.OpenRead("GLua.xaml");
        using var reader = new XmlTextReader(s);
        CodeEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
    }
    

    private void TopPanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }
        else
        {
            DragMove();
        }
    }
    
    private void CloseButton_OnClick(object sender, RoutedEventArgs e) => Close();

    private void CollapseButton_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private async void AttachButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            try
            {
                await _httpClient.GetStringAsync("http://127.0.0.1:8887", cts.Token);
                MessageBox.Show("Already attached");
                return;
            } catch (OperationCanceledException) { }
            
            var processes = Process.GetProcessesByName(ProcessName);
            if (processes.Length == 0)
            {
                MessageBox.Show($"Failed to find process {ProcessName}.exe");
                return;
            }
            using var process = Process.Start(_injectorStartInfo);
            if(process == null) return;
            await process.WaitForExitAsync();
            using var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            try
            {
                await _httpClient.GetStringAsync("http://127.0.0.1:8887", cts2.Token);
                MessageBox.Show("Attached");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Failed to attach");
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private async void ExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            var processes = Process.GetProcessesByName(ProcessName);
            if (processes.Length == 0)
            {
                MessageBox.Show($"Open the {ProcessName}.exe");
                return;
            }

            try
            {
                await _httpClient.PostAsync("http://127.0.0.1:8887/execute",
                    new StringContent(CodeEditor.Text, Encoding.ASCII, "text/plain"), cts.Token);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Not attached");
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}