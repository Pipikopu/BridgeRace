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
    //public Rigidbody rgbody;

    //private void Start()
    //{
    //    onGround();
    //}

    //public void onFall()
    //{
    //    rgbody.useGravity = true;
    //    rgbody.isKinematic = false;
    //}

    //public void onGround()
    //{
    //    rgbody.useGravity = false;
    //    rgbody.isKinematic = true;
    //}
}
