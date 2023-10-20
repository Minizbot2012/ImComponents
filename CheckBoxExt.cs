using ImGuiNET;

namespace ImComponents;

public static class ChangedCheck {
    public static bool Checkbox(string text, ref bool active) {
        var old = active;
        ImGui.Checkbox(text, ref active);
        if(old != active) {
            return true;
        }
        return false;
    }
    public static bool Checkbox(string text, string tooltip, ref bool active) {
        var old = active;
        ImGui.Checkbox(text, ref active);
        if(ImGui.IsItemHovered())
            ImGui.SetTooltip(tooltip);
        if(old != active) {
            return true;
        }
        return false;
    }
}