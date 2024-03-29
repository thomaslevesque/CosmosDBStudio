﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel.Dialogs
{
    public class DialogViewModelBase : BindableBase, IDialogViewModel
    {
        private readonly ObservableCollection<DialogButton> _buttons = new ObservableCollection<DialogButton>();

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            protected set => Set(ref _title, value);
        }

        public IEnumerable<DialogButton> Buttons => _buttons;

        public bool HasButtons => _buttons.Count > 0;

        public event EventHandler<bool?>? CloseRequested;

        protected void AddButton(DialogButton button)
        {
            _buttons.Add(button);
        }

        protected void RemoveButton(DialogButton button)
        {
            _buttons.Remove(button);
        }

        protected void AddOkButton(Action<DialogButton>? customize = null)
        {
            var button = new DialogButton
            {
                Text = "OK",
                IsDefault = true,
                DialogResult = true
            };

            customize?.Invoke(button);
            AddButton(button);
        }

        protected void AddCancelButton(Action<DialogButton>? customize = null)
        {
            var button = new DialogButton
            {
                Text = "Cancel",
                IsCancel = true,
                DialogResult = false
            };

            customize?.Invoke(button);
            AddButton(button);
        }

        protected void Close(bool? dialogResult)
        {
            CloseRequested?.Invoke(this, dialogResult);
        }

        public virtual void OnClosed(bool? result)
        {
        }

        public virtual void OnClosing(DialogClosingEventArgs args)
        {
        }
    }
}
