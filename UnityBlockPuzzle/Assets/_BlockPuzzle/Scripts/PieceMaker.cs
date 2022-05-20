using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Piece
{
    public partial class PieceMaker : MonoBehaviour
    {
        [Tooltip("Put a thing in here to create!"), ContextMenuItem("Spawn", "makeAnotherOne"), ContextMenuItem("DESTROY THE LAST THING", "destroyTheLastOne")]
        public float delay;
        public Board board;
        // public BlockQueue blockQueue;
        // public Light[] pieceLight = new Light[4];
        float keyTimer = 0.0f;
        const float keyDelay = 1f / 8;
        public Text debugText;
        public GameObject previousGhostPiece;
        public long then;
        public GameObject pauseMenu;
        public Image singlePlayerControlsImage;
        public Image coopControlsImage;

        //most of the time this is going to be equal to 0
        //this variable is used for keeping track of how many times in a row a specific line clear happened
        public int rowsClearedLastTurn = 0;
        private int timesClearedinaRow = 1; //times a specific amount of rows were cleared in a row

        //public long fallCounter = 0;
        //double iterationDelay = 1000;

        bool delayTheImprintForSoftDrop = false;

        // private bool dirtyGhost = true;

        public class LightUnattacher : MonoBehaviour
        {
            public Light _light;
            private void OnDestroy()
            {
                _light.transform.SetParent(null);
            }
        }

        public int numberOfPlayers = 1;
        public Player player1;
        public Player player2;

        public int linesNeededToLevelUp = 10;

        public void SetLevel()
        {
            if (Score.LinesCleared >= linesNeededToLevelUp)
            {
                Score.Level += 1;
                int linesInTheNextLevel;
                switch (Score.Level)
                {
                    case 1: linesInTheNextLevel = 20; break;
                    case 2: linesInTheNextLevel = 30; break;
                    case 3: linesInTheNextLevel = 40; break;
                    case 4: linesInTheNextLevel = 50; break;
                    case 5: linesInTheNextLevel = 60; break;
                    case 6: linesInTheNextLevel = 70; break;
                    case 7: linesInTheNextLevel = 80; break;
                    case 8: linesInTheNextLevel = 90; break;
                    case 9: linesInTheNextLevel = 100; break;
                    case 10: linesInTheNextLevel = 100; break;
                    case 11: linesInTheNextLevel = 100; break;
                    case 12: linesInTheNextLevel = 100; break;
                    case 13: linesInTheNextLevel = 100; break;
                    case 14: linesInTheNextLevel = 100; break;
                    case 15: linesInTheNextLevel = 100; break;
                    case 16: linesInTheNextLevel = 110; break;
                    case 17: linesInTheNextLevel = 120; break;
                    case 18: linesInTheNextLevel = 130; break;
                    case 19: linesInTheNextLevel = 140; break;
                    case 20: linesInTheNextLevel = 150; break;
                    case 21: linesInTheNextLevel = 160; break;
                    case 22: linesInTheNextLevel = 170; break;
                    case 23: linesInTheNextLevel = 180; break;
                    case 24: linesInTheNextLevel = 190; break;
                    case 25: linesInTheNextLevel = 200; break;
                    case 26: linesInTheNextLevel = 200; break;
                    case 27: linesInTheNextLevel = 200; break;
                    case 28: linesInTheNextLevel = 200; break;
                    case 29: linesInTheNextLevel = 200; break;
                    default: linesInTheNextLevel = 200; break;
                }
                linesNeededToLevelUp += linesInTheNextLevel;
            }
        }

        public void ClearLines()
        {
            int rowsCleared = 0;
            int countColumns = 0, clearRow;
            for (int row = 0; row < board.height; ++row)
            {
                for (int col = 0; col < board.width; ++col)
                {
                    if (board.objectMatrix[row][col]) // change when we add shadow pieces and whatever
                    {
                        ++countColumns;
                    }
                }
                //
                //
                if (countColumns == board.width)
                {
                    clearRow = row;
                    rowsCleared++;
                    for (int col = 0; col < board.width; ++col)
                    {
                        Destroy(board.objectMatrix[row][col]);
                    }

                    for (int shiftRow = clearRow; shiftRow > 0; --shiftRow)
                    {
                        for (int shiftCol = 0; shiftCol < board.width; ++shiftCol)
                        {
                            board.objectMatrix[shiftRow][shiftCol] = board.objectMatrix[shiftRow - 1][shiftCol];
                            if (board.objectMatrix[shiftRow][shiftCol])
                            {
                                board.objectMatrix[shiftRow][shiftCol].transform.position += Vector3.down;
                            }
                        }
                    }
                    for (int r = 0; r < board.height; ++r)
                    {
                        for (int c = 0; c < board.width; ++c)
                        {
                            GameObject mino = board.objectMatrix[r][c];
                            if (mino == null) { continue; }
                            Vector3 expectedPosition = new Vector3(c + 0.5f, -(r - 3.5f), mino.gameObject.transform.position.z);
                            if (Vector3.Distance(expectedPosition, mino.transform.position) > 0.5f)
                            {
                                mino.gameObject.GetComponentInChildren<Renderer>().material.color = Color.black;
                            }
                        }
                    }
                }

                countColumns = 0;
            }

            if (rowsClearedLastTurn == rowsCleared && rowsCleared != 0)
            {
                timesClearedinaRow++;
                if (timesClearedinaRow >= 2) { comboScore(rowsCleared, Score.Level, timesClearedinaRow); }
            }
            else
            {
                timesClearedinaRow = 1;
                lineScore(rowsCleared, Score.Level);
            }
            //Debug.Log($"rows cleared: {rowsCleared}, times cleared in a row: {timesClearedinaRow}");

            rowsClearedLastTurn = rowsCleared;
            Score.LinesCleared += rowsCleared;
            SetLevel();
        }


        private void lineScore(int rowsCleared, int level)
        {
            switch (rowsCleared)
            {
                case 0:
                    break;

                case 1:
                    Score.Instance.value += (40 * (level + 1));
                    Noisy.PlaySound("Lines1");
                    break;

                case 2:
                    Score.Instance.value += (100 * (level + 1));
                    Noisy.PlaySound("Lines2");
                    break;

                case 3:
                    Score.Instance.value += (300 * (level + 1));
                    Noisy.PlaySound("Lines3");
                    break;

                default:
                    Score.Instance.value += (1200 * (level + 1));
                    Noisy.PlaySound("Lines4");
                    break;
            }

        }

        private void comboScore(int rowsCleared, int level, int times)
        {
            switch (rowsCleared)
            {
                case 0:
                    break;

                case 1:
                    Score.Instance.value += (int)((40 * (level + 1)) * (times - 0.5));
                    break;

                case 2:
                    Score.Instance.value += (int)((100 * (level + 1)) * (times - 0.5));
                    break;

                case 3:
                    Score.Instance.value += (int)((300 * (level + 1)) * (times));
                    break;

                default:
                    Score.Instance.value += (int)((1200 * (level + 1)) * (times));
                    break;
            }
        }

        public void resetRotation(GameObject p)
        {
            p.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        Vector2Int[] minoCoords(GameObject p) => minoCoords(p.transform);
        public Vector2Int[] minoCoords(Transform pieceTransform)
        {
            Vector2Int[] coords = new Vector2Int[4];
            int index = 0;
            for (int i = 0; i < pieceTransform.childCount; ++i)
            {
                Transform mino = pieceTransform.GetChild(i);
                if (!mino.GetComponent<Light>())
                {
                    int boardXPos = (int)Mathf.Round(mino.position.x - 0.5f), boardYPos = (int)Mathf.Round((mino.position.y - 3.5f) * -1);
                    coords[index++] = new Vector2Int(boardXPos, boardYPos);
                }
            }
            return coords;
        }
        bool isColliding(GameObject p) => CollisionDetection.isColliding(minoCoords(p), board.objectMatrix);

        public static long UTCMS()
        {
            return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        }

        public enum PieceCollision { none, blockedV, blockedH, oobLeft, oobRight, oobTop, oobBottom };

        public KeyCode currentKey = KeyCode.None;

        void Start()
        {
            //numberOfPlayers = 1;
            then = UTCMS();
            debugText = GameObject.Find("Debug Text")?.GetComponent<Text>();

            singlePlayerControlsImage.enabled = false;
            coopControlsImage.enabled = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;

            //Random.InitState(System.Environment.TickCount);
            player1.blockQueue.initNums(7);
            player1.blockQueue.initialPosition = new Vector3(18, 0.5f, 4);
            player1.startingPos = transform.position;

            player1.blockQueue.makeQueue();

            if (numberOfPlayers == 2)
            {
                player1.startingPos = transform.position + Vector3.left * 2;
                player1.holdPosition = new Vector3(-10, 0.5f, 4);
                player2.startingPos = transform.position + Vector3.right * 2;
                player2.blockQueue.initialPosition = new Vector3(23, 0.5f, 4);
                player2.blockQueue.initNums(7);
                player2.blockQueue.makeQueue();
                player2.makeAnotherOne();
            }
            player1.makeAnotherOne();
        }

        bool SpecialCollisionLogic(PieceInfo currentPieceInfo, int rotationRuleIndex, PieceInfo.RotationRule[] rulesSet = null)
        {
            if (rulesSet == null) rulesSet = PieceInfo.rules_I;
            for (int i = 0; i < rulesSet[rotationRuleIndex].test.Length; i++)
            {
                (int, int) v = rulesSet[rotationRuleIndex].test[i];
                Vector3Int piece_test = new Vector3Int(v.Item1, v.Item2, 0);
                player1.currentPiece.transform.position += piece_test;
                if (!(player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)))
                {
                    return true;
                }
                else player1.currentPiece.transform.position -= piece_test;
            }
            return false;
        }

        public Dictionary<KeyCode, Action> _controls;

        Dictionary<KeyCode, Action> controls =>
            _controls != null
                ? _controls
                : _controls = new Dictionary<KeyCode, Action>()
                {
                    [KeyCode.P] = () =>
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < player1.currentPiece.transform.childCount; i++)
                        {
                            //Debug.Log("Child " + i + " " + transform.GetChild(i).position);
                            sb.Append("Child " + i + " board xPos: " + (player1.currentPiece.transform.GetChild(i).position.x - 0.5) +
                                " board yPos:" + (player1.currentPiece.transform.GetChild(i).position.y - 3.5) * -1).Append("\n");
                        }
                        Debug.Log(sb);
                        if (debugText) debugText.text = sb.ToString();
                    },
                    [KeyCode.Z] = () =>
                    {

                        player1.currentPiece.transform.Rotate(0, 0, 90);
                        PieceInfo currentPieceInfo = player1.currentPiece.GetComponent<PieceInfo>();
                        bool canRotate = true;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece))
                        {
                            canRotate = false;
                            if (currentPieceInfo.pieceType == PieceInfo.PieceType.I) // IPiece 
                            {
                                switch (currentPieceInfo.pieceOrientation)
                                {
                                    case 0:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 7);
                                        break;
                                    case 1:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 1);
                                        break;
                                    case 2:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 3);
                                        break;
                                    case 3:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 5);
                                        break;
                                }

                            }
                            else
                            {
                                switch (currentPieceInfo.pieceOrientation)
                                {
                                    case 0:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 7, PieceInfo.rules_rest);
                                        break;
                                    case 1:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 1, PieceInfo.rules_rest);
                                        break;
                                    case 2:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 3, PieceInfo.rules_rest);
                                        break;
                                    case 3:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 5, PieceInfo.rules_rest);
                                        break;
                                }
                            }
                        }
                        if (canRotate)
                        {
                            player1.currentGhostPiece.transform.Rotate(0, 0, 90);
                            currentPieceInfo.pieceOrientation--;
                            if (currentPieceInfo.pieceOrientation < 0) currentPieceInfo.pieceOrientation += 4;
                            player1.dirtyGhost = true;
                        }
                        else
                        {
                            player1.currentPiece.transform.Rotate(0, 0, -90);
                        }

                    },
                    [KeyCode.X] = () =>
                    {
                        player1.currentPiece.transform.Rotate(0, 0, -90);
                        PieceInfo currentPieceInfo = player1.currentPiece.GetComponent<PieceInfo>();
                        bool canRotate = true;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece))
                        {
                            canRotate = false;
                            if (currentPieceInfo.pieceType == PieceInfo.PieceType.I) // IPiece 
                            {
                                switch (currentPieceInfo.pieceOrientation)
                                {
                                    case 0:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 0);
                                        break;
                                    case 1:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 2);
                                        break;
                                    case 2:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 4);
                                        break;
                                    case 3:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 6);
                                        break;
                                }

                            }
                            else
                            {
                                switch (currentPieceInfo.pieceOrientation)
                                {
                                    case 0:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 0, PieceInfo.rules_rest);
                                        break;
                                    case 1:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 2, PieceInfo.rules_rest);
                                        break;
                                    case 2:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 4, PieceInfo.rules_rest);
                                        break;
                                    case 3:
                                        canRotate = SpecialCollisionLogic(currentPieceInfo, 6, PieceInfo.rules_rest);
                                        break;
                                }
                            }
                        }
                        if (canRotate)
                        {
                            player1.currentGhostPiece.transform.Rotate(0, 0, -90);
                            currentPieceInfo.pieceOrientation++;
                            if (currentPieceInfo.pieceOrientation > 3) currentPieceInfo.pieceOrientation -= 4;
                            player1.dirtyGhost = true;
                        }
                        else
                        {
                            player1.currentPiece.transform.Rotate(0, 0, 90);
                        }

                    },
                    [KeyCode.A] = () =>
                    {
                        player1.currentPiece.transform.Rotate(0, 0, -180);
                        player1.currentGhostPiece.transform.Rotate(0, 0, -180);
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.Space] = () => {
                        if (!Input.GetKeyDown(KeyCode.Space)) return;
                        player1.HardDrop(this);
                    },
                    [KeyCode.C] = () =>
                    {
                        player1.swapHold();
                    },
                    [KeyCode.DownArrow] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.down;
                        Score.Instance.value += 1 * (Score.Level / 2 + 1);

                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece))
                        {
                            player1.currentPiece.transform.position += Vector3.up;
                            Score.Instance.value -= 1 * (Score.Level / 2 + 1);
                            if (!delayTheImprintForSoftDrop)
                            {
                                player1.fallCounter = 0;
                                delayTheImprintForSoftDrop = true;
                            }
                        }
                    },
                    [KeyCode.LeftArrow] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.left;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)) player1.currentPiece.transform.position += Vector3.right;
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.RightArrow] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.right;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)) player1.currentPiece.transform.position += Vector3.left;
                        player1.dirtyGhost = true;
                    },
                };

        public Dictionary<KeyCode, Action> _coopcontrols;
        Dictionary<KeyCode, Action> coopcontrols =>
            _coopcontrols != null
                ? _coopcontrols
                : _coopcontrols = new Dictionary<KeyCode, Action>()
                {
                    [KeyCode.Q] = () =>
                    {
                        player1.currentPiece.transform.Rotate(0, 0, 90);
                        player1.currentGhostPiece.transform.Rotate(0, 0, 90);
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.E] = () =>
                    {
                        player1.currentPiece.transform.Rotate(0, 0, -90);
                        player1.currentGhostPiece.transform.Rotate(0, 0, -90);
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.F] = () =>
                    {
                        player1.currentPiece.transform.Rotate(0, 0, -180);
                        player1.currentGhostPiece.transform.Rotate(0, 0, -180);
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.Space] = () => {
                        if (!Input.GetKeyDown(KeyCode.Space)) return;
                        player1.HardDrop(this);
                        player2.placeGhostPiece();
                    },
                    [KeyCode.C] = () =>
                    {
                        player1.swapHold();
                    },
                    [KeyCode.S] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.down;
                        Score.Instance.value += 1 * (Score.Level / 2 + 1);

                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece))
                        {
                            player1.currentPiece.transform.position += Vector3.up;
                            Score.Instance.value -= 1 * (Score.Level / 2 + 1);
                            if (!delayTheImprintForSoftDrop)
                            {
                                player1.fallCounter = 0;
                                delayTheImprintForSoftDrop = true;
                            }
                        }
                    },
                    [KeyCode.A] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.left;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)) player1.currentPiece.transform.position += Vector3.right;
                        player1.dirtyGhost = true;
                    },
                    [KeyCode.D] = () =>
                    {
                        player1.currentPiece.transform.position += Vector3.right;
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)) player1.currentPiece.transform.position += Vector3.left;
                        player1.dirtyGhost = true;
                    },


                    // second player
                    [KeyCode.K] = () =>
                    {
                        player2.currentPiece.transform.Rotate(0, 0, 90);
                        player2.currentGhostPiece.transform.Rotate(0, 0, 90);
                        player2.dirtyGhost = true;
                    },
                    [KeyCode.L] = () =>
                    {
                        player2.currentPiece.transform.Rotate(0, 0, -90);
                        player2.currentGhostPiece.transform.Rotate(0, 0, -90);
                        player2.dirtyGhost = true;
                    },
                    [KeyCode.O] = () =>
                    {
                        player2.currentPiece.transform.Rotate(0, 0, -180);
                        player2.currentGhostPiece.transform.Rotate(0, 0, -180);
                        player2.dirtyGhost = true;
                    },
                    [KeyCode.Period] = () => {
                        if (!Input.GetKeyDown(KeyCode.Period)) return;
                        player2.HardDrop(this);
                        player1.placeGhostPiece();
                    },
                    [KeyCode.Comma] = () =>
                    {
                        player2.swapHold();
                    },
                    [KeyCode.DownArrow] = () =>
                    {
                        player2.currentPiece.transform.position += Vector3.down;
                        Score.Instance.value += 1 * (Score.Level / 2 + 1);

                        if (player2.isPieceOOB(player2.currentPiece) || isColliding(player2.currentPiece))
                        {
                            player2.currentPiece.transform.position += Vector3.up;
                            Score.Instance.value -= 1 * (Score.Level / 2 + 1);
                            if (!delayTheImprintForSoftDrop)
                            {
                                player2.fallCounter = 0;
                                delayTheImprintForSoftDrop = true;
                            }
                        }
                    },
                    [KeyCode.LeftArrow] = () =>
                    {
                        player2.currentPiece.transform.position += Vector3.left;
                        if (player2.isPieceOOB(player2.currentPiece) || isColliding(player2.currentPiece)) player2.currentPiece.transform.position += Vector3.right;
                        player2.dirtyGhost = true;
                    },
                    [KeyCode.RightArrow] = () =>
                    {
                        player2.currentPiece.transform.position += Vector3.right;
                        if (player2.isPieceOOB(player2.currentPiece) || isColliding(player2.currentPiece)) player2.currentPiece.transform.position += Vector3.left;
                        player2.dirtyGhost = true;
                    },
                };


        [TextArea(4, 4)]
        public string debugPosition;
        void Update()
        {
            if (!board.isGameLoaded)
            {
                then = UTCMS();
                return;
            }
            if (player1.dirtyGhost)
            {
                player1.placeGhostPiece();
                player1.dirtyGhost = false;
            }
            if (numberOfPlayers == 2 && player2.dirtyGhost)
            {
                player2.placeGhostPiece();
                player2.dirtyGhost = false;
            }
            if (keyTimer > 0)
            {
                keyTimer -= Time.deltaTime;
                if (keyTimer > 0)
                {
                    return;
                }
            }

            if (numberOfPlayers == 1)
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    //singlePlayerControlsImage.enabled = !singlePlayerControlsImage.enabled;
                    if (Time.timeScale == 1)
                    {
                        pauseMenu.SetActive(true);
                        singlePlayerControlsImage.enabled = true;
                        Time.timeScale = 0;
                    }
                    else
                    {
                        pauseMenu.SetActive(false);
                        singlePlayerControlsImage.enabled = false;
                        Time.timeScale = 1;
                        then = UTCMS();
                    }
                }
            } else
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    //coopControlsImage.enabled = !coopControlsImage.enabled;
                    if (Time.timeScale == 1)
                    {
                        pauseMenu.SetActive(true);
                        coopControlsImage.enabled = true;
                        Time.timeScale = 0;
                    }
                    else
                    {
                        pauseMenu.SetActive(false);
                        coopControlsImage.enabled = false;
                        Time.timeScale = 1;
                        then = UTCMS();
                    }
                }
            }


            if (Time.timeScale == 1)
            {
                long now = UTCMS();
                //time passed
                long passed = now - then;
                then = now;
                player1.fallCounter += passed;
                player1.pieceFallOnTime(this);
                if (numberOfPlayers == 2)
                {
                    player2.fallCounter += passed;
                    //player2.pieceFallOnTime(this);
                    player2.pieceFallOnTime(this);
                }
            }
            
            currentKey = KeyCode.None;

            if (numberOfPlayers == 2)
            {
                foreach (KeyValuePair<KeyCode, Action> kvp in coopcontrols)
                {
                    if (Input.GetKey(kvp.Key))
                    {
                        kvp.Value.Invoke();
                        keyTimer = keyDelay;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<KeyCode, Action> kvp in controls)
                {
                    if (Input.GetKey(kvp.Key))
                    {
                        kvp.Value.Invoke();
                        keyTimer = keyDelay;
                    }
                }
            }
        }
    }
}