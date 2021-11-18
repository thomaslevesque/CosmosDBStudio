using CosmosDBStudio.Model;

namespace CosmosDBStudio.Messages
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
