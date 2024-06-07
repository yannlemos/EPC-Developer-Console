using System;
using System.Collections.Generic;
using System.Linq;
using Bravo;
using BravoDatabase;
using ImGuiNET;
using MoreMountains.Tools;
using UnityEngine;
using static ImGuiNET.ImGui;

namespace BravoDebug.Monitors
{
    public class OptionsMonitor
    {
        public bool IsEnabled
        {
            get => _openWindow;
            set => _openWindow = value;
        }

        private static bool _openWindow;

        public void Render()
        {
            if (!IsEnabled) return;
            SetNextWindowPos(BravoUtils.NewVec2NoAlloc(Screen.width - 550f, 28f), ImGuiCond.Appearing);
            SetNextWindowSize(BravoUtils.NewVec2NoAlloc(550f, 500f), ImGuiCond.Appearing);
            SetNextWindowBgAlpha(0.99f);

            if (Begin("Options Monitor", ref _openWindow))
            {
                {
                    TextWrapped("Graphics Options:");
                    Indent();
                    Bullet();
                    TextWrapped($"Resolution: {OptionsManager.Current.CurrentOptions.Graphics.Resolution}");
                    Bullet();
                    TextWrapped($"Full Screen Mode: {OptionsManager.Current.CurrentOptions.Graphics.FullScreenMode}");
                    Bullet();
                    TextWrapped($"Refresh Rate: {OptionsManager.Current.CurrentOptions.Graphics.RefreshRate}");
                    Bullet();
                    TextWrapped($"Vsync: {OptionsManager.Current.CurrentOptions.Graphics.VSync}");
                    Unindent();
                }

                Spacing();

                {
                    TextWrapped("Audio Options:");
                    Indent();
                    Bullet();
                    TextWrapped($"Master Volume: {OptionsManager.Current.CurrentOptions.Audio.MasterVolume}");
                    Bullet();
                    TextWrapped($"Music Volume: {OptionsManager.Current.CurrentOptions.Audio.MusicVolume}");
                    Bullet();
                    TextWrapped($"SFX Volume: {OptionsManager.Current.CurrentOptions.Audio.SFXVolume}");
                    Bullet();
                }

                Spacing();

                {
                    TextWrapped("Accessibility Options: ");
                    Indent();
                    Bullet();
                    TextWrapped($"Accessibility Modifiers: {OptionsManager.Current.CurrentOptions.Accessibility.Modifiers}");
                }

                End();
            }
        }
    }
}