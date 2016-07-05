using UnityEngine;
using System.Collections;

public class  Minimap : MonoBehaviour
{
    public Texture2D BackgroundImage;

    public void Draw(Rect rectangle)
    {
        Rect topleft = new Rect(rectangle.x - 5, rectangle.y - 10, 10, 20);
        Rect topright = new Rect(rectangle.x - 5 + rectangle.width, rectangle.y - 10, 10, 20);
        Rect bottomleft = new Rect(rectangle.x - 5, rectangle.y - 10 + rectangle.height, 10, 20);
        Rect bottomright = new Rect(rectangle.x - 5 + rectangle.width, rectangle.y - 10 + rectangle.height, 10, 20);

        GUI.Label(rectangle, BackgroundImage);

        GUI.Label(topleft, "1");
        GUI.Label(topright, "2");
        GUI.Label(bottomleft, "3");
        GUI.Label(bottomright, "4");
    }
}
