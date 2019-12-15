using System;
using System.Windows;
using System.Windows.Controls;

namespace CosmosDBStudio.View.Controls
{
    public class RadioChoice : ListBox
    {
        static RadioChoice()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(RadioChoice),
                new FrameworkPropertyMetadata(typeof(RadioChoice)));
        }

        private readonly string _groupName = Guid.NewGuid().ToString();

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RadioChoice), new PropertyMetadata(Orientation.Vertical));

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RadioChoiceItem(_groupName);
        }
    }

    public class RadioChoiceItem : ListBoxItem
    {
        static RadioChoiceItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(RadioChoiceItem),
                new FrameworkPropertyMetadata(typeof(RadioChoiceItem)));
        }

        public RadioChoiceItem(string groupName)
        {
            GroupName = groupName;
        }

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNamePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey GroupNamePropertyKey =
            DependencyProperty.RegisterReadOnly(
                "GroupName",
                typeof(string),
                typeof(RadioChoiceItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty GroupNameProperty =
            GroupNamePropertyKey.DependencyProperty;
    }
}
