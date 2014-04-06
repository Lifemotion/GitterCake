using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace gitter.Framework
{
    public class DpiHelper
    {
        private double _kx,_ky;

        public DpiHelper(ContainerControl control)
        {
            var asf = control.AutoScaleDimensions;
            _kx = asf.Width / 96.0;
            _ky = asf.Height / 96.0;
        }

        public int ScaleIntX(int x)
        {
            return (int)((double)x * _kx);
        }

        public int ScaleIntY(int y)
        {
            return (int)((double)y * _ky);
        }

        public Rectangle ScaleRectangle(Rectangle rect)
        {
            var result = new Rectangle(ScaleIntX(rect.X), ScaleIntY(rect.Y), ScaleIntX(rect.Width), ScaleIntY(rect.Height));
            return result;
        }
    }
}
