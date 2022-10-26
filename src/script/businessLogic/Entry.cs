using Godot;
using System;

public class Entry : Node
{
    public override void _Ready()
    {
    #if GODOT_SERVER
        GetTree().ChangeScene("res://src/scene/businessLogic/ServerLobby.tscn");
    #elif GODOT_X11 || GODOT_WINDOWS || GODOT_OSX || GODOT_ANDROID || GODOT_IOS || GODOT_HTML5
        GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
    #else
        GD.Print("Failed to autodetect platform, defaulting to client.");
        GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
    #endif
    }
}
