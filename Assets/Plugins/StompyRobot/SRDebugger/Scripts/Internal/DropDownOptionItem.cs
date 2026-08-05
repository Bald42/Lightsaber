namespace SRDebugger
{
	public class DropDownOptionItem
	{
		public readonly string Name;
		public readonly object Value;

		public DropDownOptionItem(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}
}