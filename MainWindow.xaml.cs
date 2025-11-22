using System.IO;
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
    public enum AttachStatus
    {
        None,
        Attached,
        Failed
    }
    private static AttachStatus _currentStatus = AttachStatus.None;
    private static AttachStatus _oldStatus = AttachStatus.None;
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

    private void AttachButton_OnClick(object sender, RoutedEventArgs e)
    {
        _currentStatus = AttachStatus.Attached;
        MessageBox.Show(_currentStatus == _oldStatus ? "Already attached" : _currentStatus == AttachStatus.Attached ? "Attached" : "Failed");
        _oldStatus = _currentStatus;
    }

    private void ExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        
    }
}