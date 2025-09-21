using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MauiApp1.Model;

public class SpaceObjectList : IList<SpaceObject>, INotifyCollectionChanged, INotifyPropertyChanged
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly SortedList<DateTime, SpaceObject> _list = [];

    public IEnumerator<SpaceObject> GetEnumerator()
        => _list.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(SpaceObject item)
    {
        _list.Add(item.ArrivalTime, item);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
    }

    public void Clear()
    {
        _list.Clear();

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public bool Contains(SpaceObject item)
        => _list.ContainsValue(item);

    public void CopyTo(SpaceObject[] array, int arrayIndex)
        => _list.Values.CopyTo(array, arrayIndex);

    public bool Remove(SpaceObject item)
    {
        if (_list.Remove(item.ArrivalTime))
        {
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            
            return true;
        }

        return false;
    }

    public int Count => _list.Count;
    public bool IsReadOnly { get; } = false;

    public int IndexOf(SpaceObject item)
        => _list.IndexOfValue(item);

    public void Insert(int index, SpaceObject item)
        => throw new NotSupportedException("Не поддерживается.");

    public void RemoveAt(int index)
        => throw new NotSupportedException("Не поддерживается.");

    public SpaceObject this[int index]
    {
        get => _list.Values[index];
        set => throw new NotSupportedException("Не поддерживается.");
    }
}