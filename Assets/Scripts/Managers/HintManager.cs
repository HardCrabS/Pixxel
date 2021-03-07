using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour {
    [SerializeField] GameObject hintPrefab;
    GridA grid;
    private GameObject currHint;
	void Start () 
    {
        grid = GridA.Instance;
	}
	
    List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.hight; y++)
            {
                if (grid.allBoxes[x, y] != null)
                {
                    if (x < grid.width - 1)
                    {
                        if (grid.SwitchAndCheck(x, y, Vector2.right))
                        {
                            possibleMoves.Add(grid.allBoxes[x, y]);
                        }
                    }
                    if (y < grid.hight - 1)
                    {
                        if (grid.SwitchAndCheck(x, y, Vector2.up))
                        {
                            possibleMoves.Add(grid.allBoxes[x, y]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    GameObject PickRandomPiece()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = FindAllMatches();
        if(possibleMoves.Count > 0)
        {
            int randIndex = Random.Range(0, possibleMoves.Count);
            return possibleMoves[randIndex];
        }
        return null;
    }

    private IEnumerator CreateHintCorotine()
    {
        GameObject pieceToMark = PickRandomPiece();
        if(pieceToMark != null)
        {
            currHint = Instantiate(hintPrefab, pieceToMark.transform.position, transform.rotation);
        }
        yield return new WaitForSeconds(1f);
        Destroy(currHint);
    }

    public void MakeHint()
    {
        if (currHint == null)
        {
            StartCoroutine(CreateHintCorotine());
        }
    }
}
