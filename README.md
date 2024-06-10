# Extremely Powerful Capybaras Developer Console
ImGui developer console created for [Extremely Powerful Capybaras](https://store.steampowered.com/app/2089980/Extremely_Powerful_Capybaras/) in Unity.

![image](https://github.com/yannlemos/epc_developer_console/assets/16945950/b7db2b23-13b0-4a52-b173-4688491a0d48)

## About
At Studio Bravarda, we incorporated [ImGui](https://github.com/ocornut/imgui) into our first game, [Sky Caravan](https://store.steampowered.com/app/1792270/Sky_Caravan/) and loved it. It's an amazing tool for game development. However, since the ImGui library is written in C++, we utilized the [uImGui](https://github.com/psydack/uimgui) wrapper to integrate it with Unity. Using it significantly accelerated our iteration, debugging, and overall dev processes. However, due to the library being wrapped, it offers limited IntelliSense support, and reviewing example implementations requires reading them in C++ and adapting them to C#. What is available in C# is very limited, and small in scope.

That's why I'm making making our development console for [Extremely Powerful Capybaras](https://store.steampowered.com/app/2089980/Extremely_Powerful_Capybaras/) freely available as an example implementation. This console demonstrates multiple use cases for ImGui and illustrates how a complex, shipped game can work with it.

**IMPORTANT**: This is not a plug-and-play example. I am providing the code here as-is, without any refinements, exactly as it was in the shipped game. This general reference is intended for you to utilize in your games, serving as example implementations and architecture to learn from, adapt, and improve. Over our two years of development, we made numerous decisions—some successful from day one, and others that proved less effective over time. So this is not a perfect example of ImGui use, just a very realistic one.

## Examples
This session is WIP, I'll put some detailed explanations of some parts of the code as soon as possible.
