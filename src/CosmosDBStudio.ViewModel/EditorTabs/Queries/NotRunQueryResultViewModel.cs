using System;
using System.Collections.Generic;

namespace CosmosDBStudio.ViewModel.EditorTabs.Queries
{
    public class NotRunQueryResultViewModel : QueryResultViewModelBase
    {
        public override IReadOnlyList<ResultItemViewModel> Items => Array.Empty<ResultItemViewModel>();

        public override bool IsJson => false;

        public override string Text => string.Empty;

        public override string? Error => null;

        public override ResultItemViewModel? SelectedItem { get; set; }
    }
}