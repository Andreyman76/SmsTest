using CommunityToolkit.Mvvm.ComponentModel;
using SmsTest.Wpf.Models;
using SmsTest.Wpf.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SmsTest.Wpf.ViewModels;

public partial class MainWindowViewModel
    : ObservableObject
{
    private readonly IEnvironmentVariablesService _service;

    public ObservableCollection<EnvironmentVariableViewModel> Variables { get; }

    public MainWindowViewModel(
        IEnvironmentVariablesService service)
    {
        _service = service;

        Variables = new ObservableCollection<
            EnvironmentVariableViewModel>(
                service.GetAllVariables()
                .Select(CreateVariableViewModel)
            );
    }

    private EnvironmentVariableViewModel
        CreateVariableViewModel(EnvironmentVariable variable)
    {
        var viewModel =
            new EnvironmentVariableViewModel(
                variable.Name,
                variable.Value,
                variable.Comment ?? string.Empty);

        viewModel.PropertyChanged +=
            VariableOnPropertyChanged;

        return viewModel;
    }

    private void VariableOnPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName !=
            nameof(EnvironmentVariableViewModel.Value))
        {
            return;
        }

        if (sender is not EnvironmentVariableViewModel variable)
        {
            return;
        }

        _service.SetVariable(
            variable.Name,
            variable.Value);
    }
}