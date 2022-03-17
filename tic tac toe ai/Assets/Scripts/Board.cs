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
    public int moves = 0;
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
            if (isInWorldGrid(new Vector2Int((int)worldPosition.x, (int)worldPosition.y)))
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

                    Instantiate(prefabs[currentPlayer - 1], nearestPoint.transform.position, Quaternion.identity);
                    grid[nearestIndex] = currentPlayer;
                    if (checkWinner(currentPlayer))
                    {
                        Debug.Log("player : " + currentPlayer + " Win ");
                        end = true;
                    }
                    currentPlayer = currentPlayer == 1 ? 2 : 1;
                    moves++;
                    if (moves ==9)
                    {
                        end = true;
                        Debug.Log("draw");
                        return;
                    }
                    aiTurn();
                    
                }
            }
        }
        if (Input.GetKeyDown("space"))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    private void aiTurn()
    {
        Move bestMove = findBestMove(to2D(grid));
        grid[bestMove.col * 3 + bestMove.row] = 2;
        GameObject nearestPoint = points[bestMove.col * 3 + bestMove.row];
        Instantiate(prefabs[currentPlayer - 1], nearestPoint.transform.position, Quaternion.identity);
        if (checkWinner(currentPlayer))
        {
            Debug.Log("player : " + currentPlayer + " Win ");
            end = true;
        }
        currentPlayer = currentPlayer == 1 ? 2 : 1;
        moves++;
        if (moves == 9)
        {
            end = true;
            Debug.Log("draw");
            return;
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

    ////////////////////////////////////////////////AI////////////////////////////////////////////////
    class Move
    {
        public int row, col;
    };

    static int player = 2, opponent = 1;

    public int[,] to2D(int[] tab)
    {
        int[,] final = new int[3, 3];
        for (int i = 0; i < tab.Length; i++)
        {
            final[i % 3, (int)i / 3] = tab[i];
        }
        return final;
    }

    public int[] to1D(int[,] tab)
    {
        int[] final = new int[tab.Length];
        for (int i = 0; i < tab.GetLength(0); i++)
        {
            for (int j = 0; j < tab.GetLength(1); j++)
            {
                final[3*i+j] = tab[i, j];
            }
        }
        return final;
    }
    static bool isMovesLeft(int[,] board)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (board[i, j] == 0)
                    return true;
        return false;
    }

    static int evaluate(int[,] b)
    {
        // Checking for Rows for X or O victory.
        for (int row = 0; row < 3; row++)
        {
            if (b[row, 0] == b[row, 1] &&
                b[row, 1] == b[row, 2])
            {
                if (b[row, 0] == player)
                    return +10;
                else if (b[row, 0] == opponent)
                    return -10;
            }
        }

        // Checking for Columns for X or O victory.
        for (int col = 0; col < 3; col++)
        {
            if (b[0, col] == b[1, col] &&
                b[1, col] == b[2, col])
            {
                if (b[0, col] == player)
                    return +10;

                else if (b[0, col] == opponent)
                    return -10;
            }
        }

        // Checking for Diagonals for X or O victory.
        if (b[0, 0] == b[1, 1] && b[1, 1] == b[2, 2])
        {
            if (b[0, 0] == player)
                return +10;
            else if (b[0, 0] == opponent)
                return -10;
        }

        if (b[0, 2] == b[1, 1] && b[1, 1] == b[2, 0])
        {
            if (b[0, 2] == player)
                return +10;
            else if (b[0, 2] == opponent)
                return -10;
        }

        // Else if none of them have won then return 0
        return 0;
    }

    // This is the minimax function
    static int minimax(int[,] board, int depth, bool isMax)
    {
        int score = evaluate(board);

        if (score == 10)
            return score;

        if (score == -10)
            return score;

        if (isMovesLeft(board) == false)
            return 0;

        if (isMax)
        {
            int best = -1000;

            // Traverse all cells
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Check if cell is empty
                    if (board[i, j] == 0)
                    {
                        // Make the move
                        board[i, j] = player;

                        // the maximum value
                        best = Mathf.Max(best, minimax(board,
                                        depth + 1, !isMax));

                        // Undo the move
                        board[i, j] = 0;
                    }
                }
            }
            return best;
        }

        // If this minimizer's move
        else
        {
            int best = 1000;

            // Traverse all cells
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Check if cell is empty
                    if (board[i, j] == 0)
                    {
                        // Make the move
                        board[i, j] = opponent;

                        // the minimum value
                        best = Mathf.Min(best, minimax(board,
                                        depth + 1, !isMax));

                        // Undo the move
                        board[i, j] = 0;
                    }
                }
            }
            return best;
        }
    }

    // This will return the best possible
    static Move findBestMove(int[,] board)
    {
        int bestVal = -1000;
        Move bestMove = new Move();
        bestMove.row = -1;
        bestMove.col = -1;

        // optimal value.
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // Check if cell is empty
                if (board[i, j] == 0)
                {
                    // Make the move
                    board[i, j] = player;

                    int moveVal = minimax(board, 0, false);

                    // Undo the move
                    board[i, j] = 0;

                    // best
                    if (moveVal > bestVal)
                    {
                        bestMove.row = i;
                        bestMove.col = j;
                        bestVal = moveVal;
                    }
                }
            }
        }

        Debug.Log("The value of the best Move is " + bestVal);

        return bestMove;
    }

}

