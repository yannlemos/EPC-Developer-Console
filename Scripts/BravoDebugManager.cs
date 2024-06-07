using System.Collections.Generic;
using System.IO;
using System.Linq;
using BansheeGz.BGDatabase;
using BravoDebug.MainMenuBar.MenuItems;
using ImGuiNET;
using IngameDebugConsole;
using MoreMountains.Tools;
using RuntimeInspectorNamespace;
using Sirenix.OdinInspector;
using UImGui;
using UnityEngine;
using UnityEngine.Events;
using static ImGuiNET.ImGui;

namespace BravoDebug
{
    public class BravoDebugManager : MMSingleton<BravoDebugManager>
    {
        [BravoMonitor] [SerializeField] [ToggleButtons(m_trueText = "On", m_falseText = "Off")]
        private bool _debugMode;

        [BravoMonitor] [SerializeField] [ToggleButtons(m_trueText = "On", m_falseText = "Off")]
        public bool ImguiDemoWindow;

        [BravoMonitor] [ShowInInspector] 
        private readonly List<IManagerDebug> _managerDebugs = new();
        
        [BravoMonitor] [ShowInInspector] 
        private readonly List<IShopDebug> _shopDebugs = new();
        
        [BravoSetting] public KeyCode MainDebugShortcut = KeyCode.F1;
        [BravoSetting] public KeyCode PlayersMonitorShortcut = KeyCode.F2;
        [BravoSetting] public KeyCode ArenaMonitorShortcut = KeyCode.F3;
        [BravoSetting] public KeyCode MainMonitorShortcut = KeyCode.F4;
        [BravoSetting] public KeyCode RuntimeDatabaseShortcut = KeyCode.F5;
        [BravoSetting] public KeyCode RuntimeHierarchyAndInspectorShortcut = KeyCode.F6;
        [BravoSetting] public KeyCode RuntimeConsoleShortcut = KeyCode.F7;
        
        [BravoReference] [SerializeField] 
        private DebugLogManager _runtimeConsole;
        
        [BravoReference] [SerializeField] 
        private RuntimeHierarchy _runtimeHierarchy;

        [BravoReference] [SerializeField] 
        private RuntimeInspector _runtimeInspector;
        
        [BravoReference] [SerializeField] 
        private BGRuntimeDatabaseEditor _runtimeDatabase;
        
        private MainMenuBar.MainMenuBar _mainMenuBar;
        private Monitors.MainMonitor _mainMonitor;
        private Monitors.PlayersMonitor _playersMonitor;
        private Monitors.ArenaMonitor _arenaMonitor;
        private Monitors.OptionsMonitor _optionsMonitor;
        
        private UnityAction OnRenderDebugMenus;

        protected override void Awake()
        {
            base.Awake();
        
            UImGuiUtility.Layout += OnLayout;
            UImGuiUtility.OnInitialize += OnInitialize;
        }

        private void OnInitialize(UImGui.UImGui obj)
        {
            GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        }

        private void OnDisable()
        {
            UImGuiUtility.Layout -= OnLayout;
        }

        private void Start()
        {
            if (!Debug.isDebugBuild) return;

            Initialization();
        }

        private void Initialization()
        {
            _mainMenuBar = new MainMenuBar.MainMenuBar();
            _mainMonitor = new Monitors.MainMonitor();
            _playersMonitor = new Monitors.PlayersMonitor();
            _arenaMonitor = new Monitors.ArenaMonitor();
            _optionsMonitor = new Monitors.OptionsMonitor();
            
            _debugMode = Debug.isDebugBuild;
        }
        
        private void Update()
        {
            if (!Debug.isDebugBuild) return;

            Cursor.visible = _debugMode;

            ManageDebugInputs();
        }

        private void ManageDebugInputs()
        {
            ManageDebugShortcuts();
            ManageDebugHotkeys();
        }

