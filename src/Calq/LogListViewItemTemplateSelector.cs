using Calq.Logging;
using Xamarin.Forms;

namespace Calq
{
    public class LogListViewItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ShowLatex { get; set; }
        public DataTemplate ShowError { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item.GetType() == typeof(ExpressionResult))
                return ShowLatex;
            if (item.GetType() == typeof(ErrorResult))
                return ShowError;
            return null;
        }
    }
}
