using System.Linq;
using UnityEngine;
using static ImGuiNET.ImGui;

namespace BravoDebug.MainMenuBar.MenuItems
{
    public class MenuItemViews
    {
        public void Render()
        {
            if (BravoViewManager.Current == null) return;
        
            if (BeginMenu("Views"))
            {
                if (BeginMenu("Open View"))
                {
                    for (int i = 0; i < BravoViewManager.Current.GetViewCount; i++)
                    {
                        GameView view = BravoViewManager.Current.GameViews.ElementAt(i).Key;

                        if (MenuItem(BravoUtils.SplitAtUpperCase(view.ToString())))
                        {
                            BravoViewManager.Current.OpenView(view);
                        }
                    }
                
                    EndMenu();
                }
            
                Separator();

                if (BravoViewManager.Current.HasActiveView)
                {
                    if (MenuItem("Close View"))
                    {
                        BravoViewManager.Current.CloseView();
                    }
                }
            
                EndMenu();
            }
        }
    }
}
