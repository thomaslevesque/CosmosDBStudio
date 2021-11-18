using CosmosDBStudio.Model;

namespace CosmosDBStudio.ViewModel.Messages
{
    public class AccountAddedMessage
    {
        public AccountAddedMessage(CosmosAccount account)
        {
            Account = account;
        }

        public CosmosAccount Account { get; }
    }
}
