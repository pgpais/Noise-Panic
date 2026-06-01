using Godot;
using System;
using System.Collections.Generic;

namespace WeWereDumb.Core.Pooling;

public abstract class ObjectPool
{
    public string Name { get; protected set; }
    public PackedScene Scene { get; protected set; }
    public int StartingSize { get; protected set; }
    public int CurrentSize { get; protected set; }
    public bool AutoExpand { get; protected set; }

    protected List<IPooledObject> ActiveObjects;
    protected List<IPooledObject> InactiveObjects;

    public abstract Node GetObject();
    public abstract void ReturnObject(Node instance);
}

public class ObjectPool<T> : ObjectPool where T : Node, IPooledObject
{

    public ObjectPool(string name, PackedScene scene, int size = 10, bool autoExpand = true)
    {
        Name = name;
        Scene = scene;
        StartingSize = size;
        AutoExpand = autoExpand;
        ActiveObjects = new List<IPooledObject>(StartingSize);
        InactiveObjects = new List<IPooledObject>(StartingSize);
        for (int i = 0; i < StartingSize; i++)
        {
            CreateNewObject();
        }
    }

    private IPooledObject CreateNewObject()
    {
        var instance = Scene.Instantiate<T>();
        instance.SetPool(this);

        InactiveObjects.Add(instance);
        CurrentSize++;

        return instance;
    }

    public T GetTypedObject()
    {
        bool hasInactiveObject = InactiveObjects.Count > 0;
        if (!hasInactiveObject)
        {
            if (AutoExpand)
            {
                CreateNewObject();
            }
            else
            {
                return null;
            }
        }

        var instance = InactiveObjects[0];


        InactiveObjects.RemoveAt(0);
        ActiveObjects.Add(instance);

        // PrintPoolInfo();

        return instance as T;
    }

    public void ReturnTypedObject(T instance)
    {
        if (!ActiveObjects.Contains(instance))
        {
            GD.PushError($"Object not in pool {Name}");
        }

        var parent = instance.GetParent();
        // Remove from tree, use call deferred
        if (parent != null)
        {
            parent.CallDeferred("remove_child", instance); //TODO: Might be a way to improve
        }

        ActiveObjects.Remove(instance);
        InactiveObjects.Add(instance);

        // PrintPoolInfo();
    }


    public override void ReturnObject(Node instance)
    {
        if (instance is not T typed)
        {
            GD.PushError($"Returned object is not of type {typeof(T).Name} for pool {Name}");
            return;
        }
        ReturnTypedObject(typed);
    }

    public override Node GetObject() => GetTypedObject();

    public void PrintPoolInfo()
    {
        GD.Print("Pool: " + Name);
        GD.Print("Active: " + ActiveObjects.Count);
        GD.Print("Inactive: " + InactiveObjects.Count);
    }
}
