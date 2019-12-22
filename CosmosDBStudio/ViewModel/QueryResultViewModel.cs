using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;
using Hamlet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class QueryResultViewModel : QueryResultViewModelBase
    {
        private readonly QueryResult _result;
        private readonly IContainerContext _containerContext;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<ResultItemViewModel> _items;

        public QueryResultViewModel(
            QueryResult result,
            IContainerContext containerContext,
            IViewModelFactory viewModelFactory,
            IDialogService dialogService)
        {
            _result = result;
            _containerContext = containerContext;
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            if (result.Items != null && result.Items.Count > 0)
            {
                _items = new ObservableCollection<ResultItemViewModel>(
                    result.Items
                        .Select(item => _viewModelFactory.CreateDocumentViewModel(item, _containerContext)));
                Text = new JArray(result.Items).ToString(Formatting.Indented);
                IsJson = true;
                SelectedTab = ResultTab.Items;
            }
            else
            {
                ResultItemViewModel item;
                if (_result.Error != null)
                {
                    item = _viewModelFactory.CreateErrorItemPlaceholder();
                    SelectedTab = ResultTab.Error;
                }
                else
                {
                    item = _viewModelFactory.CreateEmptyResultPlaceholder();
                    SelectedTab = ResultTab.Items;
                }

                _items = new ObservableCollection<ResultItemViewModel>
                {
                    item
                };

                Text = item.Text;
                IsJson = false;
            }

            SelectedItem = Items.FirstOrDefault();
        }

        public override IReadOnlyList<ResultItemViewModel> Items => _items;
        public override bool IsJson { get; }
        public override string Text { get; }
        public override string? Error => _result?.Error?.Message;

        private ResultItemViewModel? _selectedItem;
        public override ResultItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value)
                .AndRaiseCanExecuteChanged(_editCommand)
                .AndRaiseCanExecuteChanged(_deleteCommand);
        }

        private AsyncDelegateCommand? _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand = new AsyncDelegateCommand(Refresh, CanRefresh);

        private async Task Refresh()
        {
            if (SelectedItem is DocumentViewModel item && item.IsRawDocument)
            {
                var refreshedDocument = await _containerContext.Documents.GetAsync(
                    item.Id,
                    item.HasPartitionKey ? Option.Some(item.PartitionKey) : Option.None(),
                    default);

                if (refreshedDocument is null)
                {
                    _dialogService.ShowError("The document no longer exists");
                    _items.Remove(item);
                    return;
                }

                var index = _items.IndexOf(item);
                SelectedItem = null;
                var newItem = _viewModelFactory.CreateDocumentViewModel(refreshedDocument, _containerContext);
                _items[index] = newItem;
                SelectedItem = newItem;
            }
        }

        private bool CanRefresh()
        {
            return SelectedItem is DocumentViewModel item && item.IsRawDocument;
        }

        private AsyncDelegateCommand? _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new AsyncDelegateCommand(DeleteAsync, CanDelete);

        private async Task DeleteAsync()
        {
            if (SelectedItem is DocumentViewModel item)
            {
                if (!_dialogService.Confirm("Do you really want to delete this document?"))
                    return;

                try
                {
                    await _containerContext.Documents.DeleteAsync(
                        item.Id,
                        item.HasPartitionKey ? Option.Some(item.PartitionKey) : Option.None(),
                        item.ETag,
                        default);

                    _items.Remove(item);
                }
                catch(Exception ex)
                {
                    _dialogService.ShowError(ex.Message);
                }
            }
        }

        private bool CanDelete()
        {
            return SelectedItem is DocumentViewModel item && item.HasId && item.IsRawDocument;
        }

        private DelegateCommand? _editCommand;
        public ICommand EditCommand => _editCommand ??= new DelegateCommand(Edit, CanEdit);

        private void Edit()
        {
            if (SelectedItem is DocumentViewModel item && item.GetDocument() is JObject document)
            {
                var vm = _viewModelFactory.CreateDocumentEditorViewModel(document, false, _containerContext);
                _dialogService.ShowDialog(vm);
                var modifiedDocument = vm.GetDocument();
                if (modifiedDocument != document && modifiedDocument != null)
                {
                    var index = _items.IndexOf(item);
                    SelectedItem = null;
                    var newItem = _viewModelFactory.CreateDocumentViewModel(modifiedDocument, _containerContext);
                    _items[index] = newItem;
                    SelectedItem = newItem;
                }
            }
        }

        private bool CanEdit()
        {
            return SelectedItem is DocumentViewModel item && item.IsRawDocument;
        }

        public string PartitionKeyPath => _containerContext.PartitionKeyPath ?? string.Empty;
    }
}