using System;
using System.Collections.Generic;
using System.Linq;
using BravoDatabase;
using ImGuiNET;
using MoreMountains.Tools;
using UnityEngine;
using static ImGuiNET.ImGui;
using Object = UnityEngine.Object;

namespace BravoDebug.Monitors
{
    public class ArenaMonitor
    {
        public bool IsEnabled
        {
            get => _openWindow;
            set => _openWindow = value;
        }

        private static bool _openWindow;

        private Dictionary<string, int> _enemyNamesToCountInArena = new();
        private int _debugArenaStartTime = 900;

        public void Render()
        {
            if (!IsEnabled) return;

            SetNextWindowPos(BravoUtils.NewVec2NoAlloc(Screen.width - 400f, 28f), ImGuiCond.Appearing);
            SetNextWindowSize(BravoUtils.NewVec2NoAlloc(400f, 956f), ImGuiCond.Appearing);
            SetNextWindowBgAlpha(0.99f);

            if (Begin("Arena Monitor", ref _openWindow))
            {
                var globalButtonSize = BravoUtils.NewVec2NoAlloc(GetContentRegionAvail().x, 30f);

                if (!ArenaRuntimeManager.Current.HasStarted)
                {
                    RenderArenaSetup(globalButtonSize);
                }
                
                Spacing();

                Color tabColor;
                Color idleColor;
                Color hoveredColor;
                Color activeColor;

                if (BeginTabBar("Arena Managers"))
                {
                    tabColor = TableOfColors.e_PrimaryAbilities.f_Color;

                    idleColor = new Color(tabColor.r, tabColor.g, tabColor.b, tabColor.a / 2f);
                    hoveredColor = new Color(tabColor.r, tabColor.g, tabColor.b, tabColor.a / 4f);
                    activeColor = tabColor;

                    PushStyleColor(ImGuiCol.Tab, idleColor);
                    PushStyleColor(ImGuiCol.TabHovered, hoveredColor);
                    PushStyleColor(ImGuiCol.TabActive, activeColor);
                    PushStyleColor(ImGuiCol.Header, idleColor);
                    PushStyleColor(ImGuiCol.HeaderHovered, hoveredColor);
                    PushStyleColor(ImGuiCol.HeaderActive, activeColor);
                    PushStyleColor(ImGuiCol.TableHeaderBg, idleColor);
                    
                    if (BeginTabItem("Settings"))
                    {
                        RenderArenaSettings();
                        EndTabItem();
                    }
                    
                    PopStyleColor(7);

                    tabColor = TableOfColors.e_ActiveAbilities.f_Color;

                    idleColor = new Color(tabColor.r, tabColor.g, tabColor.b, tabColor.a / 2f);
                    hoveredColor = new Color(tabColor.r, tabColor.g, tabColor.b, tabColor.a / 4f);
                    activeColor = tabColor;

                    PushStyleColor(ImGuiCol.Tab, idleColor);
                    PushStyleColor(ImGuiCol.TabHovered, hoveredColor);
                    PushStyleColor(ImGuiCol.TabActive, activeColor);
                    PushStyleColor(ImGuiCol.Header, idleColor);
                    PushStyleColor(ImGuiCol.HeaderHovered, hoveredColor);
                    PushStyleColor(ImGuiCol.HeaderActive, activeColor);
                    PushStyleColor(ImGuiCol.TableHeaderBg, idleColor);

                    
                    if (BeginTabItem("Runtime"))
                    {
                        RenderArenaRuntime();
                        EndTabItem();
                    }
                    
                    PopStyleColor(7);
                    
                    tabColor = TableOfColors.e_DashAbilities.f_Color;

                    idleColor = new Color(tabColor.r, tabColor.g, tabColor.b, tabColor.a / 2f);
                    hoveredColor = new Color(tabColor.r, tabColor.g, tabColor.b, tabColor.a / 4f);
                    activeColor = tabColor;

                    PushStyleColor(ImGuiCol.Tab, idleColor);
                    PushStyleColor(ImGuiCol.TabHovered, hoveredColor);
                    PushStyleColor(ImGuiCol.TabActive, activeColor);
                    PushStyleColor(ImGuiCol.Header, idleColor);
                    PushStyleColor(ImGuiCol.HeaderHovered, hoveredColor);
                    PushStyleColor(ImGuiCol.HeaderActive, activeColor);
                    PushStyleColor(ImGuiCol.TableHeaderBg, idleColor);

                    if (BeginTabItem("Results"))
                    {
                        RenderArenaResults();
                        EndTabItem();
                    }
                    
                    PopStyleColor(7);
                    
                    tabColor = TableOfColors.e_Confirm.f_Color;

                    idleColor = new Color(tabColor.r, tabColor.g, tabColor.b, tabColor.a / 2f);
                    hoveredColor = new Color(tabColor.r, tabColor.g, tabColor.b, tabColor.a / 4f);
                    activeColor = tabColor;

                    PushStyleColor(ImGuiCol.Tab, idleColor);
                    PushStyleColor(ImGuiCol.TabHovered, hoveredColor);
                    PushStyleColor(ImGuiCol.TabActive, activeColor);
                    PushStyleColor(ImGuiCol.Header, idleColor);
                    PushStyleColor(ImGuiCol.HeaderHovered, hoveredColor);
                    PushStyleColor(ImGuiCol.HeaderActive, activeColor);
                    PushStyleColor(ImGuiCol.TableHeaderBg, idleColor);


                    if (BeginTabItem("Events"))
                    {
                        RenderArenaEvents();
                        EndTabItem();
                    }
                    
                    PopStyleColor(7);
                    
                    EndTabBar();
                    
                }
                
                Spacing();
                Separator();
                Spacing();

                RenderArenaUtils();

                End();
            }
        }

