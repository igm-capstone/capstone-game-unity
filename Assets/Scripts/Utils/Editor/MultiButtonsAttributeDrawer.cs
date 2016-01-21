using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(MultiButtonsAttribute))]
public class MultiButtonsAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr = (MultiButtonsAttribute)attribute;
        return 16f * Mathf.CeilToInt((float)property.enumNames.Length / attr.Columns);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (MultiButtonsAttribute)attribute;
        var columns = attr.Columns;

        int buttonsIntValue = 0;
        int enumLength = property.enumNames.Length;
        bool[] buttonPressed = new bool[enumLength];
        float buttonWidth = (position.width - EditorGUIUtility.labelWidth) / columns;
        float buttonHeight = 16f;

        EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), label);

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < enumLength; i++)
        {
            var x = i % columns;
            var y = i / columns;

            // Check if the button is/was pressed 
            if ((property.intValue & (1 << i)) == 1 << i)
            {
                buttonPressed[i] = true;
            }

            Rect buttonPos = new Rect(position.x + EditorGUIUtility.labelWidth + buttonWidth * x, position.y + buttonHeight * y, buttonWidth, buttonHeight);

            buttonPressed[i] = GUI.Toggle(buttonPos, buttonPressed[i], property.enumNames[i], "Button");

            if (buttonPressed[i])
                buttonsIntValue += 1 << i;
        }

        if (EditorGUI.EndChangeCheck())
        {
            property.intValue = buttonsIntValue;
        }
    }
}
