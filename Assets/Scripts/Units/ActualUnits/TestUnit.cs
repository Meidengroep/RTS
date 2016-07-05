using UnityEngine;
using System.Collections;

public class TestUnit : Unit 
{
    public override void AssignPlayer(Player player)
    {
        base.AssignPlayer(player);

        for (int c = 0; c < transform.childCount; c++)
        {
            Transform t = transform.GetChild(c);
            if (t.name != "AI")
            {
                t.gameObject.layer = player.gameObject.layer;
                t.GetComponent<Renderer>().materials[0].color = player.Color;
            }
        }

        gameObject.layer = player.gameObject.layer;
    }
}