        private void RenderArenaUtils()
        {
            if (!ArenaRuntimeManager.Current.HasStarted) return;
            
            var globalButtonSize = BravoUtils.NewVec2NoAlloc(GetContentRegionAvail().x, 30f);

            TextWrapped("Global Utils");
            
            if (Button("Spawn Chest", globalButtonSize)) ChestManager.Current.SpawnChest();
            if (Button("Spawn Blessing", globalButtonSize)) BlessingsManager.Current.SpawnBlessing();
            if (Button("Spawn Boss (Full Health)", globalButtonSize))
            {
                BossManager.Current.DebugState = false;
                ArenaRuntimeManager.Current.GlobalTimerSpeedMultiplier = 9999f;
            }

            if (Button("Spawn Boss (One Hit Kill)", globalButtonSize))
            {
                BossManager.Current.DebugState = true;
                ArenaRuntimeManager.Current.GlobalTimerSpeedMultiplier = 9999f;
            }
            
            TextWrapped("Minion Speed"); SameLine();
            float minionVelocity = MinionMovementManager.Current.GlobalVelocityMultiplier;
            SetNextItemWidth(GetContentRegionAvail().x);
            if (SliderFloat("##MinionSpeed", ref minionVelocity, 0, 2))
            {
                MinionMovementManager.Current.GlobalVelocityMultiplier = minionVelocity;
            }
        }

        private static void RenderArenaEvents()
        {
            List<ArenaEventsController> arenaEventsControllers = GameObject.FindObjectsOfType(typeof(MonoBehaviour))
                    .OfType<ArenaEventsController>().ToList();

            foreach (var t in arenaEventsControllers)
            {
                var arenaEventsController = t;
                    
                if (TreeNode(arenaEventsController.name))
                {
                    Unindent();
                    foreach (var arenaEvent in t.ArenaEvents)
                    {
                        TextWrapped(arenaEvent.Label);
                        
                        if (BeginTable($"##Table{arenaEvent.Label}", 2, ImGuiTableFlags.Borders))
                        {
                            BulletText(arenaEvent.Label);
                            TableHeadersRow();
                            TableSetColumnIndex(0);
                            TextWrapped("Info");
                            TableSetColumnIndex(1);
                            TextWrapped("Value");
                            TableNextRow();
                            TableSetColumnIndex(0);
                            TextWrapped("Mode");
                            TableSetColumnIndex(1);
                            TextWrapped(arenaEvent.Mode.ToString());
                            TableNextRow();

                            switch (arenaEvent.Mode)
                            {
                                case ArenaEventTriggerMode.Timestamp:
                                    TableSetColumnIndex(0);
                                    TextWrapped("Timestamp");
                                    TableSetColumnIndex(1);
                                    TextWrapped(arenaEvent.Timestamp.ToString());
                                    break;
                                case ArenaEventTriggerMode.TimerOperation:
                                    TableSetColumnIndex(0);
                                    TextWrapped("Operation");
                                    TableSetColumnIndex(1);
                                    TextWrapped(arenaEvent.Operation.ToString());
                                    break;
                            }
                                
                            TableNextRow();
                                
                            TableSetColumnIndex(0);
                            TextWrapped("Enabled");
                            TableSetColumnIndex(1);
                            RadioButton($"##{arenaEvent.Label}Enabled", arenaEvent.Enabled);
                            TableNextRow();
                            TableSetColumnIndex(0);
                            TextWrapped("Has Triggered");
                            TableSetColumnIndex(1);
                            RadioButton($"##{arenaEvent.Label}HasTriggered", arenaEvent.HasTriggered);
                            TableNextRow();

                            EndTable();
                        }
                    }

                    TreePop();
                    Indent();
                }
                Separator();
            }
        }

