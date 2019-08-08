﻿using Calq.Core;
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
            WebHelper.IsOnlineChanged += () =>
            {
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
                    var x = new Variable("x");
                    Term t = new Logarithm(x) / x;
                    var k2 = t.Differentiate("x");
                    string expLat = t.ToString();
                    t = t.Evaluate();

                    Log.Insert(0, new Logging.ExpressionResult() { ExpressionLaTeX = expLat, ResultLaTeX = t.ToLaTeX() });
                }
                else
                {
                    Log.Insert(0, new Logging.ErrorResult() { ErrorMessage = "Bracket Missmatch" });
                }
            }
            catch (InvalidParameterCountException e)
            {
                Log.Insert(0, new Logging.ErrorResult() { ErrorMessage = "Invalid parameter count at " + e.Message });
            }
            catch (MissingArgumentException)
            {
                Log.Insert(0, new Logging.ErrorResult() { ErrorMessage = "Missing Argument" });
            }
        }
    }
}