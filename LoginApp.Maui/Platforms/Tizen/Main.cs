using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;

namespace LoginApp.Maui.Platforms.Windows
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MauiWinUIApplication.Start<App>(args);
        }
    }
}
