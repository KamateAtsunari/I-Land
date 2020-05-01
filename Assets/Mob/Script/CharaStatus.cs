using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create CharaStatus")]
public class CharaStatus : ScriptableObject
{
    public int statusHp;
    public int statusAttack;
    public int statusDefence;
    public float statusWalkSpeed;
    public float statusRunSpeed;

    public int GetMobHp() { return statusHp; }
    public int GetMobAttack() { return statusAttack; }
    public int GetMobDefence() { return statusDefence; }
    public float GetWalkSeed() { return statusWalkSpeed; }
    public float GetRunSeed() { return statusRunSpeed; }
}
