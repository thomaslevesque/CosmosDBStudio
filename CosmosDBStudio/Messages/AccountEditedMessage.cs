using CosmosDBStudio.Model;

namespace CosmosDBStudio.Messages
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
    }
}
