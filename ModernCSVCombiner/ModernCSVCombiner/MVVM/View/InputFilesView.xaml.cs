﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernCSVCombiner.MVVM.View
{
    /// <summary>
    /// InputFilesView.xaml の相互作用ロジック
    /// </summary>
    public partial class InputFilesView : UserControl
    {
        public InputFilesView()
        {
            InitializeComponent();
        }

        private void Flipper_OnIsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
    => System.Diagnostics.Debug.WriteLine($"Card is flipped = {e.NewValue}");
    }
}
