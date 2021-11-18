using CosmosDBStudio.Model;

namespace CosmosDBStudio.ViewModel.Messages
{
    public class AccountEditedMessage
    {
        public AccountEditedMessage(CosmosAccount account, CosmosAccount oldAccount)
        {
            Account = account;
            OldAccount = oldAccount;
        }

        public CosmosAccount Account { get; }
        public CosmosAccount OldAccount { get; }
        public bool CredentialsChanged => Account.Endpoint != OldAccount.Endpoint || Account.Key != OldAccount.Key;
    }
}
