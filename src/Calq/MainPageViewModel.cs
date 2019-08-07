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

        const string ALLOWED_CHARACTERS = "abcdefghijklmnopqrstuvwxyz()π1234567890,.*/-+^{} ";

        private string _Expression;
        public string Expression {
            get { return _Expression; }
            set {
                foreach (char c in value)
                {
                    if (!ALLOWED_CHARACTERS.Contains(c.ToString()))
                    {
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Expression"));
                        return;
                    }
                }
                _Expression = value;
            }
        }
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