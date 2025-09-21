using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1;

public partial class SpaceObjectView : ContentView
{
    public SpaceObjectView()
    {
        InitializeComponent();
        Console.WriteLine(BindingContext is null);
    }

    protected override void OnBindingContextChanged()
    {
        Console.WriteLine(BindingContext is null);
        base.OnBindingContextChanged();
    }

    private void OnObjectDoubleTapped(object? sender, TappedEventArgs e)
    {
        Console.WriteLine("Test");
    }
}