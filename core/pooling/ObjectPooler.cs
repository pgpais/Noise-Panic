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
		return null;
	}

	public void ReturnObject(string poolName, Node instance)
	{
		if (Pools.ContainsKey(poolName))
		{
			Pools[poolName].ReturnObject(instance);
		}
	}

	public void CreatePool<T>(string poolName, PackedScene scene, int size = 10, bool autoExpand = true) where T : Node, IPooledObject
	{
		if (Pools.ContainsKey(poolName)) return;
		Pools[poolName] = new ObjectPool<T>(poolName, scene, pooledObjectsParent, size, autoExpand);
	}
}
