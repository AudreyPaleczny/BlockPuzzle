using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Tooltip("Put a thing in here to create!"), ContextMenuItem("Spawn","makeAnotherOne"), ContextMenuItem("DESTROY THE LAST THING", "destroyTheLastOne")]
    public GameObject prefab;
    public float delay;
    public List<GameObject> listOfObjects = new List<GameObject>();

    public void destroyTheLastOne()
    {
        int last = listOfObjects.Count - 1;
        Destroy(listOfObjects[last]);
        listOfObjects.RemoveAt(last);
    }

    public void makeAnotherOne()
    {
        GameObject newOne = Instantiate(prefab);
        newOne.transform.position = transform.position;
        newOne.transform.SetParent(transform);
        listOfObjects.Add(newOne);
    }

    // Start is called before the first frame update
    void Start()
    {
        makeAnotherOne();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
