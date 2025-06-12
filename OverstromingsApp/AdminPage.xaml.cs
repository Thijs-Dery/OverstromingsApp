using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace OverstromingsApp;

public partial class AdminPage : ContentPage
{
    private List<(string Name, string Role)> users = new()
    {
        ("User 1", "Standaard"),
        ("User 2", "Analist"),
        ("User 3", "Standaard"),
        ("User 4", "Admin"),
        ("User 5", "Standaard"),
    };

    public AdminPage()
    {
        InitializeComponent();
        RenderUsers();
    }

    private void RenderUsers()
    {
        UsersStack.Children.Clear();

        foreach (var user in users)
        {
            var nameLabel = new Label { Text = user.Name, WidthRequest = 100 };
            var roleLabel = new Label
            {
                Text = user.Role,
                BackgroundColor = user.Role switch
                {
                    "Admin" => Colors.LightGreen,
                    "Analist" => Colors.Cyan,
                    _ => Colors.LightGray
                },
                Padding = 5,
                WidthRequest = 100,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var deleteBtn = new Button { Text = "❌", WidthRequest = 40 };
            deleteBtn.Clicked += (s, e) =>
            {
                users.Remove(user);
                RenderUsers();
            };

            var row = new HorizontalStackLayout
            {
                Spacing = 10,
                Children = { nameLabel, new Label { Text = "Rol:" }, roleLabel, deleteBtn }
            };

            if (user.Role == "Admin")
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) =>
                {
                    await Navigation.PushAsync(new AdminPage());
                };
                roleLabel.GestureRecognizers.Add(tapGesture);
            }

            UsersStack.Children.Add(row);
        }
    }

    private void OnAddClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
        {
            users.Add((email, "Standaard"));
            EmailEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            RenderUsers();
        }
    }
}

