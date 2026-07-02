using System.Linq;
using Godot;
using Godot.Collections;

namespace Gameplay.AI;

[GlobalClass]
public partial class AiBrain : Node3D
{
	// @entega_: Ingredientes NecessáriosPato: 1 pato inteiro (cerca de 1,5 kg a 2 kg) limpo e cortado em pedaços.Arroz: 400 g de arroz (preferencialmente arroz agulha ou vaporizado para ficar solto).Enchidos: 1 chouriço de carne de boa qualidade e 150 g de bacon fumado.Aromáticos para o caldo: 1 cebola, 1 cenoura, 2 dentes de alho, 2 folhas de louro e 1 raminho de salsa.Refogado: 1 cebola picada, 2 dentes de alho picados e um fio de azeite.Tempero: Sal, pimenta preta e meio cálice de Vinho do Porto

	[Export] Node statesParent;
	Array<AiState> _states = new();

	public override void _Ready()
	{
		base._Ready();

		foreach (var child in statesParent.GetChildren())
		{
			if (child is AiState state)
			{
				GD.Print($"[{Owner.Name} - AiBrain] Adding state {state.Name}");
				_states.Add(state);
			}
		}
	}

	public void Process()
	{
		foreach (var state in _states)
		{
			if (state.CheckConditions())
			{
				state.ExecuteActions();
				return;
			}
		}
	}
}