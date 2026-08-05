using System.Collections.Generic;
using System.Linq;
using SRF.Helpers;
using UnityEngine;

namespace SRDebugger
{
	public class DropDownOptionDefinition : OptionDefinition
	{
		public List<DropDownOptionItem> Items { get; private set; }
		public int SelectedIndex { get; private set; }
		public DropDownOptionItem SelectedItem => Items[SelectedIndex];

		public DropDownOptionDefinition(string name, PropertyReference property, IEnumerable<DropDownOptionItem> items, string category = "Default", int sortPriority = 0) : base(name, category, sortPriority, property)
		{
			Items = new List<DropDownOptionItem>(items);
			UpdateValue();
		}

		public void AddItem(string name, object value)
		{
			Items.Add(new DropDownOptionItem(name, value));
		}

		public void RemoveItem(string name)
		{
			DropDownOptionItem item = Items.FirstOrDefault(x => x.Name == name);

			if(item == null)
				return;

			Items.Remove(item);
		}

		public void RemoveItem(object value)
		{
			DropDownOptionItem item = Items.FirstOrDefault(x => x.Value == value);

			if(item == null)
				return;

			Items.Remove(item);
		}

		public void Select(int index)
		{
			SelectedIndex = SRMath.Wrap(Items.Count, index);
			UpdateValue();
		}

		private void UpdateValue()
		{
			Property.SetValue(SelectedItem.Value);
		}
	}
}