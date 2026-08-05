using System;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Controls.Data
{
	public class DropDownControlItem : MonoBehaviour
	{
		[SerializeField] private Text label;
		[SerializeField] private Button button;

		private int _index;
		private Action<int> _clickCallback;

		private void Awake()
		{
			button.onClick.AddListener(HandleClick);
		}

		public void Setup(int index, string name, Action<int> clickCallback)
		{
			_index = index;
			label.text = name;
			_clickCallback = clickCallback;
		}

		private void HandleClick()
		{
			_clickCallback?.Invoke(_index);
		}
	}
}