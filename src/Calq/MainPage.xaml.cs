using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calq.Core;
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
            string s = "1/x+ln(e^x)";
            testButton.Text = Function.ToPrefix(s);
            System.Diagnostics.Debug.WriteLine(testButton.Text);
            Term t = Term.TermFromMixedString(s);
            System.Diagnostics.Debug.WriteLine(t.ToString());

        }
    }
}