        private void RenderArenaSettings()
        {
            var settings = ArenaSettingsManager.Current;
            var difficulty = DifficultyManager.Current;
            
            TextWrapped($"Name: {settings.ArenaName}");
            TextWrapped($"Bounds: {settings.ArenaBounds}");
            TextWrapped($"Meta Info: {settings.ArenaMetaInfoTileMap}");

            if (CollapsingHeader("Frenzy"))
            {
                TextWrapped($"Total Frenzy: {difficulty.TotalDifficultyLevel}");
            
                // foreach (var frenzy in difficulty.DifficultySettings)
                // {
                //     TextWrapped($"{frenzy.Name} - R{frenzy.}");
                // }

                foreach (var modif in difficulty.DifficultySettings.DifficultyModifiers)
                {
                    TextWrapped($"{modif.Name} - R{modif.Rank}");
                }
            }

            RenderSettingsListenersSubmonitor();
        }

        private void RenderSettingsListenersSubmonitor()
        {
            if (CollapsingHeader("Listeners"))
            {
                RenderListeners("On Confirm Arena", ArenaSettingsManager.Current.OnConfirmArena.GetInvocationList());
                RenderListeners("On Cancel Arena", ArenaSettingsManager.Current.OnCancelArena.GetInvocationList());
            }
        }

        private void RenderArenaRuntime()
        {
            TextWrapped($"Has Started: {ArenaRuntimeManager.Current.HasStarted}");

            var currentArenaTimer = ArenaRuntimeManager.Current.ArenaTimer;
            
            if (currentArenaTimer != null)
            {
                ProgressBar(MMMaths.Remap(currentArenaTimer.Elapsed, 0f, currentArenaTimer.Duration, 0f, 1f), BravoUtils.NewVec2NoAlloc(GetContentRegionAvail().x, GetTextLineHeight()), $"{currentArenaTimer.Elapsed}/{currentArenaTimer.Duration}");
                TextWrapped($"Paused: {currentArenaTimer.Paused}");
            }
            
            Text($"Absolute Current Time: {ArenaRuntimeManager.Current.AbsoluteCurrentTimeAsInt}");
            Text($"Relative Current Time: {ArenaRuntimeManager.Current.RelativeCurrentTimeAsInt}");
            
            RenderEnemiesSubmonitor();
            RenderRuntimeListenersSubmonitor();
        }
        
        private void RenderListeners(string label, Delegate[] invocationList)
        {
            if (TreeNode($"{label}"))
            {
                if (invocationList == null)
                {
                    TreePop();
                    return;
                }

                if (invocationList.Length == 0)
                {
                    TreePop();
                    return;
                }
                
                if (BeginTable($"##Table{label}", 2, ImGuiTableFlags.Borders))
                {
                    TableHeadersRow();
                    TableSetColumnIndex(0);
                    TextWrapped("GameObject");
                    TableSetColumnIndex(1);
                    TextWrapped("Behaviour");

                    for (int i = 0; i < invocationList.Length; i++)
                    {
                        TableNextRow();
                        TableSetColumnIndex(0);
                        TextWrapped(EPCUtils.RemoveAfterFirstParentheses(invocationList[i].Target.ToString()));
                        TableSetColumnIndex(1);
                        TextWrapped(EPCUtils.RemoveOutsideParentheses(invocationList[i].Target.ToString()));
                    }
                    
                    EndTable();
                }
                
                TreePop();
            }
        }

        private void RenderRuntimeListenersSubmonitor()
        {
            if (CollapsingHeader("Listeners"))
            {
                RenderListeners("On Start Arena", ArenaRuntimeManager.Current.OnArenaStart.GetInvocationList());
                RenderListeners("On End Arena", ArenaRuntimeManager.Current.OnArenaEnd.GetInvocationList());
                RenderListeners("On Arena Timer Tick", ArenaRuntimeManager.Current.OnArenaTimerTick.GetInvocationList());
                RenderListeners("On Arena Timer Change", ArenaRuntimeManager.Current.OnArenaTimerChange.GetInvocationList());
                RenderListeners("On Load Arena", ArenaRuntimeManager.Current.GetLoadArenaListeners());
                RenderListeners("On Unload Arena", ArenaRuntimeManager.Current.GetUnloadArenaListeners());
            }
        }

