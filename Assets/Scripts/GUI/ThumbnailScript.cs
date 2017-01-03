using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class ThumbnailScript : MonoBehaviour
{
    [SerializeField]
    protected Image ImageComponent;
    
    public void SetImageComponent(string imgLocation, int height=-1, int width=-1)
    {
        if(string.IsNullOrEmpty(imgLocation))
        {
            throw new ArgumentException("imgLocation cant be null or empty");
        }

        Texture2D texture;

        if(imgLocation.StartsWith("file:///"))
        {            
            texture = new WWW(imgLocation).texture;
        }
            
        else
        {
            texture = (Texture2D)Resources.Load(imgLocation);            
        }

        if (height > -1 && height != texture.height || width > -1 && width != texture.width)
        {
            return;
        }

        ImageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.height, texture.width), new Vector2(0.5f, 0.5f));
    }


}