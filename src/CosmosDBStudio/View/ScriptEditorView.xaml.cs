using System;
using System.Windows;
using System.Windows.Controls;

namespace CosmosDBStudio.View
{
    /// <summary>
    /// Interaction logic for ScriptEditorView.xaml
    /// </summary>
    public partial class ScriptEditorView : UserControl
    {
        public ScriptEditorView()
        {
            InitializeComponent();
        }

        public ToolBar? AdditionalToolBar
        {
            get { return (ToolBar?)GetValue(AdditionalToolBarProperty); }
            set { SetValue(AdditionalToolBarProperty, value); }
        }

        public static readonly DependencyProperty AdditionalToolBarProperty =
            DependencyProperty.Register(
                "AdditionalToolBar",
                typeof(ToolBar),
                typeof(ScriptEditorView),
                new PropertyMetadata(null, OnAdditionalToolBarPropertyChanged));

        private static void OnAdditionalToolBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ScriptEditorView)?.OnAdditionalToolBarChanged();
        }

        private void OnAdditionalToolBarChanged()
        {
            while (toolBarTray.ToolBars.Count > 1)
            {
                toolBarTray.ToolBars.RemoveAt(toolBarTray.ToolBars.Count - 1);
            }

            if (AdditionalToolBar != null)
                toolBarTray.ToolBars.Add(AdditionalToolBar);
        }
    }
}
