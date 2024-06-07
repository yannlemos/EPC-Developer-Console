using UnityEngine;
using static ImGuiNET.ImGui;

namespace BravoDebug.MainMenuBar.MenuItems
{
    public class MenuItemImgui
    {
        public void Render()
        {
            if (BeginMenu("Imgui"))
            {
                if (BravoDebugManager.Current.ImguiDemoWindow == false)
                {
                    if (MenuItem("Open Demo Window"))
                    {
                        BravoDebugManager.Current.ImguiDemoWindow = true;
                    }
                }
                else
                {
                    if (MenuItem("Close Demo Window"))
                    {
                        BravoDebugManager.Current.ImguiDemoWindow = false;
                    }
                }

                EndMenu();
            }
        }
    }
}
