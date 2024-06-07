using System;
using Bravo;
using UnityEngine;
using static ImGuiNET.ImGui;

namespace BravoDebug.MainMenuBar.MenuItems
{
    public class MenuItemLanguages
    {
        public void Render()
        {
            if (LocalizationManager.Current == null) return;
        
            if (BeginMenu("Languages"))
            {
                for (int i = 0; i < LocalizationManager.Current.LanguageCount; i++)
                {
                    Language language = (Language)Enum.GetValues(typeof(Language)).GetValue(i);

                    if (MenuItem(language.ToString()))
                    {
                        LocalizationManager.Current.ChangeLanguageTo(language);
                    }
                }

                EndMenu();
            }
        }
    }
}
