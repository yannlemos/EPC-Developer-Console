using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BansheeGz.BGDatabase;
using BravoDatabase;
using ImGuiNET;
using Micosmo.SensorToolkit;
using MoreMountains.Tools;
using UImGui;
using UnityEngine;
using static ImGuiNET.ImGui;

namespace BravoDebug.Monitors
{
    public class PlayersMonitor
    {
        public bool IsEnabled
        {
            get => _openWindow;
            set => _openWindow = value;
        }

        private bool _openWindow;

        private int selectedBlessing = 0;

        private readonly string[] blessingClasses =
        {
            CapyClassName.Warrior.ToString(), CapyClassName.Assassin.ToString(), CapyClassName.Druid.ToString(),
            CapyClassName.Bard.ToString(),
            CapyClassName.Mage.ToString(), CapyClassName.Gunslinger.ToString()
        };

        private int selectedClass;
        private int selectedSkin;


        public void Render()
        {
            if (!IsEnabled) return;

            SetNextWindowPos(BravoUtils.NewVec2NoAlloc(0f, 28f), ImGuiCond.Appearing);
            SetNextWindowSize(BravoUtils.NewVec2NoAlloc(400f, 956f), ImGuiCond.Appearing);
            SetNextWindowBgAlpha(0.99f);

            if (Begin("Players Monitor", ref _openWindow))
            {
                if (Players.Current.Count == 0)
                {
                    End();
                    return;
                }

                TextWrapped("Level:");
                SameLine();
                ProgressBar(
                    MMMaths.Remap(ExperienceManager.Current.CurrentLevel, 0f, LevelUpManager.Current.PlayerLevelCap, 0f,
                        1f), BravoUtils.NewVec2NoAlloc(GetContentRegionAvail().x, GetTextLineHeight()),
                    $"{ExperienceManager.Current.CurrentLevel}/{LevelUpManager.Current.PlayerLevelCap}");
                // TextWrapped($"Current XP: [{ExperienceManager.Current.CurrentExperience}] | Need Next Level: [{ExperienceManager.Current.TotalExperienceToNextLevel}]");
                TextWrapped("Current Experience:");
                ProgressBar(
                    MMMaths.Remap(ExperienceManager.Current.CurrentExperience,
                    ExperienceManager.Current.TotalExperienceToCurrentLevel,
                    ExperienceManager.Current.TotalExperienceToNextLevel,
                    0f,
                    1f), BravoUtils.NewVec2NoAlloc(GetContentRegionAvail().x, GetTextLineHeight()),
                    $"{ExperienceManager.Current.CurrentExperience}/{ExperienceManager.Current.TotalExperienceToNextLevel}");
                Spacing();

                if (BeginTabBar("Players"))
                {
                    Unindent();
                    for (var i = 0; i < Players.Current.Count; i++)
                    {
                        if (!Players.Current.TryGetPlayerByIndex(i, out var player)) continue;

                        var playerColor = TableOfColors.GetColorByPlayerNumber(i + 1);

                        var idleColor = new Color(playerColor.r, playerColor.g, playerColor.b, playerColor.a / 2f);
                        var hoveredColor = new Color(playerColor.r, playerColor.g, playerColor.b, playerColor.a / 4f);
                        var activeColor = playerColor;

                        PushStyleColor(ImGuiCol.Tab, idleColor);
                        PushStyleColor(ImGuiCol.TabHovered, hoveredColor);
                        PushStyleColor(ImGuiCol.TabActive, activeColor);
                        PushStyleColor(ImGuiCol.Header, idleColor);
                        PushStyleColor(ImGuiCol.HeaderHovered, hoveredColor);
                        PushStyleColor(ImGuiCol.HeaderActive, activeColor);

                        if (BeginTabItem($"P{i + 1}"))
                        {
                            Indent();
                            RenderPlayerComponents(player);
                            EndTabItem();
                        }

                        PopStyleColor(6);
                    }

                    EndTabBar();
                }

                Spacing();
                Separator();
                Spacing();

                RenderGlobalUtils();

                End();
            }
        }

