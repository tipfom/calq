using Calq.Core;
using MathNet.Symbolics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace Calq
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Expression { get; set; }

        public ObservableCollection<string> Log { get; set; } = new ObservableCollection<string>();

        public Command EvaluateExpressionCommand { get; private set; }

        public MainPageViewModel()
        {
            EvaluateExpressionCommand = new Command(EvaluateExpression);
        }

        private void EvaluateExpression()
        {
            Term t = Term.TermFromMixedString(Expression);
            var exp = Infix.ParseOrThrow(Infix.Format(t.Evaluate()));
            Log.Add($"\"{Expression}\": {Infix.Format(exp)}");
        }
    }
}