using System.Numerics;
using ImGuiNET;
using MZCommon;

namespace ImComponents;

public sealed class AdvRadialMenu
{
    private AdvRadialMenu() {
        
    }
    public static AdvRadialMenu Instance => Singleton<AdvRadialMenu>.Instance;
    private static readonly float IM_PI = 3.14159265358979323846f;
    private static readonly int MIN_ITEMS = 3;
    private static readonly int MIN_ITEMS_PER_LEVEL = 3;
    private struct RootState
    {
        public Vector2 Center;
        public Stack<State> Items;
        public Stack<int> Hovered;
        public Stack<float> Rotations;
        public int last;
        public bool Open;
    }
    private RootState Root = new();
    private struct Elem
    {
        public bool IsSubMenu;
        public string Name;
    }
    private struct State
    {
        public float RADIUS_MIN;
        public float RADIUS_MAX;
        public float Rotation;
        public int IdxHovered;
        public Elem TS;
        public List<Elem> Items;
    }
    public bool BeginRadialPopup(string name, bool open)
    {
        if (open)
        {
            if (ImGui.GetFrameCount() > Root.last + 1)
            {
                Root.Center = ImGui.GetIO().MousePos;
                Root.Hovered = new();
                Root.Items = new();
                Root.Rotations = new();
            }
            Root.last = ImGui.GetFrameCount();
        }
        Root.Open = open;
        var Rotation = -IM_PI / 2.0f;
        State st = new()
        {
            TS = new()
            {
                IsSubMenu = true,
                Name = name,
            },
            Rotation = Rotation,
            IdxHovered = -1,
            Items = new(),
            RADIUS_MAX = 120.0f,
            RADIUS_MIN = 30.0f,
        };
        ImGui.SetNextWindowBgAlpha(0f);
        var opened = ImGui.BeginPopup(st.TS.Name);
        if (opened)
        {
            var list = ImGui.GetWindowDrawList();
            list.PushClipRectFullScreen();
            if (Root.Hovered.Count > 0)
            {
                st.IdxHovered = Root.Hovered.Pop();
            }
            Root.Items.Push(st);
        }
        return opened;
    }
    public bool BeginRadialMenu(string name)
    {
        if (Root.Items.Count > 0)
        {
            var pctx = Root.Items.Peek();
            Elem ThisItem = new()
            {
                IsSubMenu = true,
                Name = name,
            };
            var Rotation = pctx.Rotation;
            State st = new()
            {
                TS = ThisItem,
                Items = new(),
                Rotation = pctx.Rotation,
                IdxHovered = -1,
                RADIUS_MIN = pctx.RADIUS_MAX,
                RADIUS_MAX = pctx.RADIUS_MAX + (pctx.RADIUS_MAX - pctx.RADIUS_MIN),
            };
            pctx.Items.Add(ThisItem);
            if ((pctx.Items.Count - 1) == pctx.IdxHovered)
            {
                if (Root.Hovered.Count > 0)
                {
                    st.IdxHovered = Root.Hovered.Pop();
                }
                if (Root.Rotations.Count > 0)
                {
                    st.Rotation = Root.Rotations.Pop();
                }
                Root.Items.Push(st);
                return true;
            }
        }
        return false;
    }
    public bool RadialMenuItem(string name)
    {
        if (Root.Items.Count > 0)
        {
            var pctx = Root.Items.Peek();
            pctx.Items.Add(new Elem()
            {
                Name = name,
                IsSubMenu = false,
            });
            if ((pctx.Items.Count - 1) == pctx.IdxHovered)
            {
                if (Root.Hovered.Count > 0)
                {
                    Root.Hovered.Pop();
                }
                if (!Root.Open)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void RadialMenuItem(string name, Action<string> cb)
    {
        if (RadialMenuItem(name)) cb(name);
    }
    public void EndRadialMenu()
    {
        var ctx = Root.Items.Pop();
        var style = ImGui.GetStyle();
        var list = ImGui.GetWindowDrawList();
        var item_arc_span = 2.0f * IM_PI / Math.Max(ctx.Items.Count, MIN_ITEMS + MIN_ITEMS_PER_LEVEL * (Root.Items.Count + 1));
        var delta = new Vector2(ImGui.GetIO().MousePos.X - Root.Center.X, ImGui.GetIO().MousePos.Y - Root.Center.Y);
        var drag_angle = (float)Math.Atan2(delta.Y, delta.X);
        var drag_dist = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
        ctx.Rotation -= item_arc_span * (ctx.Items.Count - 1f) / 2f;
        for (int item = 0; item < ctx.Items.Count; item++)
        {
            string ilabel = ctx.Items[item].Name;
            float ItemSpacing = style.ItemInnerSpacing.X / (ctx.RADIUS_MIN * 2.0f);
            float item_inner_ang_min = item_arc_span * (item - 0.5f + ItemSpacing) + ctx.Rotation;
            float item_inner_ang_max = item_arc_span * (item + 0.5f - ItemSpacing) + ctx.Rotation;
            float item_outer_ang_min = item_arc_span * (item - 0.5f + ItemSpacing * (ctx.RADIUS_MIN / ctx.RADIUS_MAX)) + ctx.Rotation;
            float item_outer_ang_max = item_arc_span * (item + 0.5f - ItemSpacing * (ctx.RADIUS_MIN / ctx.RADIUS_MAX)) + ctx.Rotation;
            if (drag_dist >= ctx.RADIUS_MIN && drag_dist <= ctx.RADIUS_MAX)
            {
                while ((drag_angle - item_inner_ang_min) < 0f)
                    drag_angle += 2f * IM_PI;
                while ((drag_angle - item_inner_ang_min) > 2 * IM_PI)
                    drag_angle -= 2f * IM_PI;
                if (drag_angle >= item_inner_ang_min && drag_angle < item_inner_ang_max)
                {
                    ctx.IdxHovered = item;
                }
            }
            if (drag_dist < ctx.RADIUS_MIN)
            {
                ctx.IdxHovered = -1;
            }
            int arc_segemnts = (int)(32 * item_arc_span / (2 * IM_PI)) + 1;
            uint color = ImGui.GetColorU32(ctx.IdxHovered == item ? (ctx.Items[ctx.IdxHovered].IsSubMenu ? ImGuiCol.ButtonHovered : ImGuiCol.ButtonActive) : ImGuiCol.Button);
            list.PathArcTo(Root.Center, ctx.RADIUS_MAX - style.ItemInnerSpacing.X, item_outer_ang_min, item_outer_ang_max, arc_segemnts);
            list.PathArcTo(Root.Center, ctx.RADIUS_MIN + style.ItemInnerSpacing.X, item_inner_ang_max, item_inner_ang_min, arc_segemnts);
            list.PathFillConvex(color);
            float RadCenter = (item_arc_span * item) + ctx.Rotation;
            if (ctx.Items[item].IsSubMenu)
            {
                Vector2[] tri = new Vector2[3];

                float RadLeft = RadCenter - 5.0f / ctx.RADIUS_MAX;
                float RadRight = RadCenter + 5.0f / ctx.RADIUS_MAX;

                tri[0] = new();
                tri[0].X = Root.Center.X + (float)Math.Cos(RadCenter) * (ctx.RADIUS_MAX - 5.0f);
                tri[0].Y = Root.Center.Y + (float)Math.Sin(RadCenter) * (ctx.RADIUS_MAX - 5.0f);
                tri[1] = new();
                tri[1].X = Root.Center.X + (float)Math.Cos(RadLeft) * (ctx.RADIUS_MAX - 10.0f);
                tri[1].Y = Root.Center.Y + (float)Math.Sin(RadLeft) * (ctx.RADIUS_MAX - 10.0f);
                tri[2] = new();
                tri[2].X = Root.Center.X + (float)Math.Cos(RadRight) * (ctx.RADIUS_MAX - 10.0f);
                tri[2].Y = Root.Center.Y + (float)Math.Sin(RadRight) * (ctx.RADIUS_MAX - 10.0f);

                list.AddTriangleFilled(tri[0], tri[1], tri[2], ImGui.GetColorU32(new Vector4(1f)));
            }
            Vector2 text_size = ImGui.CalcTextSize(ilabel, 0.0f);
            Vector2 text_pos = new(
                Root.Center.X + (float)Math.Cos((item_inner_ang_min + item_inner_ang_max) * 0.5f) * (ctx.RADIUS_MIN + ctx.RADIUS_MAX) * 0.5f - text_size.X * 0.5f,
                Root.Center.Y + (float)Math.Sin((item_inner_ang_min + item_inner_ang_max) * 0.5f) * (ctx.RADIUS_MIN + ctx.RADIUS_MAX) * 0.5f - text_size.Y * 0.5f
            );
            list.AddText(text_pos, ImGui.GetColorU32(ImGuiCol.Text), ilabel);
        }
        if (Root.Items.Count == 0)
        {
            list.PopClipRect();
            ImGui.EndPopup();
            if (!Root.Open)
            {
                ImGui.CloseCurrentPopup();
            }
        }
        if (ctx.IdxHovered != -1)
        {
            Root.Hovered.Push(ctx.IdxHovered);
            if (ctx.Items[ctx.IdxHovered].IsSubMenu)
            {
                Root.Rotations.Push(item_arc_span * ctx.IdxHovered + ctx.Rotation);
            }
        }
    }
}