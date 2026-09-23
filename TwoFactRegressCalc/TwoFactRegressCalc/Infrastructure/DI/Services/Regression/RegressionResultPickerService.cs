using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.ViewModels;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.Regression;

public class RegressionResultPickerService : IRegressionResultPicker
{
    public TwoFactorRegressionResult Pick(IEnumerable<TwoFactorRegressionResult> candidates, PhysicalValue physicalValue)
    {
        var viewModel = new RegressionResultPickerViewModel(candidates, physicalValue);
        var window = new RegressionResultPickerWindow { DataContext = viewModel };

        if (window.ShowDialog() != true || viewModel.SelectedResult is not { } selectedResult)
            throw new RegressionSelectionCancelledException(physicalValue);

        return selectedResult;
    }
}
