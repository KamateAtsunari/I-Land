using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{

    public void SetDefense(int defense)
    {
        //builder.Append(transform.GetChild(2).GetComponent<Text>().text);
        transform.GetChild(2).GetComponent<Text>().text = transform.GetChild(2).GetComponent<Text>().text+defense.ToString();
    }
}
