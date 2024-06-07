using System.Collections.Generic;
using BravoDebug.MainMenuBar.MenuItems;
using ImGuiNET;
using UnityEngine;
using static ImGuiNET.ImGui;

namespace BravoDebug.MainMenuBar
{
    public class MainMenuBar
    {
        private readonly MenuItemManagers _menuItemManagers = new();
        private readonly MenuItemShops _menuItemShops = new();
        private readonly MenuItemUtils _menuItemUtils = new();
        private readonly MenuItemViews _menuItemViews = new();
        private readonly MenuItemImgui _menuItemImgui = new();
        private readonly MenuItemLanguages _menuItemLanguages = new();
        private readonly MenuItemHelp _menuItemHelp = new();
        private readonly MenuItemTools _menuItemTools = new();
        
        public void Render(List<IManagerDebug> debugMenus, List<IShopDebug> shopDebugs)
        {
            if (BeginMainMenuBar())
            {
                _menuItemManagers.Render(debugMenus);
                _menuItemShops.Render(shopDebugs);
                _menuItemTools.Render();
                _menuItemUtils.Render();
                _menuItemViews.Render();
                _menuItemLanguages.Render();
                _menuItemImgui.Render();
                _menuItemHelp.Render();
                
                RenderCloseButton();

                EndMainMenuBar();
            }
        }

        private void RenderCloseButton()
        {
            Dummy(new Vector2(GetContentRegionAvail().x - 55, 0f));
            TextColored(Color.grey, "F1");
            SameLine();
            if (Button("X"))
            {
                BravoDebugManager.Current.ToggleMainDebug();
            }
        }
    }
}