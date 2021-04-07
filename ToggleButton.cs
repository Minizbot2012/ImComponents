using System.Numerics;
using ImGuiNET;

namespace ImGuiComponents
{
    public static class ToggleComponents
    {
        public static void ToggleButton(string id, ref bool v)
        {
            var p = ImGui.GetCursorScreenPos();
            var drawList = ImGui.GetWindowDrawList();
            float height = ImGui.GetFrameHeight();
            float width = height * 1.55f;
            float radius = height * 0.50f;
            ImGui.InvisibleButton(id, new Vector2(width, height));
            if (ImGui.IsItemClicked())
            {
                v = !v;
            }
            Vector4 col_bg;
            if (ImGui.IsItemHovered())
            {
                col_bg = v ? new Vector4((145 + 20) / 255f, 211 / 255f, (68 + 20) / 255f, 1f) : new Vector4((218 - 20) / 255f, (218 - 20) / 255f, (218 - 20) / 255f, 1f);
            }
            else
            {
                col_bg = v ? new Vector4(145 / 255f, 211 / 255f, 68 / 255f, 1f) : new Vector4(218 / 255f, 218 / 255f, 218 / 255f, 1f);
            }
            drawList.AddRectFilled(p, new Vector2(p.X + width, p.Y + height), ImGui.GetColorU32(col_bg), height * 0.5f);
            drawList.AddCircleFilled(new Vector2(p.X + radius + (v ? 1 : 0) * (width - radius * 2.0f), p.Y + radius), radius - 1.5f, ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f)));
            drawList.AddText(new Vector2(p.X + width + 2f, p.Y), ImGui.GetColorU32(ImGuiCol.Text), id);
        }
    }
}