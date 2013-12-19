using System.Windows.Input;

namespace TimeLogger.UserInterface.ViewModels
{
    public class MenuItemDto
    {
        public string Header { get; set; }
        public ICommand Command { get; set; }

        public static MenuItemDto Separator {
            get
            {
                return new MenuItemDto() {Header = "--------"}
            }
        }

        public static MenuItemDto CreateInfo(string description)
        {
            return new MenuItemDto() {Header = description};
        }
    }
}