using CosmosDBStudio.Model;

namespace CosmosDBStudio.Messages
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
