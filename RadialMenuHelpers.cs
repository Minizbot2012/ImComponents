using System.Collections.Generic;
using System;
using ImGuiNET;

namespace ImComponents
{
    public static class RadialMenuHelpers
    {
        public static void SubMenu(AdvRadialMenu _impl, string name, List<(string, Action<string>)> items)
        {
            if (_impl.BeginRadialMenu(name))
            {
                foreach (var item in items)
                {
                    _impl.RadialMenuItem(item.Item1, item.Item2);
                }
                _impl.EndRadialMenu();
            }
        }
        public static void RadialMenu(AdvRadialMenu _impl, string name, List<(string, Action<string>)> items, ref bool open)
        {
            if (open)
            {
                ImGui.OpenPopup(name);
            }
            if (_impl.BeginRadialPopup(name, open))
            {
                foreach (var item in items)
                {
                    _impl.RadialMenuItem(item.Item1, item.Item2);
                }
                _impl.EndRadialMenu();
            }
        }
    }
}