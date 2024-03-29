﻿using System.Windows;
using CosmosDBStudio.ViewModel.Services;

namespace CosmosDBStudio.Services.Implementation
{
    public class ClipboardService : IClipboardService
    {
        public bool TryGetText(out string text)
        {
            if (Clipboard.ContainsText())
            {
                text = Clipboard.GetText();
                return true;
            }

            text = string.Empty;
            return false;
        }
    }
}
