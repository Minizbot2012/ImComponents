using System;
using System.Collections.Generic;
using ImGuiNET;

namespace ImComponents;

public static class RadialMenuHelpers
{
    public static void SubMenu(string name, List<(string, Action<string>)> items)
    {
        if (ImComponents.AdvRadialMenu.Instance.BeginRadialMenu(name))
        {
            foreach (var item in items)
            {
                ImComponents.AdvRadialMenu.Instance.RadialMenuItem(item.Item1, item.Item2);
            }
            ImComponents.AdvRadialMenu.Instance.EndRadialMenu();
        }
    }
    public static void RadialMenu(string name, List<(string, Action<string>)> items, ref bool open)
    {
        if (open)
        {
            ImGui.OpenPopup(name);
        }
        if (ImComponents.AdvRadialMenu.Instance.BeginRadialPopup(name, open))
        {
            foreach (var item in items)
            {
                ImComponents.AdvRadialMenu.Instance.RadialMenuItem(item.Item1, item.Item2);
            }
            ImComponents.AdvRadialMenu.Instance.EndRadialMenu();
        }
    }
}