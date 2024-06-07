using System.Collections.Generic;
using ImGuiNET;

namespace BravoDebug.MainMenuBar.MenuItems
{
    public class MenuItemShops
    {
        public void Render(List<IShopDebug> shops)
        {
            if (ImGui.BeginMenu("Shops"))
            {
                foreach (var t in shops)
                {
                    var shopName = t.GetType().ToString();
                        
                    if (ImGui.MenuItem($"{shopName}", "", t.EnableDebug))
                    {
                        t.EnableDebug = !t.EnableDebug;

                        OpenShopView(shopName);
                    }
                }
            
                ImGui.EndMenu();
            }
        }

        private static void OpenShopView(string menuLabel)
        {
            if (menuLabel.Contains("Capybara"))
            {
                BravoViewManager.Current.OpenView(GameView.CapybaraShop);
            }
            else if (menuLabel.Contains("Upgrade"))
            {
                BravoViewManager.Current.OpenView(GameView.UpgradeShop);
            }
            else if (menuLabel.Contains("Skin"))
            {
                BravoViewManager.Current.OpenView(GameView.SkinShop);
            }
            else if (menuLabel.Contains("Decor"))
            {
                BravoViewManager.Current.OpenView(GameView.DecorShop);
            }
            else if (menuLabel.Contains("Dash"))
            {
                BravoViewManager.Current.OpenView(GameView.DashShop);
            }
        }
    }
}