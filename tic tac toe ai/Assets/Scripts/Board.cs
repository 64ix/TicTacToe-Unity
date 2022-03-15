using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    public int[] grid;
    public List<GameObject> prefabs = new List<GameObject>();
    public List<GameObject> points = new List<GameObject>();
    public int currentPlayer = 1;
    public bool end = false;
    // Start is called before the first frame update
    void Start()
    {
        grid = new int[9];
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && end == false)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
            if (isInWorldGrid(new Vector2Int((int)worldPosition.x,(int)worldPosition.y)))
            {
                GameObject nearestPoint = null;
                float distance = 10000f;
                int index = 0;
                int nearestIndex = 0;
                foreach (GameObject item in points)
                {
                    float testDistance = Vector3.Distance(item.transform.position, worldPosition);
                    if (distance >= testDistance)
                    {
                        distance = testDistance;
                        nearestPoint = item;
                        nearestIndex = index;
                    }
                    index++;
                }
                if (isValid(nearestIndex))
                {

                Instantiate(prefabs[currentPlayer-1], nearestPoint.transform.position, Quaternion.identity);
                grid[nearestIndex] = currentPlayer;
                if (checkWinner(currentPlayer))
                {
                    Debug.Log("player : " + currentPlayer + " Win ");
                    end = true;
                }
                currentPlayer = currentPlayer == 1 ? 2 : 1;
                }
            }
        }
    if(Input.GetKeyDown("space"))
	{
            SceneManager.LoadScene("SampleScene");
	}
    }

    private bool checkWinner(int player)
    {
        // check rows
        if (grid[0] == player && grid[1] == player && grid[2] == player) { return true; }
        if (grid[3] == player && grid[4] == player && grid[5] == player) { return true; }
        if (grid[6] == player && grid[7] == player && grid[8] == player) { return true; }

        // check columns
        if (grid[0] == player && grid[3] == player && grid[6] == player) { return true; }
        if (grid[1] == player && grid[4] == player && grid[7] == player) { return true; }
        if (grid[2] == player && grid[5] == player && grid[8] == player) { return true; }

        // check diags
        if (grid[0] == player && grid[4] == player && grid[8] == player) { return true; }
        if (grid[2] == player && grid[4] == player && grid[6] == player) { return true; }

        return false;
    }

    private bool isInWorldGrid(Vector2Int testPos)
    {
        return (testPos.x >= -5 && testPos.x <= 5 && testPos.y >= -5 && testPos.y <= 5);
    }
    private bool isValid(int pos)
    {
        return (grid[pos] == 0);
    }
}
