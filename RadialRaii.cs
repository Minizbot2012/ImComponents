using System;

namespace ImComponents.Raii
{

    //Eventually gonna re-wire everything to this class.... it'll be fun
    public interface IMenu {
        SubMenu Menu(string name);
        bool RadialMenuItem(string name);
    }
    public class RadialMenu(bool open) : IDisposable, IMenu
    {
        public bool open = ImComponents.RadialMenu.Instance.BeginRadialPopup(open);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
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

    public class SubMenu(string name) : IDisposable, IMenu
    {
        public bool open = ImComponents.RadialMenu.Instance.BeginRadialMenu(name);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
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