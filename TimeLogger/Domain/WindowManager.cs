using System;
using System.Collections.Concurrent;
using System.Windows;

namespace TimeLogger.Domain
{
    public class WindowManager : IWindowManager
    {
        private readonly ConcurrentDictionary<string, Window> _windows;

        public WindowManager()
        {
            _windows = new ConcurrentDictionary<string, Window>();
        }

        public void Remove(string name)
        {
            Window tmpWindow;
            _windows.TryRemove(name, out tmpWindow);
        }

        public void Register<TWindow>(string name) where TWindow : Window
        {
            Register((TWindow)null, name);
        }

        public void Register(Window view, string name)
        {
            _windows[name] = view;
        }

        public bool IsVisible(string name)
        {
            Window tmpWindow;
            if (_windows.TryGetValue(name, out tmpWindow))
            {
                return tmpWindow.IsVisible;
            }
            return false;
        }

        public void Show(string name)
        {
            Window tmpWindow;
            if (_windows.TryGetValue(name, out tmpWindow))
            {
                tmpWindow.Show();
            }
        }

        public void Hide(string name)
        {
            Window tmpWindow;
            if (_windows.TryGetValue(name, out tmpWindow))
            {
                tmpWindow.Hide();
            }
        }
    }
}