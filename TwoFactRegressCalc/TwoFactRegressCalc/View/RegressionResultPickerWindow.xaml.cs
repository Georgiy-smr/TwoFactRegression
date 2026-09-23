using System.Windows;
using TwoFactRegressCalc.ViewModels;

namespace TwoFactRegressCalc
{
    /// <summary>
    /// Interaction logic for RegressionResultPickerWindow.xaml
    /// </summary>
    public partial class RegressionResultPickerWindow : Window
    {
        public RegressionResultPickerWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is RegressionResultPickerViewModel oldViewModel)
                oldViewModel.RequestClose -= OnRequestClose;
            if (e.NewValue is RegressionResultPickerViewModel newViewModel)
                newViewModel.RequestClose += OnRequestClose;
        }

        private void OnRequestClose(object? sender, EventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
