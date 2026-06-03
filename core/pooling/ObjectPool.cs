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

    protected Node pooledObjectsParent;

    public abstract Node RetrieveObject();
    public abstract void ReturnObject(Node instance);
}

public class ObjectPool<T> : ObjectPool where T : Node, IPooledObject
{
    protected List<T> ActiveObjects;
    protected List<T> InactiveObjects;

    public ObjectPool(string name, PackedScene scene, Node pooledObjectsParent, int size = 10, bool autoExpand = true)
    {
        Name = name;
        Scene = scene;
        StartingSize = size;
        AutoExpand = autoExpand;
        this.pooledObjectsParent = pooledObjectsParent;

        ActiveObjects = new List<T>(StartingSize);
        InactiveObjects = new List<T>(StartingSize);
        for (int i = 0; i < StartingSize; i++)
        {
            CreateNewObject();
        }
    }

    private IPooledObject CreateNewObject()
    {
        var instance = Scene.Instantiate<T>();
        instance.SetPool(this);
        pooledObjectsParent.AddChild(instance, true);

        instance.OnDespawn();

        InactiveObjects.Add(instance);
        CurrentSize++;

        return instance;
    }

    public override Node RetrieveObject() => RetrieveTypedObject();

    public T RetrieveTypedObject()
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

        instance.OnSpawn();

        // PrintPoolInfo();

        return instance as T;
    }

    public void ReturnTypedObject(T instance)
    {
        ActiveObjects.Remove(instance);
        InactiveObjects.Add(instance);

        instance.OnDespawn();

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


    public void PrintPoolInfo()
    {
        GD.Print("Pool: " + Name);
        GD.Print("Active: " + ActiveObjects.Count);
        GD.Print("Inactive: " + InactiveObjects.Count);
    }
}
