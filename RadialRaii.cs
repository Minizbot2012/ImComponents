using System;

namespace ImComponents.Raii
{
    public class RadialMenu(bool open) : IDisposable
    {
        public bool open = ImComponents.RadialMenu.Instance.BeginRadialPopup(open);

        public void Dispose()
        {
            if (open)
                ImComponents.RadialMenu.Instance.EndRadialMenu();
        }
        // Optional Helpers
        public bool RadialMenuItem(string name)
        {
            return ImComponents.RadialMenu.Instance.RadialMenuItem(name);
        }
        public SubMenu Menu(string name)
        {
            return new SubMenu(name);
        }
    }

    public class SubMenu(string name) : IDisposable
    {
        public bool open = ImComponents.RadialMenu.Instance.BeginRadialMenu(name);

        public void Dispose()
        {
            if (open)
            {
                ImComponents.RadialMenu.Instance.EndRadialMenu();
            }
        }
        public bool RadialMenuItem(string name)
        {
            return ImComponents.RadialMenu.Instance.RadialMenuItem(name);
        }
        public SubMenu Menu(string name)
        {
            return new SubMenu(name);
        }
    }
}