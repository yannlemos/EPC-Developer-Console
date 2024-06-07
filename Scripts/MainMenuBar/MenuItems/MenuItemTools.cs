using ImGuiNET;
using UnityEngine;

namespace BravoDebug.MainMenuBar.MenuItems
{
    public class MenuItemTools
    {
        public void Render()
        {
            if (ImGui.BeginMenu("Tools"))
            {
                if (ImGui.MenuItem("Players Monitor", BravoDebugManager.Current.PlayersMonitorShortcut.ToString(), BravoDebugManager.Current.PlayersMonitorIsActive))
                {
                    BravoDebugManager.Current.TogglePlayersMonitor();
                }
                
                if (ImGui.MenuItem("Arena Monitor", BravoDebugManager.Current.ArenaMonitorShortcut.ToString(), BravoDebugManager.Current.ArenaMonitorIsActive))
                {
                    BravoDebugManager.Current.ToggleArenaMonitor();
                }
                
                if (ImGui.MenuItem("Main Monitor", BravoDebugManager.Current.MainMonitorShortcut.ToString(), BravoDebugManager.Current.MainMonitorIsActive, BravoDebugManager.Current.MainMonitorCanBeActivated))
                {
                    BravoDebugManager.Current.ToggleMainMonitor();
                }

                if (ImGui.MenuItem("Options Monitor", "-", BravoDebugManager.Current.OptionsMonitorIsActive))
                {
                    BravoDebugManager.Current.ToggleOptionsMonitor();
                }
                
                if (ImGui.MenuItem("Database", BravoDebugManager.Current.RuntimeDatabaseShortcut.ToString(), BravoDebugManager.Current.DatabaseIsActive, true))
                {
                    BravoDebugManager.Current.ToggleDatabase();
                }
                
                if (ImGui.MenuItem("Hierarchy/Inspector", BravoDebugManager.Current.RuntimeHierarchyAndInspectorShortcut.ToString(), BravoDebugManager.Current.HierarchyIsActive, true))
                {
                    BravoDebugManager.Current.ToggleHierarchyAndInspectorDebug();
                }
                
                if (ImGui.MenuItem("Console", BravoDebugManager.Current.RuntimeConsoleShortcut.ToString(), BravoDebugManager.Current.ConsoleIsActive, true))
                {
                    BravoDebugManager.Current.ToggleConsoleDebug();
                }
                
                ImGui.EndMenu();
            }
        }
    }
}