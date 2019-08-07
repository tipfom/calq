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

        const string ALLOWED_CHARACTERS = "abcdefghijklmnopqrstuvwxyz()π∫1234567890,.*/-+^{} ";

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
        public ObservableCollection<object> Log { get; set; } = new ObservableCollection<object>();

        public Command EvaluateExpressionCommand { get; private set; }

        public string OnlineText { get { return WebHelper.IsOnline ? "Online" : "Offline"; } }

        public Color OnlineColor { get { return WebHelper.IsOnline ? Color.Green : Color.Red; } }

        public MainPageViewModel()
        {
            EvaluateExpressionCommand = new Command(EvaluateExpression);
            WebHelper.IsOnlineChanged += () => {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OnlineText"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OnlineColor"));
            };
        }

        private void EvaluateExpression()
        {
            try
            {
                if (Term.CheckBracketCount(Expression))
                {
                    Term t = Term.TermFromMixedString(Expression);
                    string expLat = t.ToLaTeX();
                    var exp = t.GetAsExpression();
                    if (WebHelper.IsOnline && false)
                    {
                        t = Term.TermFromMixedString(Infix.Format(exp));
                        exp = t.Evaluate();

                    }

                    string normal = Infix.Format(exp);
                    string expandet = Infix.Format(Algebraic.Expand(exp));

                    Log.Add(new Logging.ExpressionResult() { ExpressionLaTeX =  expLat, ResultLaTeX= Term.TermFromMixedString(Infix.Format(exp)).ToLaTeX()});
                }
                else System.Diagnostics.Debug.WriteLine("Bracket Missmatch");
            }
            catch(InvalidParameterCountException e)
            {
                System.Diagnostics.Debug.WriteLine("Invalid parameter count at " + e.Message);
            }
            catch(MissingArgumentException e)
            {
                System.Diagnostics.Debug.WriteLine("Missing argument");
            }
        }
    }
}