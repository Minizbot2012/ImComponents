using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace ImComponents
{
    public static class AdvRadialMenu
    {
        private static readonly float IM_PI = 3.14159265358979323846f;
        private struct RadialMenuRootCtx
        {
            public Stack<RadialMenuContext> contexts;
            public Stack<int> Hovered;
            public Stack<float> Rotations;
            public int last;
            public Vector2 Center;
        }
        public struct RadialMenuElem
        {
            public string Title;
            public bool IsSubMenu;
        }
        public struct RadialMenuContext
        {
            public float Rotation;
            public string Name;
            public int IdxHovered;
            public float RADIUS_MIN;
            public float RADIUS_MAX;
            public List<RadialMenuElem> Items;
            public Vector2 center;
            public bool Open;
        }
        private static RadialMenuRootCtx P = new()
        {
            contexts = new(),
            Hovered = new(),
            Rotations = new(),
            last = 0
        };
        private static Stack<RadialMenuContext> Contexts => P.contexts;
        public static Stack<int> Hovered => P.Hovered;
        private static Stack<float> Rotations => P.Rotations;
        private static int HoveredCount => Hovered.Count;
        private static int ContextCount => Contexts.Count;
        private static int RotationCount => Contexts.Count;
        private static readonly int MIN_ITEMS = 3;
        private static readonly int MIN_ITEMS_PER_LEVEL = 3;
        public static void DebugMenu()
        {
            ImGui.Begin("MZRadialDebugging");
            ImGui.Text("Context Count : " + ContextCount);
            ImGui.Text("Hovered Count : " + HoveredCount);
            ImGui.Text("Rotation Count : " + RotationCount);
            ImGui.End();
        }
        private static void PushRadialMenu(string name, Vector2 cent, bool open)
        {
            int Idx = -1;
            if (HoveredCount > 0)
            {
                Idx = Hovered.Pop();
            }
            float Rotation = -IM_PI / 2f;
            if (RotationCount > 0)
            {
                Rotation = Rotations.Pop();
            }
            RadialMenuContext ctx = new()
            {
                Name = name,
                IdxHovered = Idx,
                Rotation = Rotation,
                RADIUS_MAX = 120.0f,
                RADIUS_MIN = 30.0f,
                center = cent,
                Items = new(),
                Open = open
            };
            Contexts.Push(ctx);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0, 0, 0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 1.0f);
            ImGui.SetNextWindowBgAlpha(0f);
        }
        public static bool BeginRadialPopup(string name, Vector2 center, bool open)
        {
            PushRadialMenu(name, center, open);
            var ctx = Contexts.Peek();
            var opened = ImGui.BeginPopup(ctx.Name);
            if (opened)
            {
                return true;
            }
            else
            {
                Contexts.Pop();
                return false;
            }
        }
        public static bool BeginRadialPopup(string name, bool open)
        {
            if (open)
            {
                if (ImGui.GetFrameCount() > P.last + 1)
                {
                    P.Center = ImGui.GetIO().MousePos;
                }
                P.last = ImGui.GetFrameCount();
            }
            PushRadialMenu(name, P.Center, open);
            var ctx = Contexts.Peek();
            var opened = ImGui.BeginPopup(ctx.Name);
            if (opened)
            {
                return true;
            }
            else
            {
                Contexts.Pop();
                return false;
            }
        }
        public static bool BeginRadialMenu(string name)
        {
            if (Contexts.Count > 0)
            {
                var pctx = Contexts.Peek();
                pctx.Items.Add(new()
                {
                    Title = name,
                    IsSubMenu = true
                });
                if ((pctx.Items.Count - 1) == pctx.IdxHovered)
                {
                    int Idx = -1;
                    if (HoveredCount > 0)
                    {
                        Idx = Hovered.Pop();
                    }
                    float Rotation = pctx.Rotation;
                    if (RotationCount > 0)
                    {
                        Rotation = Rotations.Pop();
                    }
                    RadialMenuContext ctx = new()
                    {
                        Name = name,
                        IdxHovered = Idx,
                        Rotation = Rotation,
                        RADIUS_MIN = pctx.RADIUS_MAX,
                        RADIUS_MAX = pctx.RADIUS_MAX + (pctx.RADIUS_MAX - pctx.RADIUS_MIN),
                        center = pctx.center,
                        Items = new(),
                        Open = pctx.Open,
                    };
                    Contexts.Push(ctx);
                    return true;
                }
            }
            return false;
        }
        public static bool RadialMenuItem(string name)
        {
            var ctx = Contexts.Peek();
            RadialMenuElem elem = new()
            {
                Title = name,
                IsSubMenu = false
            };
            ctx.Items.Add(elem);
            if ((ctx.Items.Count - 1) == ctx.IdxHovered && !ctx.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void RadialMenuItem(string name, Action<string> cb)
        {
            if (RadialMenuItem(name)) cb(name);
        }
        public static RadialMenuContext EndRadialMenu()
        {
            var ctx = Contexts.Pop();
            var delta = new Vector2(ImGui.GetIO().MousePos.X - ctx.center.X, ImGui.GetIO().MousePos.Y - ctx.center.Y);
            var drag_dist = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            var style = ImGui.GetStyle();
            ImDrawListPtr list = ImGui.GetWindowDrawList();
            list.PushClipRectFullScreen();
            list.PathArcTo(ctx.center, (ctx.RADIUS_MIN + ctx.RADIUS_MAX) * 0.5f, 0.0f, 2.0f * 0.99f * IM_PI, 32);
            list.PathStroke(ImGui.GetColorU32(ImGuiCol.WindowBg), ImDrawFlags.Closed, (float)(ctx.RADIUS_MAX - ctx.RADIUS_MIN));
            float item_arc_span = 2.0f * IM_PI / Math.Max(ctx.Items.Count, MIN_ITEMS + MIN_ITEMS_PER_LEVEL * (ContextCount + 1));
            float drag_angle = (float)Math.Atan2(delta.Y, delta.X);
            if (ContextCount > 0)
                ctx.Rotation -= item_arc_span * (ctx.Items.Count - 1f) / 2f;
            for (int item = 0; item < ctx.Items.Count; item++)
            {
                string ilabel = ctx.Items[item].Title;
                float MinItemSpacing = style.ItemInnerSpacing.X / (ctx.RADIUS_MIN * 2.0f);
                float MaxItemSpacing = style.ItemInnerSpacing.X / (ctx.RADIUS_MAX * 2.0f);
                float item_ang_min = item_arc_span * (item - 0.5f + MinItemSpacing) + ctx.Rotation;
                float item_ang_max = item_arc_span * (item + 0.5f - MaxItemSpacing) + ctx.Rotation;
                if (drag_dist >= ctx.RADIUS_MIN && drag_dist <= ctx.RADIUS_MAX)
                {
                    while ((drag_angle - item_ang_min) < 0f)
                        drag_angle += 2f * IM_PI;
                    while ((drag_angle - item_ang_min) > 2 * IM_PI)
                        drag_angle -= 2f * IM_PI;
                    if (drag_angle >= item_ang_min && drag_angle < item_ang_max)
                    {
                        ctx.IdxHovered = item;
                    }
                }
                else if (drag_dist < ctx.RADIUS_MIN)
                {
                    ctx.IdxHovered = -1;
                }
                int arc_segemnts = (int)(32 * item_arc_span / (2 * IM_PI)) + 1;
                uint color = ImGui.GetColorU32(ctx.IdxHovered == item ? (ctx.Items[ctx.IdxHovered].IsSubMenu ? ImGuiCol.ButtonHovered : ImGuiCol.ButtonActive) : ImGuiCol.Button);
                list.PathArcTo(ctx.center, ctx.RADIUS_MAX - style.ItemInnerSpacing.X, item_ang_min, item_ang_max, arc_segemnts);
                list.PathArcTo(ctx.center, ctx.RADIUS_MIN + style.ItemInnerSpacing.X, item_ang_max, item_ang_min, arc_segemnts);
                list.PathFillConvex(color);
                float RadCenter = (item_arc_span * item) + ctx.Rotation;
                if (ctx.Items[item].IsSubMenu)
                {
                    Vector2[] tri = new Vector2[3];

                    float RadLeft = RadCenter - 5.0f / ctx.RADIUS_MAX;
                    float RadRight = RadCenter + 5.0f / ctx.RADIUS_MAX;

                    tri[0] = new();
                    tri[0].X = ctx.center.X + (float)Math.Cos(RadCenter) * (ctx.RADIUS_MAX - 5.0f);
                    tri[0].Y = ctx.center.Y + (float)Math.Sin(RadCenter) * (ctx.RADIUS_MAX - 5.0f);
                    tri[1] = new();
                    tri[1].X = ctx.center.X + (float)Math.Cos(RadLeft) * (ctx.RADIUS_MAX - 10.0f);
                    tri[1].Y = ctx.center.Y + (float)Math.Sin(RadLeft) * (ctx.RADIUS_MAX - 10.0f);
                    tri[2] = new();
                    tri[2].X = ctx.center.X + (float)Math.Cos(RadRight) * (ctx.RADIUS_MAX - 10.0f);
                    tri[2].Y = ctx.center.Y + (float)Math.Sin(RadRight) * (ctx.RADIUS_MAX - 10.0f);

                    list.AddTriangleFilled(tri[0], tri[1], tri[2], ImGui.GetColorU32(new Vector4(1f)));
                }
                Vector2 text_size = ImGui.CalcTextSize(ilabel, 0.0f);
                Vector2 text_pos = new(
                    ctx.center.X + (float)Math.Cos((item_ang_min + item_ang_max) * 0.5f) * (ctx.RADIUS_MIN + ctx.RADIUS_MAX) * 0.5f - text_size.X * 0.5f,
                    ctx.center.Y + (float)Math.Sin((item_ang_min + item_ang_max) * 0.5f) * (ctx.RADIUS_MIN + ctx.RADIUS_MAX) * 0.5f - text_size.Y * 0.5f);
                list.AddText(text_pos, ImGui.GetColorU32(ImGuiCol.Text), ilabel);
            }
            if (!ctx.Open && ContextCount == 0)
            {
                ImGui.CloseCurrentPopup();
            }
            if (ctx.Open)
            {
                Rotations.Push(item_arc_span * ctx.IdxHovered + ctx.Rotation);
                if (ctx.IdxHovered != -1)
                {
                    Hovered.Push(ctx.IdxHovered);
                }
            }
            list.PopClipRect();
            return ctx;
        }
        public static void EndRadialPopup()
        {
            ImGui.PopStyleColor(2);
            ImGui.PopStyleVar(2);
            ImGui.EndPopup();
        }
    }
    public static class RadialMenuHelpers
    {
        public static void SubMenu(string name, List<(string, Action<string>)> items)
        {
            if (AdvRadialMenu.BeginRadialMenu(name))
            {
                foreach (var item in items)
                {
                    AdvRadialMenu.RadialMenuItem(item.Item1, item.Item2);
                }
                AdvRadialMenu.EndRadialMenu();
            }
        }
        public static void RadialMenu(string name, List<(string, Action<string>)> items, ref bool open)
        {
            if (AdvRadialMenu.BeginRadialPopup(name, open))
            {
                foreach (var item in items)
                {
                    AdvRadialMenu.RadialMenuItem(item.Item1, item.Item2);
                }
                AdvRadialMenu.EndRadialMenu();
            }
            AdvRadialMenu.EndRadialPopup();
        }
    }
}