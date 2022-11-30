using Godot;
using System;

namespace ReversiFEI
{
	public class Entry : Node
	{
		public override void _Ready()
		{
			if(OS.HasFeature("Server"))
			{
				GD.Print("Server executable detected, initiating server lobby...");
				GetTree().ChangeScene("res://src/scene/userInterface/Lobby.tscn");
					//IMPORTANT:
					//Extensive usage of raw URIs is unavoidable due to the way Godot handles resources,
					//thus their usage shouldn't be considered a code smell.
			}
			else 
			{
				GD.Print("Client executable detected, initiating client...");
				GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
			}
		}
	}
}
