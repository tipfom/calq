using CSharpMath.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Calq
{
    class LogMathView : MathView
    {
        public LogMathView()
        {
        }

        public override SizeRequest GetSizeRequest(double widthConstraint, double heightConstraint)
        {
            var rect = base.Measure;
            if (rect == null) return base.GetSizeRequest(widthConstraint, heightConstraint);
            return new SizeRequest(new Size(rect.Value.Width, rect.Value.Height+5));
        }
    }
}
