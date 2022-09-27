using Godot;
using System;

public class LogInButton : Button
{
    public override void _Ready()
    {
        
    }

    private void _on_LogInButton_pressed()
    {
        GetTree().ChangeScene("res://src/scene/userInterface/LogIn.tscn");
    }
}
