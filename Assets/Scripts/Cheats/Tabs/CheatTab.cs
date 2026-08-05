using SRF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using SRDebugger;

namespace Lightsaber.Cheats
{
    public abstract class CheatTab : ICheatTab
    {
        protected abstract string Name { get; }

        public abstract void Initialize();

        protected OptionDefinition AddButton(string buttonName, Action buttonAction)
        {
            OptionDefinition optionDefinition = OptionDefinition.FromMethod(buttonName, buttonAction, Name);
            SRDebug.Instance.AddOption(optionDefinition);
            return optionDefinition;
        }

        protected OptionDefinition AddOption<T>(string name, Func<T> getter, Action<T> setter)
        {
            OptionDefinition optionDefinition = OptionDefinition.Create(name, getter, setter, Name);
            SRDebug.Instance.AddOption(optionDefinition);
            return optionDefinition;
        }

        protected DropDownOptionDefinition AddDropdown<T>(string name, Func<T> getter, Action<T> setter,
            IEnumerable<DropDownOptionItem> items)
        {
            PropertyReference property = PropertyReference.FromLambda(getter, setter);
            DropDownOptionDefinition option = new DropDownOptionDefinition(name, property, items, category: Name);
            SRDebug.Instance.AddOption(option);
            return option;
        }

        protected DropDownOptionDefinition AddDropdown<T>(string name, Func<T> getter, Action<T> setter,
            IEnumerable<T> values, Func<T, string> nameGetter)
        {
            return AddDropdown(name, getter, setter,
                values.Select(value => new DropDownOptionItem(nameGetter.Invoke(value), value)));
        }

        protected void RemoveButton(OptionDefinition optionDefinition)
        {
            SRDebug.Instance.RemoveOption(optionDefinition);
        }
    }
}