using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using MVVM.Extensions;
using TimeLogger.Wpf.ViewModels;

namespace TimeLogger.UserInterface.ViewModels
{
    public class TaskTrayViewModel : ObservableObject
    {
        private readonly IApplication _app;
        private IComposer _composer;
        private readonly object _composerLock = new object();

        public TaskTrayViewModel(IApplication app)
        {
            _app = app;
            _composer = null;
            MenuItems = new ObservableCollection<MenuItemDto>();
            StatelessItems = new List<MenuItemDto>();
            RefreshCommand = new DelegateCommand(RefreshMenu);
            RefreshMenu();
        }

        private void RefreshMenu()
        {
            MenuItems.Clear();
            if (StatelessItems.Any())
            {
                foreach (var item in StatelessItems)
                {
                    MenuItems.Add(item);
                }
                MenuItems.Add(MenuItemDto.Separator);
            }

            bool hasComposer;
            bool isEnding;
            ComposerState state;
            lock (_composerLock)
            {
                hasComposer = _composer != null;
                isEnding = hasComposer && _composer.Ending;
                state = hasComposer ? _composer.GetState() : null
            }
            if (!hasComposer)
            {
                MenuItems.Add(new MenuItemDto() {
                    Header = "Clock In",
                    Command = new AsyncDelegateCommand(ClockIn)
                });
                MenuItems.Add(new MenuItemDto() {
                    Header = "Quit",
                    Command = new AsyncDelegateCommand(ExitApplication)
                });
            }
            else if (!isEnding)
            {
                MenuItems.Add(MenuItemDto.CreateInfo(state.Description));
                if (!state.IsLogging && state.HasTimeToLog)
                    MenuItems.Add(new MenuItemDto()
                        {
                            Header = "Log Work",
                            Command = new AsyncDelegateCommand(LogWork)
                        });

                MenuItems.Add(new MenuItemDto()
                    {
                        Header = "Clock Out",
                        Command = new AsyncDelegateCommand(ClockOut)
                    });
                MenuItems.Add(new MenuItemDto()
                    {
                        Header = "Quit",
                        Command = new AsyncDelegateCommand(ExitApplication)
                    });   
            }
            else
            {
                MenuItems.Add(MenuItemDto.CreateInfo("FUCKOFF IM BUSY!"));
            }
        }

        private void LogWork()
        {
            lock (_composerLock)
            {
                if(_composer != null)
                    _composer.LogWork(_app.GetTime());
            }
        }

        private void ClockOut()
        {
            IComposer composer;
            lock (_composerLock)
            {
                if (_composer == null)
                    return;

                composer = _composer;
                _composer = null;

                composer.Ending = true;
                composer.LogWork(_app.GetEndTime());
            }
            
            var disposable = composer as IDisposable;
            if(disposable != null)
                disposable.Dispose();
        }

        private void ClockIn()
        {
            var composer = _app.CreateComposer();
            lock (_composerLock)
            {
                _composer = composer;
            }
        }

        private void ExitApplication()
        {
            ClockOut();
            _app.Shutdown();
        }

        public ObservableCollection<MenuItemDto> MenuItems { get; set; }
        private List<MenuItemDto> StatelessItems { get; set; } 

        public ICommand RefreshCommand { get; set; }

        public void AddHttpItem(string name, Uri url)
        {
            StatelessItems.Add(new MenuItemDto()
                {
                    Header = name,
                    Command = new DelegateCommand(() => Process.Start(url.AbsoluteUri))
                });
        }
    }
}
