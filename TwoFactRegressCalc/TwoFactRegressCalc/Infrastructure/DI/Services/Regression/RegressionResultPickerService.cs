using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.ViewModels;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

public class RegressionResultPickerService : IRegressionResultPicker
{
    public TwoFactorRegressionResult Pick(IEnumerable<TwoFactorRegressionResult> candidates, string physicalValueLabel)
    {
        var viewModel = new RegressionResultPickerViewModel(candidates, physicalValueLabel);
        var window = new RegressionResultPickerWindow { DataContext = viewModel };

        if (window.ShowDialog() != true || viewModel.SelectedResult is not { } selectedResult)
            throw new RegressionSelectionCancelledException(physicalValueLabel);

        return selectedResult;
    }
}
