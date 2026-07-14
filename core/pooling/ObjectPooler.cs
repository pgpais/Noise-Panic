using System.Collections.Generic;
using Godot;

namespace WeWereDumb.Core.Pooling;

public partial class ObjectPooler : Node
{
	public static ObjectPooler Instance { get; private set; }

	Dictionary<string, ObjectPool> Pools;

	private Node3D pooledObjectsParent;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			GD.PrintErr("ObjectPooler already exists");
			QueueFree();
		}

		Pools = new Dictionary<string, ObjectPool>();

		pooledObjectsParent = new Node3D
		{
			Name = "PooledObjectsParent"
		};
		AddChild(pooledObjectsParent, true);
	}

	public T RequestObject<T>(string poolName) where T : Node, IPooledObject
	{
		if (Pools.TryGetValue(poolName, out var pool))
			return pool.RetrieveObject() as T;
		GD.PrintErr("Pool " + poolName + " does not exist");
		return null;
	}

	public T RequestSpecificObject<T>(T instance, string poolName) where T : Node, IPooledObject
	{
		if (Pools.TryGetValue(poolName, out var pool))
			return pool.RetrieveSpecificObject(instance) as T;
		GD.PrintErr("Pool " + poolName + " does not exist");
		return null;
	}

	public void ReturnObject(string poolName, Node instance)
	{
		if (Pools.ContainsKey(poolName))
		{
			Pools[poolName].ReturnObject(instance);
		}
	}

	public ObjectPool CreatePool<T>(string poolName, PackedScene scene, int size = 10, bool autoExpand = true) where T : Node, IPooledObject
	{
		if (Pools.ContainsKey(poolName)) return Pools[poolName];
		Pools[poolName] = new ObjectPool<T>(poolName, scene, pooledObjectsParent, size, autoExpand);

		return Pools[poolName];
	}

	public ObjectPool GetPool(string poolName)
	{
		if (!Pools.ContainsKey(poolName))
			return null;
		return Pools[poolName];
	}
}
