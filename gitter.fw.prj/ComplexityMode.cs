using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gitter.Framework
{
     
    public enum ComplextyModeVariants
    {
        simple,
        standard,
        advanced
    }

    public class ComplexityMode
    {
        private ComplextyModeVariants _mode = ComplextyModeVariants.advanced;

        public ComplextyModeVariants Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public string Name
        {
            get { return _mode.ToString(); }
        }
    }

    

}