        private void RenderGlobalUtils()
        {
            var globalButtonSize = BravoUtils.NewVec2NoAlloc(GetContentRegionAvail().x, 30f);

            TextWrapped("Global Utils");

            if (Button("Quickset Player Classes", globalButtonSize))
                Players.Current.QuicksetPlayerClasses();

            if (Button("Spawn Local Player", globalButtonSize))
                Players.Current.SpawnLocalPlayer();

            if (Button("Randomize Builds", globalButtonSize))
                Players.Current.List.ForEach(DebugRandomizeBuild);

            if (Button("Level Up", globalButtonSize))
                LevelUpManager.Current.StartLevelUp();

            if (Button("Open Chest", globalButtonSize))
                ChestManager.Current.ChestOpened();

            if (Button("Increase Reroll Amount", globalButtonSize))
                LevelUpManager.Current.AddToCurrentRerollAmount();

            if (TreeNode("Blessings"))
            {
                Unindent();
                if (Button("Warrior", globalButtonSize)) BlessingsManager.Current.BlessPlayers(CapyClassName.Warrior);
                if (Button("Assassin", globalButtonSize)) BlessingsManager.Current.BlessPlayers(CapyClassName.Assassin);
                if (Button("Druid", globalButtonSize)) BlessingsManager.Current.BlessPlayers(CapyClassName.Druid);
                if (Button("Bard", globalButtonSize)) BlessingsManager.Current.BlessPlayers(CapyClassName.Bard);
                if (Button("Gunslinger", globalButtonSize)) BlessingsManager.Current.BlessPlayers(CapyClassName.Gunslinger);
                if (Button("Mage", globalButtonSize)) BlessingsManager.Current.BlessPlayers(CapyClassName.Mage);
                Indent();
                TreePop();
            }
        }

        private void RenderPlayerComponents(BravoPlayer player)
        {
            TextWrapped("Name: " + player.SteamNickname);

            if (RadioButton("Noclip (Ctrl+N)", player.ColliderController.CollisionsAreDisabled))
                player.ColliderController.ToggleCollisions(player.ColliderController.CollisionsAreDisabled);

            if (player.PlayerIndex != 0 && BravoGameManager.Current.IsInLocalMultiplayerGame)
                if (Button("Despawn Local Player"))
                    Players.Current.DespawnPlayer(player.PlayerNumber);

            RenderTransformSubmonitor(player);
            RenderNetworkingSubmonitor(player);
            RenderCapybaraSubmonitor(player);
            RenderHealthSubmonitor(player);
            RenderStatsSubmonitor(player);
            RenderAbilitiesSubmonitor(player);
            RenderDynamicsSubmonitor(player);
        }

        private void RenderNetworkingSubmonitor(BravoPlayer player)
        {
            if (CollapsingHeader("Networking"))
            {
                TextWrapped("Server Status: " + (player.IsServer ? "Host" : "Client"));
                TextWrapped("Ownership: " + (player.IsLocal ? "Local" : "Remote"));

                if (player.GetPingFloat() <= 20)
                    TextColored(Color.green, "Ping: " + player.GetPingString());
                else if (player.GetPingFloat() > 20 && player.GetPingFloat() <= 100)
                    TextColored(Color.yellow, "Ping: " + player.GetPingString());
                else if (player.GetPingFloat() > 100) TextColored(Color.red, "Ping: " + player.GetPingString());
            }
        }

        private void RenderCapybaraSubmonitor(BravoPlayer player)
        {
            if (CollapsingHeader("Capybara"))
            {
                TextWrapped("Capy-Class: " + player.CurrentCapyClass);
                TextWrapped("Current Rank: " + player.ClassController.CurrentClassRank);
                if (Combo("##Set Class", ref selectedClass, TableOfCapyClasses.GetClassNames(),
                        TableOfCapyClasses.GetClassNames().Length))
                    player.ClassController.SetCapyClass(TableOfCapyClasses.GetEntity(selectedClass).f_CapyClassName);
                if (TableOfCapyClasses.CanSynergize(player.CurrentCapyClass))
                    if (Button("Synergize"))
                        player.ClassController.Synergize();
                Separator();

                if (TableOfSkinShop.GetEntitiesByKeyCategory(player.CurrentCapyClass) == null)
                    return;

                TextWrapped("Capy-Skin: " + player.CurrentCapySkin);

                var validSkinNames = TableOfSkinShop.GetEntitiesByKeyCategory(player.CurrentCapyClass).Select(t => t.f_CapySkin.f_name).ToArray();

                if (Combo("##Set Skin", ref selectedSkin, validSkinNames, validSkinNames.Length))
                    player.SkinController.SetCapySkin(validSkinNames[selectedSkin]);
            }
        }

