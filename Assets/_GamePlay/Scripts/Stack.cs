using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : GameUnit
{
    public Renderer stackRenderer;

    public void changeMaterial(Material material)
    {
        stackRenderer.material = material;
    }
}
