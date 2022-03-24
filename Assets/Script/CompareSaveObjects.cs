using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareSaveObjects : IComparer<ComparableComponent>
{
    public int Compare(ComparableComponent x, ComparableComponent y)
    {
        Vector3 Pos1 = x.transform.position;
        Vector3 Pos2 = y.transform.position;
        if (Pos1.x > Pos2.x)
            return 1;
        else if (Pos1.x == Pos2.x)
            if (Pos1.y > Pos2.y)
                return 1;
            else if (Pos1.y == Pos2.y)
                if (Pos1.z > Pos2.z)
                    return 1;
                else if (Pos1.z == Pos2.z)
                    return 0;
                else
                    return -1;
            else
                return -1;
        else
            return -1;
    }
}
