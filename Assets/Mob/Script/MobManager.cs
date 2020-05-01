using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    const int limitMobNum = 15;
    private int mobNum;

    public void AddMobNum() { mobNum++; }
    public void ReduceMobNum(){ mobNum--; }
    public bool CheakMobNum() {
        if(limitMobNum > mobNum)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
}
