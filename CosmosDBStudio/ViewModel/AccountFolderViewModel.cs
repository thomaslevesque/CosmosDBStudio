using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class AccountFolderViewModel : NonLeafTreeNodeViewModel
    {
        private readonly CosmosAccountFolder _folder;
        private readonly IAccountDirectory _accountDirectory;
        private readonly IViewModelFactory _viewModelFactory;

        public AccountFolderViewModel(
            CosmosAccountFolder folder,
            AccountFolderViewModel? parent,
            IAccountDirectory accountDirectory,
            IViewModelFactory viewModelFactory)
        {
            _folder = folder;
            Parent = parent;
            _accountDirectory = accountDirectory;
            _viewModelFactory = viewModelFactory;
        }

        public override string Text => _folder.Name;

        public string Name => _folder.Name;
        public string FullPath => _folder.FullPath;

        public override NonLeafTreeNodeViewModel? Parent { get; }

        protected override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var childNodes = _accountDirectory.GetChildNodes(_folder.FullPath);
            var result = new List<TreeNodeViewModel>();
            foreach (var node in childNodes)
            {
                var childVM = node switch
                {
                    CosmosAccount account => (TreeNodeViewModel)_viewModelFactory.CreateAccountViewModel(account, this),
                    CosmosAccountFolder folder => (TreeNodeViewModel)_viewModelFactory.CreateAccountFolderViewModel(folder, this),
                    _ => throw new Exception("Invalid node type")
                };

                result.Add(childVM);
            }

            return Task.FromResult<IEnumerable<TreeNodeViewModel>>(result);
        }
    }
}
