using Bravo;
using ImGuiNET;
using UnityEngine;
using static ImGuiNET.ImGui;

namespace BravoDebug.Monitors
{
    public class MainMonitor
    {
        public bool IsEnabled { get; set; }
        
        public void Render()
        {
            if (!CanEnable()) return;
            if (!IsEnabled) return;

            SetNextWindowSize(new Vector2(Screen.width, 100));
            SetNextWindowPos(new Vector2(Screen.width / 2, Screen.height - 50), ImGuiCond.Always,
                new Vector2(0.5f, 0.5f));
            SetNextWindowBgAlpha(0.98f);
            var flags = ImGuiWindowFlags.None;
            flags |= ImGuiWindowFlags.NoResize;

            
            if (Begin("Main Monitor", flags))
            {
                SetWindowFontScale(0.9f);
                Columns(4);
                
                TextColored(Color.grey, "Game Type:");
                SameLine();
                Text($"{GetCurrentGameType()} ({Players.Current.Count})");
                
                TextColored(Color.grey, "Game View:");
                SameLine();
                Text($"{GetCurrentGameView()}");

                NextColumn();
                
                TextColored(Color.grey, "Timescale:");
                SameLine();
                Text($"{Time.timeScale}");
                
                TextColored(Color.grey, "Language:");
                SameLine();
                Text($"{LocalizationManager.Current.CurrentLanguage}");

                NextColumn();
                
                TextColored(Color.grey, "Camera:");
                SameLine();
                var cinemachineVirtualCamera = CameraManager.Current.GetActiveVirtualCamera();
                Text($"{cinemachineVirtualCamera.gameObject.name} ({cinemachineVirtualCamera.Priority})");
                
                TextColored(Color.grey, "Transition:");
                SameLine();

                if (TransitionManager.Current.IsTransitioning)
                {
                    Text(TransitionManager.Current.CurrentTransition);
                }
                else
                {
                    Text("None");
                }

                NextColumn();
                
                TextColored(Color.grey, "Open Scenes:");

                var sceneMonitor = "";

                var sceneNames = BravoSceneManager.Current.GetActiveSceneNames();

                for (var i = 0; i < sceneNames.Count; i++)
                {
                    sceneMonitor += sceneNames[i];

                    if (i < sceneNames.Count - 1 && sceneNames.Count != 1) sceneMonitor += ", ";
                }

                TextWrapped(sceneMonitor);

                End();
            }
        }


        private string GetCurrentGameType()
        {
            if (BravoGameManager.Current == null) return "";

            if (BravoGameManager.Current.IsInSingleplayerGame) return "Single Player";

            if (BravoGameManager.Current.IsInLocalMultiplayerGame) return "Local Multiplayer";

            if (BravoGameManager.Current.IsInOnlineMultiplayerGame) return "Online Multiplayer";

            return "Error";
        }

        private string GetCurrentGameView()
        {
            if (BravoViewManager.Current == null) return "";

            return BravoViewManager.Current.CurrentGameView.ToString();
        }

        public bool CanEnable()
        {
            if (CameraManager.Current == null) return false;
            if (BravoSceneManager.Current == null) return false;
            if (LocalizationManager.Current == null) return false;

            return true;
        }
    }
}