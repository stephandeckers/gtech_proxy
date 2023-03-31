using System;
using System.Windows;

namespace Proximus.ifh.DebugCCProxy.View
{
    public partial class ProxyCCView : Window 
    {
        private readonly ProxyCCViewmodel _viewModel;

        public ProxyCCView(ProxyCCViewmodel viewmodel)
        {
            InitializeComponent();
            _viewModel = viewmodel;
            DataContext = _viewModel;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonLaunch_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LaunchCC();
        }

        private void LaunchCCView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewModel.CloseForm();
            DataContext = null;
        }
    }
}