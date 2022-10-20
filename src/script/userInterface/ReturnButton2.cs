using Godot;
using System;

public class ReturnButton2 : Button
{
    public override void _Ready()
    {
        
    }
    
    private void _on_ReturnButton2_pressed()
    {
        GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
    }
    
}
