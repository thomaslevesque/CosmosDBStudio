using CosmosDBStudio.Model;

namespace CosmosDBStudio.ViewModel.Messages
{
    public class AccountRemovedMessage
    {
        public AccountRemovedMessage(CosmosAccount account)
        {
            Account = account;
        }

        public CosmosAccount Account { get; }
    }
}
