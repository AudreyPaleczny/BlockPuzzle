using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobbly : MonoBehaviour
{
    Vector3 axis;
	private void Start() {
        axis = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
        axis.Normalize();
	}
	void Update()
    {
        transform.Rotate(axis, 10 * Time.deltaTime);
    }
}
