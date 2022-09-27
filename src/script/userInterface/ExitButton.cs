using Godot;
using System;

public class ExitButton : Button
{
    public override void _Ready()
    {
        
    }
    private void _on_ExitButton_pressed()
    {
        GetTree().Quit();
    }
}



