using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CosmosDBStudio.View
{
    /// <summary>
    /// Interaction logic for StoredProcedureParametersView.xaml
    /// </summary>
    public partial class StoredProcedureParametersView : UserControl
    {
        public StoredProcedureParametersView()
        {
            InitializeComponent();
        }

        public ICommand? ExecuteCommand
        {
            get { return (ICommand?)GetValue(ExecuteCommandProperty); }
            set { SetValue(ExecuteCommandProperty, value); }
        }

        public static readonly DependencyProperty ExecuteCommandProperty =
            DependencyProperty.Register("ExecuteCommand", typeof(ICommand), typeof(StoredProcedureParametersView), new PropertyMetadata(null));

    }
}
