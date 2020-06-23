using CosmosDBStudio.ViewModel;
using System.Windows;
using System.Windows.Controls;

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

        private void items_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)e.OriginalSource;
            if (scrollViewer.ScrollableHeight > 0 &&
                scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight &&
                DataContext is QueryResultViewModel vm)
            {
                if (vm.LoadNextPageCommand.CanExecute(null))
                {
                    vm.LoadNextPageCommand.Execute(null);
                }
            }
        }
    }
}
