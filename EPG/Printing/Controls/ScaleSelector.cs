using Prism.Mvvm;
using System;

namespace EPG.Printing.Controls
{
    public sealed class ScaleSelector : BindableBase
    {
        double scale = 1;
        public double Scale
        {
            get { return scale; }
            set { SetProperty(ref scale, value); }
        }

        public void Zoom(int delta)
        {
            Scale *= 1 + Math.Sign(delta) * 0.1;
        }
    }
}
