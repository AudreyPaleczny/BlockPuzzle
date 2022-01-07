using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMaker : MonoBehaviour
{
    [Tooltip("Put a thing in here to create!"), ContextMenuItem("Spawn", "makeAnotherOne"), ContextMenuItem("DESTROY THE LAST THING", "destroyTheLastOne")]
    public List<GameObject> prefabsOfPieces = new List<GameObject>();
    public float delay;
    public List<GameObject> listOfObjects = new List<GameObject>();
    public Board board;
    public Light pieceLight;

    public class LightUnattacher : MonoBehaviour
    {
        public Light light;
        private void OnDestroy()
        {
            light.transform.SetParent(null);
        }
    }

    public void destroyTheLastOne()
    {
        int last = listOfObjects.Count - 1;
        Destroy(listOfObjects[last]);
        listOfObjects.RemoveAt(last);
    }

    public void makeAnotherOne()
    {
        int p = 3;
        //Random.Range(0, prefabsOfPieces.Count);

        GameObject newOne = Instantiate(prefabsOfPieces[p]);
        newOne.transform.position = transform.position;
        newOne.transform.SetParent(transform);
        listOfObjects.Add(newOne);
        pieceLight.transform.SetParent(newOne.transform);
        pieceLight.transform.localPosition = Vector3.zero;

        PieceMove pm = newOne.GetComponent<PieceMove>();
        pm.board = board;
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
