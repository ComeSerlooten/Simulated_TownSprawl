using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudAnimate : MonoBehaviour
{
    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
