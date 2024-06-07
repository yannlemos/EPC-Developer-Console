using System.Collections.Generic;
using ImGuiNET;

namespace BravoDebug.MainMenuBar.MenuItems
{
    public class MenuItemManagers
    {
        public void Render(List<IManagerDebug> menus)
        {
            if (ImGui.BeginMenu("Managers"))
            {
                foreach (var t in menus)
                {
                    var type = t.GetType();

                    var menuLabel = BravoUtils.SplitAtUpperCase(type.ToString().Replace("Bravo", "").Replace("Manager", ""));

                    if (menuLabel == "Conditions") menuLabel = "Game Infos";
                    if (menuLabel == "Network") menuLabel += "ing";

                
                    if (ImGui.MenuItem($"{menuLabel}", "", t.EnableDebug))
                    {
                        t.EnableDebug = !t.EnableDebug;
                    }
                }

                ImGui.EndMenu();
            }
        }

    }
}