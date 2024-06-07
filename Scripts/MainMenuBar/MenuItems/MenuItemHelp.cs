using ImGuiNET;
using UnityEngine;
using static ImGuiNET.ImGui;

namespace BravoDebug.MainMenuBar.MenuItems
{
    public class MenuItemHelp
    {
        private bool _openPopup = true;
        
        public void Render()
        {
            if (BeginMenu("Help"))
            {
                if (MenuItem("Request Debug..."))
                {
                    BravoDebugManager.Current.Popup = true;
                }
                EndMenu();
            }
        }
    }
}