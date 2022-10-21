using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Piece
{
    public enum playerActions
    {
        SingleRotateLeft,
        SingleRotateRight,
        SingleHardDrop,
        SingleHold,
        SingleSoftDrop,
        SingleMoveLeft,
        SingleMoveRight,

        Coop1RotateLeft,
        Coop1RotateRight,
        Coop1HardDrop,
        Coop1Hold,
        Coop1SoftDrop,
        Coop1MoveLeft,
        Coop1MoveRight,

        Coop2RotateLeft,
        Coop2RotateRight,
        Coop2HardDrop,
        Coop2Hold,
        Coop2SoftDrop,
        Coop2MoveLeft,
        Coop2MoveRight,

        count
    }
    public partial class PieceMaker
    {
        public void printPositionStuff()
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
        }
        public void singleRotateLeft()
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

        }
        public void singleRotateRight()
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

        }
        public void singleRotate180()
        {
            player1.currentPiece.transform.Rotate(0, 0, -180);
            player1.currentGhostPiece.transform.Rotate(0, 0, -180);
            if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece))
            {
                player1.currentPiece.transform.Rotate(0, 0, 180);
                player1.currentGhostPiece.transform.Rotate(0, 0, 180);
            }
            player1.dirtyGhost = true;
        }
        public void singleHardDrop()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            player1.HardDrop(this);
        }
        public void singleHold()
        {
            player1.swapHold();
        }
        public void singleSoftDrop()
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
        }
        public void singleMoveLeft()
        {
            player1.currentPiece.transform.position += Vector3.left;
            if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)) player1.currentPiece.transform.position += Vector3.right;
            player1.dirtyGhost = true;
        }
        public void singleMoveRight()
        {
            player1.currentPiece.transform.position += Vector3.right;
            if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece)) player1.currentPiece.transform.position += Vector3.left;
            player1.dirtyGhost = true;
        }
        public Dictionary<KeyCode, Action> _controls;
        Dictionary<KeyCode, Action> controls
        {
            get
            {
                if (_controls != null) return _controls;
                var controlMap = Keybinds.loadControls();
                return _controls = new Dictionary<KeyCode, Action>()
                {
                    [KeyCode.P] = printPositionStuff,
                    [controlMap[playerActions.SingleRotateLeft]] = singleRotateLeft,
                    [controlMap[playerActions.SingleRotateRight]] = singleRotateRight,
                    [KeyCode.A] = singleRotate180,
                    [controlMap[playerActions.SingleHardDrop]] = singleHardDrop,
                    [controlMap[playerActions.SingleHold]] = singleHold,
                    [controlMap[playerActions.SingleSoftDrop]] = singleSoftDrop,
                    [controlMap[playerActions.SingleMoveLeft]] = singleMoveLeft,
                    [controlMap[playerActions.SingleMoveRight]] = singleMoveRight,
                    [KeyCode.M] = PauseGameKey,
                };
            }
        }
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
                        if (player1.isPieceOOB(player1.currentPiece) || isColliding(player1.currentPiece))
                        {
                            player1.currentPiece.transform.Rotate(0, 0, 180);
                            player1.currentGhostPiece.transform.Rotate(0, 0, 180);
                        }
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
                        if (player2.isPieceOOB(player2.currentPiece) || isColliding(player2.currentPiece))
                        {
                            player2.currentPiece.transform.Rotate(0, 0, 180);
                            player2.currentGhostPiece.transform.Rotate(0, 0, 180);
                        }
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
                    [KeyCode.M] = PauseGameKey,
                };
    }

}