        private void RenderTransformSubmonitor(BravoPlayer player)
        {
            if (CollapsingHeader("Transform"))
            {
                var playerTransform = player.transform;

                var playerPosition = playerTransform.position;
                var newPosition = BravoUtils.NewVec3NoAlloc(playerPosition.x, playerPosition.y, playerPosition.z);

                var playerRotation = playerTransform.rotation;
                var newRotation = BravoUtils.NewVec3NoAlloc(playerRotation.x, playerRotation.y, playerRotation.z);

                var playerScale = playerTransform.localScale;
                var newScale = BravoUtils.NewVec3NoAlloc(playerScale.x, playerScale.y, playerScale.z);

                var dragFloatWidth = GetWindowWidth();

                BulletText("Position");
                PushItemWidth(dragFloatWidth);
                if (DragFloat3("##Position", ref newPosition)) playerTransform.position = newPosition;

                BulletText("Rotation");
                PushItemWidth(dragFloatWidth);
                if (DragFloat3("##Rotation", ref newRotation)) playerTransform.rotation = Quaternion.Euler(newRotation);

                BulletText("Scale");
                PushItemWidth(dragFloatWidth);
                if (DragFloat3("##Scale", ref newScale)) playerTransform.localScale = newScale;
            }
        }

        private void RenderDynamicsSubmonitor(BravoPlayer player)
        {
            if (CollapsingHeader("Dynamics"))
            {
                var topDownController = player.TopDownController;
                var rigidbody = player.GetComponent<Rigidbody2D>();
                var collider = player.GetComponent<Collider2D>();

                if (TreeNode("Top Down Controller"))
                {
                    TextWrapped("Direction: " + topDownController.Direction);
                    TextWrapped("Velocity: " + topDownController.Velocity);
                    if (RadioButton("Free Movement", topDownController.FreeMovement))
                        topDownController.FreeMovement = !topDownController.FreeMovement;

                    TreePop();
                }

                Separator();

                if (TreeNode("Collider"))
                {
                    TextWrapped("Collisions Enabled: " + collider.enabled);

                    List<Collider2D> collidingWith = new();
                    collider.GetContacts(collidingWith);

                    TextWrapped("Current Collisions: " + collidingWith.Count);

                    if (collidingWith.Count > 0)
                        for (var i = 0; i < collidingWith.Count; i++)
                            BulletText(collidingWith[i].gameObject.name);

                    TreePop();
                }

                Separator();

                if (TreeNode("Sensor"))
                {
                    var sensor = player.GetComponent<Sensor>();

                    var signalsCount = sensor.Signals.Count;
                    TextWrapped("Current Signals: " + signalsCount);

                    for (var i = 0; i < signalsCount; i++)
                    {
                        var signal = sensor.Signals.ElementAt(i);

                        BulletText(signal.Object.name + " | " + signal.Strength);
                    }

                    TreePop();
                }
            }
        }

        private int currentPassive;
        private int currentActive;

        private void RenderAbilitiesSubmonitor(BravoPlayer player)
        {
            var abilities = player.AbilityController.Abilities;

            if (CollapsingHeader("Abilities"))
            {
                if (BeginTabBar("AbilityTabBar"))
                {
                    if (BeginTabItem("List"))
                    {
                        RenderAbilityList(abilities);
                        EndTabItem();
                    }

                    if (BeginTabItem("Monitor"))
                    {
                        RenderAbilityMonitor(player.AbilityController, abilities);
                        EndTabItem();
                    }

                    if (BeginTabItem("Utils"))
                    {
                        RenderAbilityUtils(player, abilities);
                        EndTabItem();
                    }

                    EndTabBar();
                }

                Separator();
            }
        }

