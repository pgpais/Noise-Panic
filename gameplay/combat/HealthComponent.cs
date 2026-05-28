using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : Node
{
	[Signal] public delegate void HealthDepletedEventHandler();
	[Signal] public delegate void HealthChangedEventHandler(int health);

	[Export] public int MaxHealth { get; private set; } = 100;

	public int CurrentHealth { get; private set; }

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
	}

	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;
		if (CurrentHealth <= 0)
		{
			Die();
		}
	}

	public void Heal(int amount)
	{
		CurrentHealth += amount;
	}

	public void Reset()
	{
		CurrentHealth = MaxHealth;
	}

	public void Die()
	{
		CurrentHealth = 0;
		EmitSignal(SignalName.HealthDepleted);
	}
}
