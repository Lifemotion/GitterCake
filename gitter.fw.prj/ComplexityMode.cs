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

    public class ComplexityMode
    {
        private List<ItemVisisbility> _itemsList=new List<ItemVisisbility>();

        public ComplexityMode()
        {
           // _itemsList.Add(new ItemVisisbility("", ComplextyModeVariants.advanced));
            _itemsList.Add(new ItemVisisbility("BranchMenuViewReflog", Complexty.advanced));
            _itemsList.Add(new ItemVisisbility("BranchMenuResetHead", Complexty.advanced));
            _itemsList.Add(new ItemVisisbility("BranchMenuRebaseHere", Complexty.advanced));
            _itemsList.Add(new ItemVisisbility("", Complexty.advanced));
        }

        private class ItemVisisbility
        {
            private string _name;
            private Complexty _visibleFrom;

            public ItemVisisbility (string name, Complexty visibleFrom)
            {
                _name=name;
                _visibleFrom=visibleFrom;
            }

            public bool Check(Complexty mode)
            {
                if (ComplexityMode.ComplexityLevelOfMode(mode) >= ComplexityMode.ComplexityLevelOfMode(_visibleFrom))
                {
                    return true;
                }
                return false;
            }

            public string Name
            {
                get
                {
                    return (_name);
                }
            }
        }

        public bool CheckVisibility(string name)
        {
            foreach (var elem in _itemsList)
            {
                if (elem.Name == name) { return elem.Check(_currentMode); }
            }
            throw new Exception();
        }

        private Complexty _currentMode = Complexty.advanced;

        public Complexty Mode
        {
            get { return _currentMode; }
            set { _currentMode = value; }
        }

        public bool IsItemVisible(Complexty complexityMode)
        {
            if (ComplexityMode.ComplexityLevelOfMode(_currentMode) >= ComplexityMode.ComplexityLevelOfMode(complexityMode))
            {
                return true;
            }
            return false;
        }

        public static int ComplexityLevelOfMode(Complexty mode)
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
