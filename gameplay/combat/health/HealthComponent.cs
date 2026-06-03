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
		Reset();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void TakeDamage(int damage)
	{
		if (!IsMultiplayerAuthority()) Rpc(MethodName.TakeDamage, damage);

		// Damage is applied on all instances
		CurrentHealth -= damage;

		if (IsMultiplayerAuthority())
		{
			// Authority holds ground truth
			Rpc(MethodName.SyncHealth, CurrentHealth);

			if (CurrentHealth <= 0)
			{
				Rpc(MethodName.Die);
			}

		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void Heal(int amount)
	{
		if (!IsMultiplayerAuthority()) Rpc(MethodName.Heal, amount);

		// Healing is applied on all instances
		CurrentHealth += amount;

		if (CurrentHealth > MaxHealth)
		{
			CurrentHealth = MaxHealth;
		}

		if (IsMultiplayerAuthority())
		{
			// Authority holds ground truth
			Rpc(MethodName.SyncHealth, CurrentHealth);
		}
	}

	public void Reset()
	{
		CurrentHealth = MaxHealth;
		EmitSignal(SignalName.HealthChanged, CurrentHealth);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void Die()
	{
		CurrentHealth = 0;

		PlayDeathFeedback();

		EmitSignal(SignalName.HealthDepleted);
	}

	private void PlayDeathFeedback()
	{

	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	private void SyncHealth(int health)
	{
		CurrentHealth = health;
		EmitSignal(SignalName.HealthChanged, CurrentHealth);


	}
}
