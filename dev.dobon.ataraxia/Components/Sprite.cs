namespace dev.dobon.ataraxia.Components;

public sealed class Sprite(string texturePath): IComponent
{
    public string TexturePath = texturePath;
}