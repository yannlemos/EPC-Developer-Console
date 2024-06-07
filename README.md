# Extremely Powerful Capybaras Developer Console
ImGui developer console created for [Extremely Powerful Capybaras](https://store.steampowered.com/app/2089980/Extremely_Powerful_Capybaras/) in Unity.

![image](https://github.com/yannlemos/epc_developer_console/assets/16945950/b7db2b23-13b0-4a52-b173-4688491a0d48)

## About
We started using [ImGui](https://github.com/ocornut/imgui) in Studio Bravarda's first game, [Sky Caravan](https://store.steampowered.com/app/1792270/Sky_Caravan/). Since the source library is in C++, we used the [uImGui](https://github.com/psydack/uimgui) wrapper to use it in Unity. It is an amazing library and really sped up iteration, debugging and overall development of the game. However, there's very limited intellisense since it's a wrapped library and to take a look at example implementations you have to read them in C++ and adapt them to the C# implementation. 

That's why I decided to make freely available as an example implementation our development console for Extremely Powerful Capybaras. It contains multiple use-cases for ImGui and shows how a complex, shipped game can use the library in a way that enhances development.

**Important:** this is not a plug-and-play example. I'm uploading the code here with no window dressing, warts-and-all as it is in the shipped game as a general reference for you to use in your games and example implementations and architecture to steal from and improve upon. We developed this during 2 years of development, little by little, with some decisions that with time didn't work that well, and other things that worked really well since day 1.
