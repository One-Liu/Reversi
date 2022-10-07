using Godot;
using System;

public class ReturnButton : Button
{
    public override void _Ready()
    {
        
    }
    
    private void _on_ReturnButton_pressed()
    {
        GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
    }
    
}
