using System;

namespace MenuSystem
{
    public class MenuItem
    {
        public virtual string Label { get; set; }
        public virtual string UserChoice { get; set; }
        public virtual Func<string>? MethodToExecute { get; set; }
        public bool IsPredefined { get; set; }

        public MenuItem(string label, string userChoice, Func<string>? methodToExecute, bool predefined = false)
        {
            Label = label.Trim();
            UserChoice = userChoice.Trim();
            MethodToExecute = methodToExecute;
            IsPredefined = predefined;
        }

        public override string ToString()
        {
            return UserChoice + ") " + Label;
        }
    }
}