using System.Collections.Generic;
using UnityEngine.Events;

public interface IStorage
{
    UnityEvent<IStorable> OnStore { get; }
    UnityEvent<IStorable> OnRemove { get; }
    Dictionary<StorableType, List<IStorable>> Storables { get; }
}
