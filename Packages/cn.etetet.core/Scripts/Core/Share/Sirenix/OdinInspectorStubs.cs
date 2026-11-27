using System;

namespace Sirenix.OdinInspector
{
    public struct ValueDropdownItem<T>
    {
        public string Text;
        public T Value;

        public ValueDropdownItem(string text, T value)
        {
            this.Text = text;
            this.Value = value;
        }
    }

    public class BoxGroupAttribute : Attribute
    {
        public bool CenterLabel { get; set; }

        public BoxGroupAttribute()
        {
        }

        public BoxGroupAttribute(string group)
        {
        }
    }

    public class LabelWidthAttribute : Attribute
    {
        public LabelWidthAttribute(int width)
        {
        }
    }

    public class LabelTextAttribute : Attribute
    {
        public LabelTextAttribute()
        {
        }

        public LabelTextAttribute(string text)
        {
        }
    }

    public class PropertyOrderAttribute : Attribute
    {
        public PropertyOrderAttribute(int order)
        {
        }
    }

    public class ShowIfAttribute : Attribute
    {
        public ShowIfAttribute()
        {
        }

        public ShowIfAttribute(string condition)
        {
        }
    }

    public class FoldoutGroupAttribute : Attribute
    {
        public bool Expanded { get; set; }

        public FoldoutGroupAttribute(string group)
        {
        }
    }

    public class MinValueAttribute : Attribute
    {
        public MinValueAttribute(double value)
        {
        }
    }

    public class ReadOnlyAttribute : Attribute
    {
    }

    public class HideReferenceObjectPickerAttribute : Attribute
    {
    }

    public class DisplayAsStringAttribute : Attribute
    {
    }

    public class ShowInInspectorAttribute : Attribute
    {
    }

    public class HideLabelAttribute : Attribute
    {
    }

    public class InlinePropertyAttribute : Attribute
    {
    }

    public class HideMonoScriptAttribute : Attribute
    {
    }

    public class HorizontalGroupAttribute : Attribute
    {
        public HorizontalGroupAttribute()
        {
        }

        public HorizontalGroupAttribute(string group)
        {
        }
    }

    public class ButtonAttribute : Attribute
    {
        public ButtonAttribute()
        {
        }

        public ButtonAttribute(string name)
        {
        }
    }

    public class InfoBoxAttribute : Attribute
    {
        public InfoBoxAttribute(string message)
        {
        }
    }

    public class ValueDropdownAttribute : Attribute
    {
        public ValueDropdownAttribute(string valuesGetter)
        {
        }
    }

    public class ListDrawerSettingsAttribute : Attribute
    {
        public bool ShowFoldout { get; set; }
        public bool HideAddButton { get; set; }
        public bool HideRemoveButton { get; set; }
        public bool DraggableItems { get; set; }
        public bool ShowIndexLabels { get; set; }
        public bool ShowItemCount { get; set; }
    }

    public class TitleAttribute : Attribute
    {
        public TitleAttribute(string title)
        {
        }
    }

    public class RequiredAttribute : Attribute
    {
    }

    public class TabGroupAttribute : Attribute
    {
        public TabGroupAttribute(string group)
        {
        }

        public TabGroupAttribute(string group, string tab)
        {
        }
    }

    public class EnumToggleButtonsAttribute : Attribute
    {
    }

    public class SuffixLabelAttribute : Attribute
    {
        public SuffixLabelAttribute(string label)
        {
        }
    }

    public class PropertyRangeAttribute : Attribute
    {
        public PropertyRangeAttribute(double min, double max)
        {
        }
    }

    public class OnValueChangedAttribute : Attribute
    {
        public OnValueChangedAttribute(string action)
        {
        }
    }

    public class GUIColorAttribute : Attribute
    {
        public GUIColorAttribute(float r, float g, float b, float a = 1f)
        {
        }

        public GUIColorAttribute(string getColor)
        {
        }
    }

    public class SearchableAttribute : Attribute
    {
    }

    public class TableListAttribute : Attribute
    {
    }

    public class InlineEditorAttribute : Attribute
    {
    }

    public class ShowIfGroupAttribute : Attribute
    {
        public ShowIfGroupAttribute(string condition)
        {
        }
    }

    public class TitleGroupAttribute : Attribute
    {
        public TitleGroupAttribute(string title)
        {
        }
    }

    public class PropertySpaceAttribute : Attribute
    {
    }

    public class MultiLinePropertyAttribute : Attribute
    {
        public MultiLinePropertyAttribute(int lines = 3)
        {
        }
    }

    public class DisableIfAttribute : Attribute
    {
        public DisableIfAttribute(string condition)
        {
        }
    }

    public class EnableIfAttribute : Attribute
    {
        public EnableIfAttribute(string condition)
        {
        }
    }

    public class HideIfAttribute : Attribute
    {
        public HideIfAttribute(string condition)
        {
        }
    }
}
