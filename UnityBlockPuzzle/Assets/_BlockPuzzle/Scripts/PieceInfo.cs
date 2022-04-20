using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInfo : MonoBehaviour
{

    public enum PieceType { none, I, J, L, T, O, S, Z }
    public PieceType pieceType;
    public int pieceOrientation = 0;

    public static RotationRule[] rules_I = new RotationRule[]{ // needs to be re-ordered
        new RotationRule(0,1, ( 0, 2), ( 0, 1), ( 1, 2), (-1, 2), ( 1, 1), (-1,1)),
        new RotationRule(1,0, (-2, 0), (-2, 1), ( 1, 0), ( 1, 1)),
        new RotationRule(1,2, (-1, 0), (-1, 1), ( 2, 0), ( 2, 1)),
        new RotationRule(2,1, ( 0, 2), ( 0, 1), ( 1, 2), (-1, 2), ( 1, 1), (-1,1)),
        new RotationRule(2,3, ( 0, 1), ( 1, 1), (-1, 1), (-2, 1)),
        new RotationRule(3,2, ( 2, 0), (-1, 0), ( 2, 1), (-1, 1)),
        new RotationRule(3,0, ( 1, 0), (-2, 0), ( 1, 1), (-2, 1)),
        new RotationRule(0,3, ( 0, 1), ( 1, 1), (-1, 1), ( 2, 1))
    };

    public static RotationRule[] rules_rest = new RotationRule[]
    {
        new RotationRule(0,1, (-1, 0), (-1, 1), ( 0,-2), (-1,-2)),
        new RotationRule(1,0, ( 1, 0), ( 1,-1), ( 0, 2), ( 1, 2)),
        new RotationRule(1,2, ( 1, 0), ( 1,-1), ( 0, 2), ( 1, 2)),
        new RotationRule(2,1, (-1, 0), (-1, 1), ( 0,-2), (-1,-2)),
        new RotationRule(2,3, ( 1, 0), ( 1, 1), ( 0,-2), ( 1,-2)),
        new RotationRule(3,2, (-1, 0), (-1,-1), ( 0, 2), (-1, 2)),
        new RotationRule(3,0, (-1, 0), (-1,-1), ( 0, 2), (-1, 2)),
        new RotationRule(0,3, ( 1, 0), ( 1, 1), ( 0,-2), ( 1,-2))
    };


    public class RotationRule
    {
        public int rotationStart;
        public int rotationEnd;

        public (int,int)[] test;

        public RotationRule(int start, int end, params (int,int) [] p)
        {
            rotationStart = start;
            rotationEnd = end;
            test = p;
        }
    }

}
