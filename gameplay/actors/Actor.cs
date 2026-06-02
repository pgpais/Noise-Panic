using Godot;

namespace WeWereDumb.Gameplay.Actors;

public partial class Actor : CharacterBody3D
{
    [Export] Node3D componentsParent;

    public T GetComponent<T>() where T : Node
    {
        foreach (Node child in componentsParent.GetChildren())
        {
            if (child is T t)
            {
                return t;
            }
        }
        return null;
    }
}
