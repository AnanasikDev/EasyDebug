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

    public Theme(string name, Color fieldType, Color propertyType, Color value, Color nameColor, Color prefix, Color script)
    {
        Name = name;
        fieldTypeColor = fieldType;
        propertyTypeColor = propertyType;
        valueColor = value;
        this.nameColor = nameColor;
        prefixColor = prefix;
        scriptColor = script;
    }
}
