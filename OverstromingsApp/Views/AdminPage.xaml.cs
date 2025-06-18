using OverstromingsApp.ViewModels;

namespace OverstromingsApp.Views;

public partial class AdminPage : ContentPage
{
    private readonly UserManagementViewModel _vm;

    public AdminPage()
    {
        InitializeComponent();
        _vm = App.Services.GetRequiredService<UserManagementViewModel>();
        BindingContext = _vm;
    }

    private void OnRoleChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker && picker.BindingContext is Core.Models.User user)
        {
            _ = _vm.UpdateUserRoleAsync(user);
        }
    }

    private void OnSearchCompleted(object sender, EventArgs e)
    {
        _ = _vm.LoadUsersCommand.ExecuteAsync(null);
    }
}
