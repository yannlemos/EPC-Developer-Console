# Extremely Powerful Capybaras Developer Console

ImGui developer console created for [Extremely Powerful Capybaras](https://store.steampowered.com/app/2089980/Extremely_Powerful_Capybaras/) in Unity.



https://github.com/yannlemos/EPC-Developer-Console/assets/16945950/f1a7f3a6-7a92-446c-8dda-a7e49e664578

## About

At Studio Bravarda, we incorporated [ImGui](https://github.com/ocornut/imgui) into our first game, [Sky Caravan](https://store.steampowered.com/app/1792270/Sky_Caravan/) and loved it. It's an amazing tool for game development. However, since the ImGui library is written in C++, we utilized the [uImGui](https://github.com/psydack/uimgui) wrapper to integrate it with Unity. Using it significantly accelerated our iteration, debugging, and overall dev processes. However, due to the library being wrapped, it offers limited IntelliSense support, and reviewing example implementations requires reading them in C++ and adapting them to C#. What is available in C# is very limited and small in scope.

That's why I'm making our development console for [Extremely Powerful Capybaras](https://store.steampowered.com/app/2089980/Extremely_Powerful_Capybaras/) freely available as an example implementation. This console demonstrates multiple use cases for ImGui and illustrates how a complex, shipped game can work with it.

> [!WARNING]
> This is not a plug-and-play example. I am providing the code here as-is, without any refinements, exactly as it was in the shipped game. This general reference is intended for you to utilize in your games, serving as example implementations and architecture to learn from, adapt, and improve. Over our two years of development, we made numerous decisions—some successful from day one, and others that proved less effective over time. So this is not a perfect example of ImGui use, just a very realistic one.

# Components

## Bravo Debug Manager

A singleton GameObject that sits in the game's main scene and initializes ImGui and all the dev console components and renders them. All of the components sit in the BravoDebug namespace to avoid mixing debug code with all the other systems and help in naming the classes and components without worrying about conflicts with the rest of the codebase.

## Main Menu Bar

The upper toolbar in the dev console. Declares and renders all the individual menu bar buttons, which are described below.

### Managers

![image](https://github.com/yannlemos/EPC-Developer-Console/assets/16945950/3f91e4a0-43e2-4ebb-b1f7-a7d3f22ec5bc)

Managers are core systems of the game that need a specific ImGui window for monitoring, debugging, and general dev access in build. They are rendered as a dropdown when you press the "Managers" button in the main menu bar. A good example is Meta Progression: we want to make an easy-to-use menu that will appear inside the "Managers" button and will render a small window focused on that manager only, like unlocking stages, achievements, and such. To do this, we create a `partial .Debug.cs` file for the manager, which implements the IManagerDebug interface.

Here's an example:

**`IManagerDebug`**
```csharp
using BravoDebug;

/// <summary>
/// This interface must call InitializeDebug() inside Awake or Start in the behavior in order to be initialized
/// </summary>
public interface IManagerDebug
{
    public bool EnableDebug
    {
        get => _openWindow;
        set => _openWindow = value;
    }

    private static bool _openWindow;
    
    /// <summary>
    /// This function must be called inside Awake or Start in order for the Debug Menu to be subscribed to the Debug Manager
    /// </summary>
    void InitializeDebug()
    {
        if (BravoDebugManager.Current == null) return;
        
        BravoDebugManager.Current.AddManagerDebug(this);
        EnableDebug = false;
    }

    /// <summary>
    /// Use a conditional with EnableDebug property to control menu state on the first line of implementation
    /// Ex: if (!EnableDebug) return;
    /// </summary>
    public void RenderDebug();
}
```
**`MetaProgressionManager.Debug.cs`** (just an example implementation, several managers use this pattern in the game)
```csharp
public partial class MetaProgressionManager : IManagerDebug
{
    public bool EnableDebug
    {
        get => _openWindow;
        set => _openWindow = value;
    }

    private static bool _openWindow;

    private int _selectedMPT;
    private int _selectedSteamAchievement;
    
    private void Start() => (this as IManagerDebug).InitializeDebug();
    
    public void RenderDebug()
    {
        if (!EnableDebug) return;

        SetNextWindowSize(new Vector2(500f, 500f), Appearing);
        
        if (Begin("Meta Progression", ref _openWindow))
        {
            string[] mptNames = GetMPTNamesFromDatabase();
            string[] steamAchievementNames = GetSteamAchievementNamesFromSteamSettings();

            if (BeginTabBar("MetaProgressionTabs"))
            {
                RenderMetaProgressionUtils();
                RenderMPTs(mptNames);
                RenderSteamAchievements(steamAchievementNames);
                EndTabBar();
            }

            End();
        }
        else
        {
            End();
        }
    }
...
```
![image](https://github.com/yannlemos/EPC-Developer-Console/assets/16945950/5e89e05a-629c-4ff4-a4d8-f4d50671ef30)

This is a very useful pattern to implement debug functionality inside a certain class, make it obey the general rendering process of ImGui without making a spaghetti mess and breaking encapsulation. Unfortunately, Unity doesn't support default implementation for interfaces, which makes some code duplication necessary. However, this was really helpful in making ImGui immediately accessible whenever we wanted in the core systems of the game. If we are working with networking, ok, create the debug partial, insert the boilerplate code, and start messing with the window without worrying about rendering order, how you access the menu, and a bunch of other things.

> [!TIP]
> The EnableDebug field is what renders the "X" button in the window and the "checkmark" in dropdown menus that show the current state of the window. I took some time to understand how this worked in C#, as it's a very C++ way of doing things and is not very well documented elsewhere. I use this implementation a lot because it's very natural for game designers, even though a bit more cumbersome to do in code.

---

### Shops

The game has four different shops that the player can access. This menu button renders all the shops as a dropdown so you can easily access anywhere in the build while opening their individual debug menus at the same time.

### Tools

![image](https://github.com/yannlemos/EPC-Developer-Console/assets/16945950/32942b51-1ab7-48ca-80d6-40317e0fe4b1)

Displays all the available main debug menus as a dropdown, with their respective hotkeys.

### Utils

![image](https://github.com/yannlemos/EPC-Developer-Console/assets/16945950/ac87cece-38cf-48c5-9e57-472f7599f696)

A general menu for useful stuff that devs need to access quickly to test things fast. With this menu, you can spawn any enemy, boss, gain money, change the timescale, and others. Basic god powers that are useful to have when testing the game.

### Views

![image](https://github.com/yannlemos/EPC-Developer-Console/assets/16945950/fe8829cd-7cc3-4320-b978-eb44914a6f31)

A full list of all the unique screens in the game, which are called Views in code.

### Languages

Fast access to language change anywhere in the build to any of the 5 supported languages (English, Portuguese, Simplified Chinese, Korean, and Japanese).

### ImGui

Just a button to display the demo window of ImGui for my purposes when I need to take a look at a certain component while developing the console.

### Help

![image](https://github.com/yannlemos/EPC-Developer-Console/assets/16945950/c68b2bc8-32c5-4dda-98e8-b7a7c1c3939b)

This is a menu I made to learn how the ImGui modals work and also to make the debug tooling requests that the other developers were making easier and faster for me to parse. Modals have a bunch of specific settings which are a bit weird to get in C#, so it's useful code to look at even if you won't do a debug request in build.

## Monitors

Monitors are the main menus of the debug console, mostly for monitoring values and the game's runtime, but also containing important controls to manipulate the game's state.

### Player Monitor

The player monitor keeps track of all the players in the game and exposes dozens of controls to expedite testing. This is extremely important for multiplayer games, as we learned the hard way. Adding and removing players and going to and from certain game states need to be as fast as possible. Foldouts were great to help make good use of space, and ImGui really excels in these situations where you need a menu to be responsive from the get-go, because the monitor needs to be visible a lot of the time when checking stat and ability changes in EPC's case.

### Arena Monitor

Same as the player monitor, but specifically for the game's arenas. EPC is a horde-survivor, so the arenas are the main experience of the game, and a dedicated menu really improved our testing times and debugging by exposing absolutely everything going on inside the arena at any moment in-build, especially in a networked environment.

### Main Monitor

A kind of basic "what's up" of the game's main info which are important to be kept track of at most times, like what cameras are on, what scenes, and so on.

### Options Monitor

A direct deserialization to the game's options, which are contained in a .json file. Useful to bypass the UI and avoid interface problems being confused with serialization ones. Nothing fancy, just renders lines of text with the options.
