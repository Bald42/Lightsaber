namespace SRDebugger.UI.Other
{
	using SRDebugger.UI.Controls;
	using SRDebugger.UI.Controls.Data;
	using SRF;
    using UnityEngine;
    using UnityEngine.UI;

	public class CategoryGroup : SRMonoBehaviourEx
    {
        [RequiredField] public RectTransform Container;
        [RequiredField] public Text Header;
        [RequiredField] public GameObject Background;
        [RequiredField] public Toggle SelectionToggle;
		[Space]
        [SerializeField] private Text buttonText;
		[SerializeField] private string arrowUp;
		[SerializeField] private string arrowDown;

        public GameObject[] EnabledDuringSelectionMode = new GameObject[0];

        private bool _selectionModeEnabled = true;
		private bool _isView = false;
		private OptionsControlBase[] _actionControls = new OptionsControlBase[0];

		public void AddActionControls(OptionsControlBase[] actionControls)
		{
			_actionControls = actionControls;
			ChangeView(_isView);
		}

		public bool IsSelected
        {
            get
            {
                return SelectionToggle.isOn;
            }
            set
            {
                SelectionToggle.isOn = value;

                if (SelectionToggle.graphic != null)
                {
                    SelectionToggle.graphic.CrossFadeAlpha(value ? _selectionModeEnabled ? 1.0f : 0.2f : 0f, 0, true);
                }
            }
        }

        public bool SelectionModeEnabled
        {
            get { return _selectionModeEnabled; }

            set
            {
                if (value == _selectionModeEnabled)
                {
                    return;
                }

                _selectionModeEnabled = value;

                for (var i = 0; i < EnabledDuringSelectionMode.Length; i++)
                {
                    EnabledDuringSelectionMode[i].SetActive(_selectionModeEnabled);
                }
            }
		}

		public void OnClickHide()
		{
			_isView = !_isView;
			ChangeView(_isView);
		}

		private void ChangeView(bool isActive)
		{
			buttonText.text = isActive ? arrowUp : arrowDown;
			foreach(OptionsControlBase actionControl in _actionControls)
			{
				actionControl.gameObject.SetActive(isActive);
			}
		}
	}
}
