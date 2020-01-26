using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CosmosDBStudio.View
{
    /// <summary>
    /// Interaction logic for QueryResultsView.xaml
    /// </summary>
    public partial class QueryResultsView : UserControl
    {
        public QueryResultsView()
        {
            this.DataContextChanged += OnDataContextChanged;
            InitializeComponent();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResizeGridViewColumns();
        }

        private void ResizeGridViewColumns()
        {
            var view = (GridView)items.View;
            foreach (var column in view.Columns)
            {
                ResizeGridViewColumn(column);
            }
        }

        private void ResizeGridViewColumn(GridViewColumn column)
        {
            if (double.IsNaN(column.Width))
            {
                column.Width = column.ActualWidth;
            }

            column.Width = double.NaN;
        }

    }
}
