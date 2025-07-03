using System;
using Avalonia.Controls;
using RenderBugRepro.ViewModels;

namespace RenderBugRepro.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
        base.OnClosed(e);
    }
}