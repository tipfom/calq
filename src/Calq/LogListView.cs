using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Calq
{
    class LogListView : ListView
    {
        public static readonly BindableProperty ItemTemplateSelectorProperty = BindableProperty.Create("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(LogListView), null, propertyChanged: OnDataTemplateSelectorChanged);
        private DataTemplateSelector currentItemSelector; 
        private static void OnDataTemplateSelectorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((LogListView)bindable).OnDataTemplateSelectorChanged((DataTemplateSelector)oldvalue, (DataTemplateSelector)newvalue);
        }
        protected virtual void OnDataTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue)
        {
            // check to see we don't have an ItemTemplate set
            if (ItemTemplate != null && newValue != null)
                throw new ArgumentException("Cannot set both ItemTemplate and ItemTemplateSelector", "ItemTemplateSelector");
            currentItemSelector = newValue;
        }
        protected override Cell CreateDefault(object item)
        {
            if (currentItemSelector != null)
            {
                var template = currentItemSelector.SelectTemplate(item, this);
                if (template != null)
                {
                    var templateInstance = template.CreateContent();
                    // see if it's a view or a cell
                    var templateView = templateInstance as View;
                    var templateCell = templateInstance as Cell;
                    if (templateView == null && templateCell == null)
                        throw new InvalidOperationException("DataTemplate must be either a Cell or a View");
                    if (templateView != null) // we got a view, wrap in a cell
                        templateCell = new ViewCell { View = templateView };
                    return templateCell;
                }
            }
            return base.CreateDefault(item);
        }
        public DataTemplateSelector ItemTemplateSelector {
            get {
                return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
            }
            set {
                SetValue(ItemTemplateSelectorProperty, value);
            }
        }
    }
}
