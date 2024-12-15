using UnityEngine;

public interface IStorable
{
    Transform Transform { get; }
    Rigidbody Rigidbody { get; }
    IStorage Storage { get; set; }
    StorableAttributes GetAttributes { get; }
    bool IsStored { get; set; }
    void EnablePhysics();
    void DisablePhysics();
}
