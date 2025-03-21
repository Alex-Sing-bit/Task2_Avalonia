using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication4.ViewModels;

namespace AvaloniaApplication4.Views
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            var viewModel = new FileSystemViewModel(this);
            InitializeComponent();
            DataContext = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}