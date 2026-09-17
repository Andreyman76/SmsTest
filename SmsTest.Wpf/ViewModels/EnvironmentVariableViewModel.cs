using CommunityToolkit.Mvvm.ComponentModel;

namespace SmsTest.Wpf.ViewModels;

public partial class EnvironmentVariableViewModel(
    string name,
    string value,
    string comment) : ObservableObject
{
    public string Name { get; } = name;

    [ObservableProperty]
    private string value = value;

    public string Comment { get; } = comment;
}