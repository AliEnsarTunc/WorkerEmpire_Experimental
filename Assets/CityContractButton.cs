﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityContractButton : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        GameManager.Instance.WorkerContractWindow.LoadWindowInfo(transform.parent.gameObject.name);
    }
}