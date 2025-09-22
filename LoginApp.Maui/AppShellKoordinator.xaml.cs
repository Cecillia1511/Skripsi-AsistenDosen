using LoginApp.Maui.ViewModels;
using System;
using Microsoft.Maui.Controls;

namespace LoginApp.Maui;

public partial class AppShellKoordinator : Shell
{
    public AppShellKoordinator()
    {
        InitializeComponent();
        BindingContext = new AppShellViewModel();
    }
    // Handles taps from the custom flyout template
    private async void OnFlyoutItemTapped(object sender, TappedEventArgs e)
    {
        try
        {
            if (e?.Parameter is string route && !string.IsNullOrWhiteSpace(route))
            {
                // Navigate to the target route (absolute route e.g., //home)
                await GoToAsync(route);

                // Close the flyout after navigation
                FlyoutIsPresented = false;
            }
        }
        catch (Exception ex)
        {
            // Optional: log or handle navigation errors
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
        }
    }
}