        private void RenderAbilityMonitor(PlayerAbilityController controller, List<BravoAbility> abilities)
        {
            TextWrapped($"Abilities equipped: {controller.TotalNumberOfAbilitiesEquipped}");
            TextWrapped($"Actives equipped: {controller.TotalNumberOfActivesEquipped}");
            TextWrapped($"Passives equipped: {controller.TotalNumberOfPassivesEquipped}");
            TextWrapped($"Empty ability slots: {controller.TotalNumberOfEmptyAbilitySlots}");
            TextWrapped($"At maximum ability capacity: {controller.IsAtMaxAbilityCapacity}");
            TextWrapped($"At maximum active capacity: {controller.IsAtMaxActiveAbilityCapacity}");
            TextWrapped($"At maximum passive capacity: {controller.IsAtMaxPassiveAbilityCapacity}");
            TextWrapped($"Has dash: {controller.HasDash}");
            TextWrapped($"Has primary ability: {controller.HasPrimaryAbility}");
            TextWrapped($"Actives and Passives at max level: {controller.AllActivesAndPassivesAreAtMaxLevel}");
            TextWrapped($"All actives at max level: {controller.ActivesAtMaxLevel}");
            TextWrapped($"All passives at max level: {controller.PassivesAtMaxLevel}");
            TextWrapped($"Max ability capacity: {controller.MaxAbilityCapacity}");
            TextWrapped($"Max active capacity: {controller.MaxActiveAbilityCapacity}");
            TextWrapped($"Max passive capacity: {controller.MaxPassiveAbilityCapacity}");
            TextWrapped($"Full Capacity For All & Max Level: {controller.AllActivesAndPassivesAreAtMaxLevel && controller.IsAtMaxAbilityCapacity}");
        }

        //Here we precache 64 textures to use toa void allocating a bunch every frame
        private Texture2D[] _generatedTextures = new Texture2D[64];

        private void RenderAbilityList(List<BravoAbility> abilities)
        {
            for (var i = 0; i < abilities.Count; i++)
            {
                var ability = abilities[i];

                // Get the sprite in the atlas
                var sprite = TableOfAbilityIndex.GetEntity(ability.Name).f_Icon;

                // Get the texture of the atlas
                var texture = sprite.texture;

                // Get the texture of the sprite in the atlas
                // var spriteTexture = new Texture2D((int)sprite.textureRect.width, (int)sprite.textureRect.height);
                if (_generatedTextures[i] == null)
                {
                    _generatedTextures[i] = new Texture2D((int)sprite.textureRect.width, (int)sprite.textureRect.height);
                }
                else
                {
                    _generatedTextures[i].Reinitialize((int)sprite.textureRect.width, (int)sprite.textureRect.height);

                }
                _generatedTextures[i].SetPixels(texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y,
                        (int)sprite.textureRect.width, (int)sprite.textureRect.height));
                _generatedTextures[i].Apply();


                var id = UImGuiUtility.GetTextureId(_generatedTextures[i]);
                var size = new Vector2(40, 40);
                Image(id, size);

                SameLine();

                PushStyleColor(ImGuiCol.TableHeaderBg,
                    TableOfColors.GetEntity($"{ability.AbilityType}Abilities").f_Color);
                PushStyleColor(ImGuiCol.Text, TableOfColors.GetEntity($"{ability.AbilityType}Abilities").f_Color);

                if (TreeNode($"Ability{ability.Name}", $"{ability.Name}\nLvl. {ability.CurrentLevel}"))
                {
                    PopStyleColor();
                    Unindent();

                    TextWrapped($"Rank: {ability.CurrentRank}");
                    TextWrapped($"Direction: {ability.AbilityDirection}");
                    if (RadioButton("Permitted", ability.AbilityPermitted)) ability.ToggleAbilityPermission(!ability.AbilityPermitted);
                    if (Button("Level Up")) ability.LevelUp();
                    if (ability.AbilityType is not AbilityType.Passive) RenderCooldown(ability);

                    if (TreeNode("##AbilityStats", "Stats"))
                    {
                        RenderStats(ability.AbilityStats, TableOfAbilityStats.MetaDefault);

                        TreePop();
                    }

                    Indent();
                    TreePop();
                }

                PopStyleColor();

                if (i != abilities.Count - 1)
                {
                    Separator();
                    Spacing();
                }
            }
        }

