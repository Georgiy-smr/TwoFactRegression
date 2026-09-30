using System.Windows;
using TwoFactRegressCalc.ViewModels;

namespace TwoFactRegressCalc
{
    public partial class DatasetReviewWindow : Window
    {
        public DatasetReviewWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is DatasetReviewViewModel oldViewModel)
                oldViewModel.RequestClose -= OnRequestClose;
            if (e.NewValue is DatasetReviewViewModel newViewModel)
                newViewModel.RequestClose += OnRequestClose;
        }

        private void OnRequestClose(object? sender, EventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
