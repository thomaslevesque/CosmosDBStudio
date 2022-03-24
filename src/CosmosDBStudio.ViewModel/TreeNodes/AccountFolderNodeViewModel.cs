using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel.Commands;

namespace CosmosDBStudio.ViewModel.TreeNodes
{
    public class AccountFolderNodeViewModel : NonLeafTreeNodeViewModel
    {
        private readonly CosmosAccountFolder _folder;
        private readonly AccountCommands _accountCommands;
        private readonly IAccountContextFactory _accountContextFactory;
        private readonly IAccountDirectory _accountDirectory;
        private readonly IViewModelFactory _viewModelFactory;

        public AccountFolderNodeViewModel(
            CosmosAccountFolder folder,
            AccountFolderNodeViewModel? parent,
            AccountCommands accountCommands,
            IAccountContextFactory accountContextFactory,
            IAccountDirectory accountDirectory,
            IViewModelFactory viewModelFactory)
        {
            _folder = folder;
            Parent = parent;
            _accountCommands = accountCommands;
            _accountContextFactory = accountContextFactory;
            _accountDirectory = accountDirectory;
            _viewModelFactory = viewModelFactory;

            Commands = new[]
            {
                new CommandViewModel("Add account", _accountCommands.AddCommand, this)
            };
        }

        public override string Text => _folder.Name;

        public string Name => _folder.Name;
        public string FullPath => _folder.FullPath;

        public override NonLeafTreeNodeViewModel? Parent { get; }

        protected override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var childNodes = _accountDirectory.GetChildNodes(_folder.FullPath);
            var result = new List<TreeNodeViewModel>();
            foreach (var node in childNodes.OrderBy(n => n.DisplayName))
            {
                var childVM = node switch
                {
                    CosmosAccount account =>
                        (TreeNodeViewModel)_viewModelFactory.CreateAccountNode(
                            account,
                            _accountContextFactory.Create(account),
                            this),
                    CosmosAccountFolder folder => (TreeNodeViewModel)_viewModelFactory.CreateAccountFolderNode(folder, this),
                    _ => throw new Exception("Invalid node type")
                };

                result.Add(childVM);
            }

            return Task.FromResult<IEnumerable<TreeNodeViewModel>>(result);
        }

        public override IEnumerable<CommandViewModel> Commands { get; }
    }
}
