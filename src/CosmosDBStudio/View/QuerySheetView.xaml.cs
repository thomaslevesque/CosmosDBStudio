﻿using System.Windows;
using System.Windows.Controls;

namespace CosmosDBStudio.View
{
    public partial class QuerySheetView : UserControl
    {
        public QuerySheetView()
        {
            InitializeComponent();
        }

        private void ParametersViewVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            editorColumn.Width = new GridLength(1, GridUnitType.Star);
            if (e.NewValue is false)
            {
                parametersColumn.MinWidth = 0;
                parametersColumn.Width = new GridLength(0);
            }
            else
            {
                parametersColumn.Width = new GridLength(300);
                parametersColumn.MinWidth = 200;
            }
        }
    }
}