        private void ManageDebugHotkeys()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.N)) Players.Current.List.ForEach(t => t.ColliderController.ToggleCollisions(t.ColliderController.CollisionsAreDisabled));
            }
                
        }

        private void ManageDebugShortcuts()
        {
            if (Input.GetKeyDown(MainDebugShortcut))
                ToggleMainDebug();

            if (Input.GetKeyDown(RuntimeHierarchyAndInspectorShortcut))
                ToggleHierarchyAndInspectorDebug();

            if (Input.GetKeyDown(RuntimeConsoleShortcut))
                ToggleConsoleDebug();

            if (Input.GetKeyDown(MainMonitorShortcut))
                ToggleMainMonitor();

            if (Input.GetKeyDown(RuntimeDatabaseShortcut))
                ToggleDatabase();

            if (Input.GetKeyDown(PlayersMonitorShortcut))
                TogglePlayersMonitor();

            if (Input.GetKeyDown(ArenaMonitorShortcut))
                ToggleArenaMonitor();
        }

        public void ToggleMainDebug()
        {
            _debugMode = !_debugMode;
        }

        public bool HierarchyIsActive => _runtimeHierarchy.gameObject.activeSelf;
        public bool InspectorIsActive => _runtimeInspector.gameObject.activeSelf;
        public void ToggleHierarchyAndInspectorDebug()
        {
            _runtimeHierarchy.gameObject.SetActive(!_runtimeHierarchy.gameObject.activeSelf);
            _runtimeInspector.gameObject.SetActive(!_runtimeInspector.gameObject.activeSelf);
        }

        public bool ConsoleIsActive => _runtimeConsole.gameObject.activeSelf;
        public void ToggleConsoleDebug()
        {
            _runtimeConsole.gameObject.SetActive(!_runtimeConsole.gameObject.activeSelf);
        }

        public bool DatabaseIsActive => _runtimeDatabase.isActiveAndEnabled;
        public void ToggleDatabase()
        {
            _runtimeDatabase.gameObject.SetActive(!_runtimeDatabase.isActiveAndEnabled);
        }
        
        
        public bool MainMonitorIsActive => _mainMonitor.IsEnabled;
        public bool MainMonitorCanBeActivated => _mainMonitor.CanEnable();
        public void ToggleMainMonitor()
        {
            _mainMonitor.IsEnabled = !_mainMonitor.IsEnabled;
        }

        public bool PlayersMonitorIsActive => _playersMonitor.IsEnabled;
        public void TogglePlayersMonitor()
        {
            _playersMonitor.IsEnabled = !_playersMonitor.IsEnabled;
        }
        
        public bool ArenaMonitorIsActive => _arenaMonitor.IsEnabled;
        public void ToggleArenaMonitor()
        {
            _arenaMonitor.IsEnabled = !_arenaMonitor.IsEnabled;
        }

        public bool OptionsMonitorIsActive => _optionsMonitor.IsEnabled;
        public void ToggleOptionsMonitor()
        {
            _optionsMonitor.IsEnabled = !_optionsMonitor.IsEnabled;
        }        

        private void OnLayout(UImGui.UImGui obj)
        {
            if (!Debug.isDebugBuild) return;
            if (!_debugMode) return;

            if (ImguiDemoWindow) ShowDemoWindow();
        
            RenderMainMenuBar();
            RenderMonitors();
            RenderModals();
            
            OnRenderDebugMenus?.Invoke();
        }

        private void RenderMonitors()
        {
            _mainMonitor.Render();
            _playersMonitor.Render();
            _arenaMonitor.Render();
            _optionsMonitor.Render();
        }

        private void RenderMainMenuBar()
        {
            _mainMenuBar.Render(_managerDebugs, _shopDebugs);
        }

        [HideInInspector]
        public bool Popup;

        private DebugRequestWriter _debugRequestWriter = new();

        private string[] _devNames = BravoUtils.GetDevNames();
        private int _currentDevName;
        
        private string _debugRequestName = "";
        private string _debugRequestExpectedBehaviour = "";
        private string _debugRequestSuggestedPath = "";
        private string _debugRequestAuthor = "";

        private const string DebugRequestNameHint = "Ex: Buy All Upgrades";
        private const string DebugRequestExpectedBehaviourHint = "Ex: Increase the rank of all items in the Upgrade Shop by one";
        private const string DebugRequestSuggestedPathHint = "Ex: Utils -> Shops -> Buy All Upgrades";
        private const string DebugSaveTooltip = "On save, get the request file on your desktop and send it\nto the channel #debug-requests on Discord";

        private void RenderModals()
        {
            if (Popup) OpenPopup("Debug Request Form");
            
            Vector2 center = GetMainViewport().GetCenter();
            SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            SetNextWindowSize(new Vector2(GetContentRegionMax().x * 2, GetContentRegionMax().y * 1.5f));
            
            if (BeginPopupModal("Debug Request Form", ref Popup, ImGuiWindowFlags.AlwaysAutoResize))
            {
                TextWrapped("Name"); TextDebugRequestHint(DebugRequestNameHint);
                PushItemWidth(GetWindowSize().x);
                InputText("##Name", ref _debugRequestName, 250);
                PopItemWidth();
                TextWrapped("Expected Behaviour"); TextDebugRequestHint(DebugRequestExpectedBehaviourHint);
                InputTextMultiline("##Expected Behaviour", ref _debugRequestExpectedBehaviour, 1000, new Vector2(GetWindowSize().x, 200));
                
                TextWrapped("Suggested Path"); TextDebugRequestHint(DebugRequestSuggestedPathHint);
                PushItemWidth(GetWindowSize().x);
                InputText("##Suggested Path", ref _debugRequestSuggestedPath, 250);
                PopItemWidth();

                Spacing();
                Separator();

                TextWrapped("Author"); 
                if (Combo("##Author", ref _currentDevName, _devNames, _devNames.Length))
                {
                    _debugRequestAuthor = _devNames[_currentDevName];
                }
                
                SameLine();
                
                PushStyleColor(ImGuiCol.Button, new Color(Color.green.r / 2, Color.green.g, Color.green.b, 0.25f));
                if (Button("Save Request"))
                {
                    _debugRequestWriter.CreateAndSaveDebugRequestFile(
                        new DebugRequest
                        (
                            _debugRequestName,
                            _debugRequestExpectedBehaviour,
                            _debugRequestSuggestedPath,
                            _debugRequestAuthor)
                        );
                    
                    CloseCurrentPopup();
                }
                PopStyleColor();

                if (IsItemHovered())
                {
                    SetTooltip(DebugSaveTooltip);
                }
                
                SameLine();
                
                PushStyleColor(ImGuiCol.Button, new Color(Color.red.r / 2, Color.red.g, Color.red.b, 0.5f));
                if (Button("Clear"))
                {
                    _debugRequestName = "";
                    _debugRequestExpectedBehaviour = "";
                    _debugRequestSuggestedPath = "";
                    _debugRequestAuthor = "";
                    _currentDevName = 0;
                }
                PopStyleColor();
                
                EndPopup();
            }
        }

        private void TextDebugRequestHint(string hint)
        {
            SameLine();
            PushStyleColor(ImGuiCol.Text, Color.grey);
            TextWrapped(hint);
            PopStyleColor();
        }

        public void AddManagerDebug(IManagerDebug managerDebug)
        {
            _managerDebugs.Add(managerDebug);
            OnRenderDebugMenus += managerDebug.RenderDebug;
        }

        public void AddShopDebug(IShopDebug shopDebug)
        {
            _shopDebugs.Add(shopDebug);
            OnRenderDebugMenus += shopDebug.RenderDebug;

        }
    }
}

public class DebugRequestWriter
{
    public void CreateAndSaveDebugRequestFile(DebugRequest debugRequest)
    {
        // Construct the file name
        string fileName = $"DebugRequest_{BravoUtils.RemoveAllWhitespaceFromString(debugRequest.Name)}.txt";

        // Get the desktop path
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        // Combine the desktop path and file name to get the full file path
        string filePath = Path.Combine(desktopPath, fileName);

        // Create or overwrite the file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the content to the file
            writer.WriteLine($"Name: {debugRequest.Name}");
            writer.WriteLine($"Expected Behaviour: {debugRequest.ExpectedBehaviour}");
            writer.WriteLine($"Suggested Path: {debugRequest.SuggestedPath}");
            writer.WriteLine($"Author: {debugRequest.Author}");
        }

        Debug.Log($"Debug request file created at: {filePath}");
    }
}

public struct DebugRequest
{
    public string Name;
    public string ExpectedBehaviour;
    public string SuggestedPath;
    public string Author;

    
    public DebugRequest(string name, string expectedBehaviour, string suggestedPath, string author)
    {
        Name = name;
        ExpectedBehaviour = expectedBehaviour;
        SuggestedPath = suggestedPath;
        Author = author;
    }
}