        private void RenderAbilityUtils(BravoPlayer player, List<BravoAbility> abilities)
        {
            var passiveAbilityNames = TableOfAbilityIndex.GetPassiveAbilityNames();
            if (Button("Add Passive"))
                player.AbilityController.AddAbility(TableOfAbilityIndex
                    .GetEntity(passiveAbilityNames[currentPassive]).f_MainPrefab);
            SameLine();
            PushItemWidth(GetContentRegionAvail().x);
            Combo("##Passives", ref currentPassive, passiveAbilityNames, passiveAbilityNames.Length);

            var activeAbilityNames = TableOfAbilityIndex.GetActiveAbilityNames();
            if (Button("Add Active"))
                player.AbilityController.AddAbility(TableOfAbilityIndex
                    .GetEntity(activeAbilityNames[currentActive]).f_MainPrefab);
            SameLine();
            PushItemWidth(GetContentRegionAvail().x);
            Combo("##Actives", ref currentActive, activeAbilityNames, activeAbilityNames.Length);

            if (Button("Randomize Build", BravoUtils.NewVec2NoAlloc(GetContentRegionAvail().x, 30f)))
            {
                DebugRandomizeBuild(player);
            }

            if (Button("Level Up All", BravoUtils.NewVec2NoAlloc(GetContentRegionAvail().x, 30f)))
                foreach (var t in abilities)
                    if(!t.IsPrimary)
                        t.LevelUp();
        }

        private static void DebugRandomizeBuild(BravoPlayer player)
        {
            var randomAbilities = AbilityDealer.Current.DealRandomAbilitiesToPlayer();

            foreach (var t in randomAbilities) player.AbilityController.AddAbility(t);
        }

        private void RenderCooldown(BravoAbility ability)
        {
            var cooldown = ability.Cooldown;

            if (TreeNode("Cooldown"))
            {
                ProgressBar(MMMaths.Remap(cooldown.CurrentDurationLeft, 0, cooldown.RefillDuration, 0f, 1f),
                    BravoUtils.NewVec2NoAlloc(GetContentRegionAvail().x, GetTextLineHeight() * 2), $"{cooldown.CooldownState}");

                TextWrapped("Durations:");

                BulletText($"Refill: {cooldown.RefillDuration}");
                BulletText($"Casting: {cooldown.CastingDuration}");

                TreePop();
            }
        }

        private void RenderStatsSubmonitor(BravoPlayer player)
        {
            if (CollapsingHeader("Stats"))
            {
                var stats = player.Stats;

                RenderStats(stats, TableOfPlayerStats.MetaDefault);
            }
        }

