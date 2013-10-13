using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gitter.Framework
{
     
    public enum Complexty
    {
        simple,
        standard,
        advanced
    }

    public class ComplexityManager
    {
        public ComplexityManager()
        {
        }

        private Complexty _currentMode = Complexty.advanced;

        public Complexty Mode
        {
            get { return _currentMode; }
            set { _currentMode = value; }
        }

        public bool CurrentModeBiggerThan(Complexty complexityMode)
        {
            if (ComplexityManager.ComplexityModeToValue(_currentMode) >= ComplexityManager.ComplexityModeToValue(complexityMode))
            {
                return true;
            }
            return false;
        }

        public static int ComplexityModeToValue(Complexty mode)
        {
                 if (mode == Complexty.advanced) { return 100; }
                if (mode == Complexty.standard) { return 60; }
                if (mode == Complexty.simple) { return 10; }
                throw new Exception();
        }


        public string Name
        {
            get { return _currentMode.ToString(); }
        }
    }

    

}
