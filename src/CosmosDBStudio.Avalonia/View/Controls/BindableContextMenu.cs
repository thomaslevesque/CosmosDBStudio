using System;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Styling;
using CosmosDBStudio.ViewModel;

namespace CosmosDBStudio.Avalonia.View.Controls;

public class BindableContextMenu : ContextMenu, IStyleable
{
    Type IStyleable.StyleKey => typeof(ContextMenu);

    protected override IItemContainerGenerator CreateItemContainerGenerator()
    {
        return new BindableContextMenuItemContainerGenerator(this);
    }
    
    private class BindableContextMenuItemContainerGenerator : MenuItemContainerGenerator
    {
        public BindableContextMenuItemContainerGenerator(IControl owner) : base(owner)
        {
        }

        protected override IControl CreateContainer(object item)
        {
            if (item is CommandViewModel { IsSeparator: true })
                return new Separator();
            return base.CreateContainer(item);
        }
    }
}