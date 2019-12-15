using System.Windows;
using System.Windows.Controls;

namespace CosmosDBStudio.View
{
    public partial class QuerySheetView : TabItem
    {
        public QuerySheetView()
        {
            InitializeComponent();
        }

        private void ParametersViewVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is false)
            {
                editorColumn.Width = new GridLength(1, GridUnitType.Star);
                parametersColumn.Width = GridLength.Auto;
            }
        }
    }
}
