namespace CosmosDBStudio.ViewModel.Messages
{
    public class SetStatusBarMessage
    {
        public SetStatusBarMessage(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
