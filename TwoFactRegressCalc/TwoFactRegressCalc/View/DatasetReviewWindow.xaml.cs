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

        // The view model is created for this window only, so the subscription never needs removing.
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is DatasetReviewViewModel viewModel)
                viewModel.RequestClose += (_, _) =>
                {
                    DialogResult = true;
                    Close();
                };
        }
    }
}
