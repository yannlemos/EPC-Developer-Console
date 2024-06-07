using System.Collections.Generic;
using System.Linq;
using BravoDatabase;
using ImGuiNET;
using UnityEngine;
using static ImGuiNET.ImGui;

namespace BravoDebug.MainMenuBar.MenuItems
{
    public class MenuItemUtils
    {
        private float _debugTimeScale = 1f;
        
        public void Render()
        {
            if (BeginMenu("Utils"))
            {
                if (!CanOpen()) return;

                RenderSpawnMenu();
                RenderTimescaleMenu();
                RenderExperienceMenu();
                RenderWalletMenu();
                RenderOnboardingMenu();

                EndMenu();
            }
        }

        private void RenderSpawnMenu()
        {
            if (BeginMenu("Spawn"))
            {
                if (BeginMenu("Minions"))
                {
                    for (int i = 0; i < TableOfMinions.CountEntities; i++)
                    {
                        var minion = TableOfMinions.GetEntity(i);
                        
                        if (MenuItem(minion.Name))
                        {
                            MinionSpawnManager.Current.DebugSpawnMinion(minion.Name);
                        }
                    }
                    
                    EndMenu();
                }
                
                if (BeginMenu("Elites"))
                {
                    for (int i = 0; i < TableOfElites.CountEntities; i++)
                    {
                        var elite = TableOfElites.GetEntity(i);

                        if (MenuItem(elite.Name))
                        {
                            EliteManager.Current.SpawnElite(elite.Name);
                        }
                    }
                    EndMenu();
                }
                
                if (BeginMenu("Boss"))
                {
                    for (int i = 0; i < TableOfBosses.CountEntities; i++)
                    {
                        var bossName = TableOfBosses.GetEntity(i).Name;
                        if (MenuItem(bossName))
                        {
                            BossManager.Current.DebugInstantiateBoss(bossName);
                        }
                    }
                    EndMenu();
                }
                
                EndMenu();
            }
        }

        private static void RenderOnboardingMenu()
        {
            if (BeginMenu("Onboarding"))
            {
                List<string> onboardingNames = new();

                for (int i = 0; i < TableOfOnboardings.CountEntities; i++)
                {
                    var onboardingName = TableOfOnboardings.GetEntity(i).f_name;

                    if (onboardingNames.Contains(onboardingName)) continue;

                    onboardingNames.Add(onboardingName);

                    if (MenuItem(onboardingName))
                    {
                        OnboardingManager.Current.TriggerGlobalOnboarding(onboardingName);
                    }
                }

                EndMenu();
            }
        }

        private static void RenderWalletMenu()
        {
            if (BeginMenu("Wallet"))
            {
                if (BeginMenu("Coins"))
                {
                    Text("Capy Coins");
                    SameLine();
                    TextColored(Color.yellow, WalletManager.Current.CurrentCapyCoins.ToString());

                    if (Button("Empty##emptyCoins", new Vector2(100, 50)))
                        WalletManager.Current.SubtractCapyCoins(WalletManager.Current.CurrentCapyCoins);
                    SameLine();
                    if (Button("--##Coin", new Vector2(100, 50))) WalletManager.Current.SubtractCapyCoins(5000);
                    SameLine();
                    if (Button("-##Coin", new Vector2(100, 50))) WalletManager.Current.SubtractCapyCoins(100);

                    SameLine();

                    if (Button("+##Coin", new Vector2(100, 50))) WalletManager.Current.AddCapyCoins(100);
                    SameLine();
                    if (Button("++##Coin", new Vector2(100, 50))) WalletManager.Current.AddCapyCoins(5000);

                    EndMenu();
                }

                if (BeginMenu("Token"))
                {
                    Text("Capy Tokens");
                    SameLine();
                    TextColored(Color.magenta, WalletManager.Current.CurrentCapyTokens.ToString());

                    if (Button("Empty##emptyTokens", new Vector2(100, 50)))
                        WalletManager.Current.SubtractCapyTokens(WalletManager.Current.CurrentCapyTokens);
                    SameLine();
                    if (Button("--##Token", new Vector2(100, 50))) WalletManager.Current.SubtractCapyTokens(10);
                    SameLine();
                    if (Button("-##Token", new Vector2(100, 50))) WalletManager.Current.SubtractCapyTokens(1);

                    SameLine();

                    if (Button("+##Token", new Vector2(100, 50))) WalletManager.Current.AddCapyTokens(1);
                    SameLine();
                    if (Button("++##Token", new Vector2(100, 50))) WalletManager.Current.AddCapyTokens(10);

                    EndMenu();
                }

                EndMenu();
            }
        }

        private static void RenderExperienceMenu()
        {
            if (BeginMenu("Experience"))
            {
                if (MenuItem("Add Total Experience To Next Level"))
                {
                    ExperienceManager.Current.AddTotalExperienceToGoNextLevel();
                }

                PushStyleColor(ImGuiCol.Text, Color.blue);
                if (MenuItem("Gain Blue Orb"))
                {
                    ExperienceManager.Current.GainExperience(ExperienceType.Blue, global::Players.Current.List[0]);
                }

                PopStyleColor();

                PushStyleColor(ImGuiCol.Text, Color.red);
                if (MenuItem("Gain Red Orb"))
                {
                    ExperienceManager.Current.GainExperience(ExperienceType.Red, global::Players.Current.List[0]);
                }

                PopStyleColor();

                PushStyleColor(ImGuiCol.Text, Color.grey);
                if (MenuItem("Gain Silver Orb"))
                {
                    ExperienceManager.Current.GainExperience(ExperienceType.Silver,
                        global::Players.Current.List[0]);
                }

                PopStyleColor();

                PushStyleColor(ImGuiCol.Text, Color.yellow);
                if (MenuItem("Gain Gold Orb"))
                {
                    ExperienceManager.Current.GainExperience(ExperienceType.Gold, global::Players.Current.List[0]);
                }

                PopStyleColor();

                if (MenuItem("Gain Platinum Orb"))
                {
                    ExperienceManager.Current.GainExperience(ExperienceType.Platinum,
                        global::Players.Current.List[0]);
                }

                EndMenu();
            }
        }

        private void RenderTimescaleMenu()
        {
            if (BeginMenu("Timescale"))
            {
                if (SliderFloat("##Timescale", ref _debugTimeScale, 0f, 1f))
                {
                    BravoTimeManager.Current.MMTime.SetTimescaleTo(_debugTimeScale);
                }

                EndMenu();
            }
        }

        private static bool CanOpen()
        {
            if (BravoTimeManager.Current.MMTime == null)
            {
                EndMenu();
                return false;
            }

            if (WalletManager.Current == null)
            {
                EndMenu();
                return false;
            }

            if (ExperienceManager.Current == null)
            {
                EndMenu();
                return false;
            }

            if (OnboardingManager.Current == null)
            {
                EndMenu();
                return false;
            }

            if (BossManager.Current == null)
            {
                EndMenu();
                return false;
            }

            return true;
        }
    }
}