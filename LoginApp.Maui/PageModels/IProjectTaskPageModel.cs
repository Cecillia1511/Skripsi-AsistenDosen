using CommunityToolkit.Mvvm.Input;
using LoginApp.Maui.Models;

namespace LoginApp.Maui.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}