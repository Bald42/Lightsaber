using System;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Controls.Data
{
	public class DropDownControl : DataBoundControl
	{
		[SerializeField] private Button button;
		[SerializeField] private Text titleLabel;
		[SerializeField] private Text valueLabel;
		[SerializeField] private GameObject listObject;
		[SerializeField] private RectTransform listTransform;
		[SerializeField] private DropDownControlItem itemPrefab;
		[SerializeField] private RectTransform itemParent;

		private DropDownOptionDefinition _optionDefinition;

		protected override void OnBind(string propertyName, Type t)
		{
			base.OnBind(propertyName, t);

			_optionDefinition = (DropDownOptionDefinition)Option;

			titleLabel.text = propertyName;
			valueLabel.text = _optionDefinition.SelectedItem.Name;

			for (var index = 0; index < _optionDefinition.Items.Count; index++)
			{
				DropDownOptionItem dropDownOptionItem = _optionDefinition.Items[index];
				DropDownControlItem dropDownControlItem = Instantiate(itemPrefab, itemParent);
				dropDownControlItem.Setup(index, dropDownOptionItem.Name, HandleItemClick);
			}

			LayoutRebuilder.ForceRebuildLayoutImmediate(itemParent);
			button.onClick.AddListener(HandleMainClick);
		}

		protected override void OnValueUpdated(object newValue)
		{
			valueLabel.text = _optionDefinition.SelectedItem.Name;
		}

		public override bool CanBind(Type type, bool isReadOnly)
		{
			return true;
		}

		private void ToggleItemListActivation()
		{
			listObject.SetActive(!listObject.activeSelf);
			LayoutRebuilder.ForceRebuildLayoutImmediate(itemParent);
			listTransform.sizeDelta = new Vector2(listTransform.sizeDelta.x, Mathf.Min(itemParent.sizeDelta.y + 10, 200));

			Vector3[] corners = new Vector3[4];
			listTransform.GetWorldCorners(corners);

			if (corners[0].y < 0)
			{
				PlaceListAbove();
			}
			else
			{
				PlaceListUnder();
			}
		}

		private void HandleMainClick()
		{
			ToggleItemListActivation();
		}

		private void HandleItemClick(int index)
		{
			_optionDefinition.Select(index);
			ToggleItemListActivation();
		}

		private void PlaceList(float anchorMinY, float anchorMaxY, float pivotY)
		{
			listTransform.anchorMin = new Vector2(listTransform.anchorMin.x, anchorMinY);
			listTransform.anchorMax = new Vector2(listTransform.anchorMax.x, anchorMaxY);
			listTransform.pivot = new Vector2(listTransform.pivot.x, pivotY);
			listTransform.anchoredPosition = new Vector2(listTransform.anchoredPosition.x, 0);
		}

		private void PlaceListUnder()
		{
			PlaceList(0, 0, 1);
		}

		private void PlaceListAbove()
		{
			PlaceList(1, 1, 0);
		}

		private bool IsRectTransformInsideScreen(RectTransform rectTransform)
		{
			Vector3[] corners = new Vector3[4];
			rectTransform.GetWorldCorners(corners);
			foreach (Vector3 corner in corners)
			{
				Vector3 screenPoint = Camera.main.WorldToScreenPoint(corner);
				if (screenPoint.x < 0 || screenPoint.x > Screen.width || screenPoint.y < 0 || screenPoint.y > Screen.height)
				{
					return false;
				}
			}
			return true;
		}
	}
}