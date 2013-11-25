using System;
using System.Windows;
using TimeLogger.Core;
using TimeLogger.Models;
using TimeLogger.ViewModels;
using TimeLogger.Views.Modal;

namespace TimeLogger.Domain
{
    /// <summary>
    /// Manages the initialisation and launching of the prompt window.
    /// </summary>
    public class PromptManager : IPromptManager
    {
        private readonly Window _prompt;
        private readonly PromptViewModel _viewModel;

        public PromptManager(Settings settingsModel)
        {
            _viewModel = new PromptViewModel(settingsModel);
            _prompt = new PromptWindow(_viewModel);
            _viewModel.AfterSleep(Hide);
            _viewModel.AfterWake(Show);
        }

        private void Show()
        {
            _prompt.Dispatcher.Invoke(() => _prompt.Show());
        }

        private void Hide()
        {
            _prompt.Dispatcher.Invoke(() => _prompt.Hide());
        }

        public void Prompt(TimeSpan sleepAllowance, Action continueAction)
        {
            _viewModel.Reset(sleepAllowance, _ =>
            {
                continueAction();
                Hide();
            });
            Show();
            _viewModel.StartCountdown();
        }
    }
}