using Godot;
using System;

public class GoToSignUpButton : Button
{
    public override void _Ready()
    {
        
    }

    private void _on_GoToSignUpButton_pressed()
    {
        GetTree().ChangeScene("res://src/scene/userInterface/SignUp.tscn");
    }
}
