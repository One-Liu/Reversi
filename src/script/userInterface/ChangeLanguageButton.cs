using Godot;
using System;

public class ChangeLanguageButton : Button
{
    public override void _Ready()
    {
        
    }

    private void _on_ChangeLanguageButton_pressed()
    {
        switch(TranslationServer.GetLocale())
        {
            case "en":
                TranslationServer.SetLocale("es");
                break;
            case "es":
                TranslationServer.SetLocale("en");
                break;
            default:
                TranslationServer.SetLocale("en");
                break;
        }
    }
}
