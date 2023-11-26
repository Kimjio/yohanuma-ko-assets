using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBehaviourScript : MonoBehaviour
{
	public Action action = delegate() {
		Console.WriteLine("Hello World!");
	};
	
    // Start is called before the first frame update
    void Start()
    {
        action.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
