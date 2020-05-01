using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    public CharaStatus charaStatus = default;

    private int characterHp;

    // Start is called before the first frame update
    void Start()
    {
        characterHp = charaStatus.statusHp;
    }
}
