using System;
using System.Windows;

namespace Walterlv.Whiteman
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnActivated(object sender, EventArgs e)
        {
            MovingCircle.IsAnimationEnabled = true;
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            MovingCircle.IsAnimationEnabled = false;
        }
    }
}
