using UnityEngine;

[System.Serializable]
public class Theme
{
    public string Name;
    public Color fieldTypeColor;
    public Color propertyTypeColor;
    public Color valueColor;
    public Color nameColor;
    public Color prefixColor;
    public Color scriptColor;
    public Color dictKeyColor;

    public Theme(string name, Color fieldTypeColor, Color propertyTypeColor, Color valueColor, Color nameColor, Color prefixColor, Color scriptColor, Color dictKeyColor)
    {
        Name = name;
        this.fieldTypeColor = fieldTypeColor;
        this.propertyTypeColor = propertyTypeColor;
        this.valueColor = valueColor;
        this.nameColor = nameColor;
        this.prefixColor = prefixColor;
        this.scriptColor = scriptColor;
        this.dictKeyColor = dictKeyColor;
    }
}
