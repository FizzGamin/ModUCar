using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SpriteService : UnitySingleton<SpriteService>
{
    //Might want to init every sprite and store them
    Dictionary<string, Texture2D> textureDict = new Dictionary<string, Texture2D>();
    void Start()
    {
        GameManager.instance.SetSpriteService(this);

        //Init sprites
        Texture2D[] textures = Resources.LoadAll("Sprites", typeof(Texture2D)).Cast<Texture2D>().ToArray();
        foreach (Texture2D texture in textures)
            textureDict[texture.name] = texture;

    }

    public Sprite GetSprite(string name)
    {
        if (name == null || !textureDict.ContainsKey(name)) return null;
        Texture2D texture = textureDict[name];
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(.5f, .5f));
    }
}
