using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpText : MonoBehaviour
{

    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, 0);
    
    void Start()
    {
        Destroy(gameObject, lifeTime);
        transform.localPosition += offset;
    }

}
