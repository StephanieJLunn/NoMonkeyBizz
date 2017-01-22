using UnityEngine;
using System.Collections;

public class Actionnable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void action()
    {
        Debug.Log("ACTION REALIZED INSIDE THE CUBE!");
        transform.GetComponent<Renderer>().material.color = new Color(255.0f, 0.0f, 0.0f);
    }
}
