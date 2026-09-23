using System.Collections.ObjectModel;
using System.Windows.Input;
using TwoFactRegressCalc.Infrastructure.Commands.Base;
using TwoFactRegressCalc.Infrastructure.DI.Services.Readers;
using TwoFactRegressCalc.Models;
using TwoFactRegressCalc.ViewModels.Base;

namespace TwoFactRegressCalc.ViewModels;

public class RegressionResultPickerViewModel : ViewModel
{
    public RegressionResultPickerViewModel(IEnumerable<TwoFactorRegressionResult> candidates, PhysicalValue physicalValue)
    {
        Candidates = new ObservableCollection<TwoFactorRegressionResult>(candidates);
        Message = $"Select a regression model for {physicalValue}";
        _selectedResult = Candidates.MinBy(c => c.MaxError);
    }

    public string Message { get; }

    public ObservableCollection<TwoFactorRegressionResult> Candidates { get; }

    private TwoFactorRegressionResult? _selectedResult;
    public TwoFactorRegressionResult? SelectedResult
    {
        get => _selectedResult;
        set => Set(ref _selectedResult, value);
    }

    public bool Confirmed { get; private set; }

    public event EventHandler? RequestClose;

    private ICommand? _okCommand;
    public ICommand OkCommand => _okCommand ??= new LambdaCommand(OnOkExecuted, CanOkExecute);

    private bool CanOkExecute(object p) => SelectedResult is not null;

    private void OnOkExecuted(object p)
    {
        Confirmed = true;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
