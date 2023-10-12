using ImGuiNET;

namespace ImComponents;

public static class RadialMenuHelpers
{
    public static void SubMenu(string name, List<(string, Action<string>)> items)
    {
        if (AdvRadialMenu.Instance.BeginRadialMenu(name))
        {
            foreach (var item in items)
            {
                AdvRadialMenu.Instance.RadialMenuItem(item.Item1, item.Item2);
            }
            AdvRadialMenu.Instance.EndRadialMenu();
        }
    }
    public static void RadialMenu(string name, List<(string, Action<string>)> items, ref bool open)
    {
        if (open)
        {
            ImGui.OpenPopup(name);
        }
        if (AdvRadialMenu.Instance.BeginRadialPopup(name, open))
        {
            foreach (var item in items)
            {
                AdvRadialMenu.Instance.RadialMenuItem(item.Item1, item.Item2);
            }
            AdvRadialMenu.Instance.EndRadialMenu();
        }
    }
}