        private void RenderEnemiesSubmonitor()
        {
            if (CollapsingHeader("Enemies"))
            {
                _enemyNamesToCountInArena.Clear();
                for (int i = 0; i < MinionSpawnManager.Current.LoadedWaves.Count; i++)
                {
                    if (!_enemyNamesToCountInArena.ContainsKey(MinionSpawnManager.Current.LoadedWaves[i].EnemyName))
                    {
                        _enemyNamesToCountInArena.Add(MinionSpawnManager.Current.LoadedWaves[i].EnemyName, 0);
                    }

                    if (MinionSpawnManager.Current.LoadedWaves[i].isAlive && MinionSpawnManager.Current.LoadedWaves[i].isActive)
                    {
                        _enemyNamesToCountInArena[MinionSpawnManager.Current.LoadedWaves[i].EnemyName] +=
                            MinionSpawnManager.Current.LoadedWaves[i].CurrentEnemies;
                    }
                }

                //
                TextWrapped("Enemies currently alive:");
                SameLine();
                TextWrapped(MinionSpawnManager.Current.AmountOfEnemiesCurrentlyAlive.ToString());
                Indent();
                if (CollapsingHeader("Current Waves"))
                {
                    TextWrapped("Waves");
                    for (int i = 0; i < MinionSpawnManager.Current.LoadedWaves.Count; i++)
                    {
                        if (MinionSpawnManager.Current.LoadedWaves[i].isAlive &&
                            MinionSpawnManager.Current.LoadedWaves[i].isActive)
                        {
                            TextColored(Color.yellow,
                                $"{MinionSpawnManager.Current.LoadedWaves[i]._waveName} -> [{MinionSpawnManager.Current.LoadedWaves[i].CurrentEnemies}]");
                        }
                        else if (MinionSpawnManager.Current.LoadedWaves[i].isAlive &&
                                 !MinionSpawnManager.Current.LoadedWaves[i].isActive)
                        {
                            TextColored(Color.green,
                                $"{MinionSpawnManager.Current.LoadedWaves[i]._waveName} -> [{MinionSpawnManager.Current.LoadedWaves[i].CurrentEnemies}]");
                        }
                    }
                }

                if (CollapsingHeader("Enemies Per Enemy Name"))
                {
                    foreach (var elem in _enemyNamesToCountInArena)
                    {
                        TextWrapped($"{elem.Key} -> {elem.Value}");
                    }
                }

                Unindent();
            }
        }

        private void RenderArenaResults()
        {
            if (!ArenaRuntimeManager.Current.HasStarted) return;
            
            if (BeginTable("##ArenaResultsTable", 2, ImGuiTableFlags.Borders))
            {
                TableHeadersRow();
                SetColumnWidth(0, 150f);
                TableSetColumnIndex(0);
                TextWrapped("Name");
                TableSetColumnIndex(1);
                TextWrapped("Value");

                for (int i = 0; i < ArenaResultsManager.Current.Results.Count; i++)
                {
                    var result = ArenaResultsManager.Current.Results.ElementAt(i);
                    TableNextRow();

                    TableSetColumnIndex(0);
                    TextWrapped(result.Key.ToString());
                    TableSetColumnIndex(1);
                    int currentValue = result.Value;
                    if (InputInt($"##{result.Key.ToString()}", ref currentValue))
                    {
                        ArenaResultsManager.Current.SetResult(result.Key, currentValue);
                    }
                }

                EndTable();
            }

            if (CollapsingHeader("Listeners"))
            {
                RenderListeners("On Change Arena Results", ArenaResultsManager.Current.OnChangeArenaResults.GetInvocationList());
            }
        }

        private int _selectedArena = 1;
        
        private void RenderArenaSetup(Vector2 globalButtonSize)
        {
            TextWrapped("Arena:");
            PushItemWidth(globalButtonSize.x);
            Combo("##ArenaSelect", ref _selectedArena, TableOfArenas.GetArenaNames(), TableOfArenas.CountEntities);
            
            TextWrapped("Start Time:");
            if (SliderInt("##ArenaTimer", ref _debugArenaStartTime, 900, 1, $"{BravoUtils.FormatSecondsAsClock(_debugArenaStartTime)}"))
            {
                ArenaRuntimeManager.Current.DebugArenaTimer = true;
                ArenaRuntimeManager.Current.DebugArenaStartTime = _debugArenaStartTime;
            }

            if (Button("Start", globalButtonSize))
            {
                ArenaRuntimeManager.Current.TestArena(_selectedArena);
            }
        }
    }
}
