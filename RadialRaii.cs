using System;
using System.Collections.Generic;
using System.Numerics;
using MZCommon;

namespace ImComponents.Raii
{
    //Eventually gonna re-wire everything to this class.... it'll be fun
    public interface IMenu
    {
        SubMenu Menu(string name);
        bool RadialMenuItem(string name);
    }
    public class RadialMenu : IDisposable, IMenu
    {
        public bool open;
        public RadialMenu(bool open)
        {
            this.open = ImComponents.RadialMenu.Instance.BeginRadialPopup(open);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (open)
                ImComponents.RadialMenu.Instance.EndRadialMenu();
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

    public class SubMenu : IDisposable, IMenu
    {
        public bool open;
        public SubMenu(string name)
        {
            open = ImComponents.RadialMenu.Instance.BeginRadialMenu(name);
        }
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