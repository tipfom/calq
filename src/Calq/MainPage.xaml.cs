using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calq.Core;
using MathNet.Symbolics;
using Xamarin.Forms;

namespace Calq
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //string s = "0 + 1 * 1 * (x - 2) / (1 - 2) * (x - 3) / (1 - 3) * (x - 4) / (1 - 4) + 8 * 1 * (x - 1) / (2 - 1) * (x - 3) / (2 - 3) * (x - 4) / (2 - 4) + 27 * 1 * (x - 1) / (3 - 1) * (x - 2) / (3 - 2) * (x - 4) / (3 - 4) + 64 * 1 * (x - 1) / (4 - 1) * (x - 2) / (4 - 2) * (x - 3) / (4 - 3)";
            string s = "π + 1 * (x - 1) + sqrt(x) - cos(π) * e^x";
            Expression exp = Infix.ParseOrThrow(s);
            System.Diagnostics.Debug.WriteLine(Infix.Format(exp));

            //testButton.Text = Function.ToPrefix(s);
            //System.Diagnostics.Debug.WriteLine(testButton.Text);
            Term t = Term.TermFromMixedString(s);
            System.Diagnostics.Debug.WriteLine(t.ToString());

            System.Diagnostics.Debug.WriteLine(Infix.Format(t.Evaluate()));
            exp = Infix.ParseOrThrow(Infix.Format(t.Evaluate()));
            System.Diagnostics.Debug.WriteLine(Infix.Format(exp));

        }
    }
}
