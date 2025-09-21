using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Model;

namespace MauiApp1;

public partial class SpaceObjectView : ContentView
{
    public SpaceObjectView()
        => InitializeComponent();

    private void OnObjectDoubleTapped(object? sender, TappedEventArgs e)
    {
        var obj = BindingContext as SpaceObject;
        Debug.Assert(obj is not null);

        DoubleTapped?.Invoke(obj);
    }

    public event Action<SpaceObject>? DoubleTapped;
}