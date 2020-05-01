using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    [SerializeField] private AudioClip walkSE = null;
    [SerializeField] private AudioClip bagSE = null;
    [SerializeField] private AudioClip craftSE = null;
    [SerializeField] private AudioClip eatSE = null;
    [SerializeField] private AudioClip punchSE = null;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        //Componentを取得
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayBagSE()
    {
        audioSource.PlayOneShot(bagSE);
    }
    public void PlayCraftSE()
    {
        audioSource.PlayOneShot(craftSE);
    }
    public void PlayEatSE()
    {
        audioSource.PlayOneShot(eatSE);
    }
    void Foot()
    {
        audioSource.PlayOneShot(walkSE);
    }
    void Punch()
    {
        audioSource.PlayOneShot(punchSE);
    }
}