        private void RenderStats(BravoStats stats, BGMetaEntity statTable)
        {
            for (var index = 0; index < stats.Stats.Count; index++)
            {
                var stat = stats.Stats[index];

                // var icon = statTable.GetKey("Main").GetEntityByKey(stat.Name)).f_Icon.texture;

                if (stat.Name != StatName.Modifier)
                {
                    var statEntity = statTable.GetKey("Main").GetEntityByKey(stat.Name);

                    var icon = statEntity.Get<Sprite>("Icon");

                    var id = UImGuiUtility.GetTextureId(icon.texture);
                    var size = BravoUtils.NewVec2NoAlloc(20, 20);
                    Image(id, size);

                    SameLine();

                    var statColor = statEntity.Get<Color>("Color");
                    PushStyleColor(ImGuiCol.TableHeaderBg, statColor);
                    PushStyleColor(ImGuiCol.Text, statColor);
                }

                TextUnformatted($"{stat.GetValueAsString()}");


                SameLine(85);

                if (TreeNode($"Stat{stat.Name.ToString()}", stat.Name.ToString()))
                {
                    PopStyleColor();

                    Unindent();
                    if (BeginTable($"#StatTable{stat.Name}", 2, ImGuiTableFlags.Borders))
                    {
                        TableHeadersRow();
                        TableSetColumnIndex(0);
                        TextWrapped("Info");
                        TableSetColumnIndex(1);
                        TextWrapped("Value");

                        TableNextRow();

                        TableSetColumnIndex(0);
                        TextWrapped("Base Value");
                        TableSetColumnIndex(1);
                        TextUnformatted(stat.BaseValue.ToString(CultureInfo.InvariantCulture) +
                                        stat.GetValueTypeSuffix(stat.ValueType));

                        TableNextRow();

                        TableSetColumnIndex(0);
                        TextWrapped("Initial Value");
                        TableSetColumnIndex(1);
                        TextUnformatted(stat.InitialValue.ToString(CultureInfo.InvariantCulture) +
                                        stat.GetValueTypeSuffix(stat.ValueType));

                        TableNextRow();

                        TableSetColumnIndex(0);
                        TextWrapped("Value Type");
                        TableSetColumnIndex(1);
                        TextWrapped(stat.ValueType.ToString());

                        EndTable();
                    }

                    Indent();

                    if (stat.HasModifiers())
                    {
                        Unindent(GetTreeNodeToLabelSpacing());
                        if (TreeNode($"StatModifiers{stat.Name.ToString()}", "Modifiers"))
                        {
                            for (var i = 0; i < stat.Modifiers.Count; i++)
                                if (TreeNode($"StatModifiers{i}{stat.Name.ToString()}",
                                        $"{stat.Modifiers[i].Source} [{i}]"))
                                {
                                    var t = stat.Modifiers[i];

                                    if (BeginTable($"#StatModifierTable{stat.Name}_{t.Source}", 2,
                                            ImGuiTableFlags.Borders))
                                    {
                                        TableHeadersRow();
                                        TableSetColumnIndex(0);
                                        TextWrapped("Info");
                                        TableSetColumnIndex(1);
                                        TextWrapped("Value");

                                        TableNextRow();

                                        TableSetColumnIndex(0);
                                        TextWrapped("Value");
                                        TableSetColumnIndex(1);
                                        TextUnformatted(t.GetValueAsString());

                                        TableNextRow();

                                        TableSetColumnIndex(0);
                                        TextWrapped("Type");
                                        TableSetColumnIndex(1);
                                        TextWrapped(t.ValueType.ToString());

                                        EndTable();
                                    }

                                    TreePop();
                                }

                            TreePop();
                        }

                        Indent();
                    }

                    TreePop();
                }

                PopStyleColor();

                if (index != stats.Stats.Count - 1)
                    Separator();
            }
        }

        private int damageToTake = 10;
        private int healthToHeal = 10;

        private void RenderHealthSubmonitor(in BravoPlayer player)
        {
            if (CollapsingHeader("Health"))
            {
                var health = player.Health;
                TextWrapped("Health:");
                SameLine();
                ProgressBar(MMMaths.Remap(health.CurrentHealth, 0f, health.MaximumHealth, 0f, 1f),
                    BravoUtils.NewVec2NoAlloc(GetContentRegionAvail().x, GetTextLineHeight()),
                    $"{health.CurrentHealth}/{health.MaximumHealth}");
                TextWrapped("Last Damage: " + health.LastDamage);
                TextWrapped("Invulnerable:");
                SameLine();

                if (RadioButton("##Invincibility", health.Invulnerable))
                {
                    if (health.Invulnerable)
                        health.SetVulnerable();
                    else
                        health.SetInvulnerable();
                }

                TextWrapped("Infinity Health:");
                SameLine();
                if (RadioButton("##Infinity Health", health.InfiniteHealth))
                {
                    if (health.InfiniteHealth)
                        health.SetFiniteHealth();
                    else
                        health.SetInfiniteHealth();
                }

                if (Button("Take Damage")) health.TakeDamage(damageToTake);
                SameLine();
                DragInt("##Damage", ref damageToTake);

                if (Button("Heal Health")) health.ReceiveHealth(healthToHeal);
                SameLine();
                DragInt("##Heal", ref healthToHeal);
            }
        }
    }
}