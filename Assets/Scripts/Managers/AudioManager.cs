using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioClip shuffleCards;
    [SerializeField] AudioClip cardPull;
    [SerializeField] AudioClip coinSingle;
    [SerializeField] AudioClip coinStack;
    [SerializeField] AudioClip victoryStinger;
    static AudioSource audioSrc;
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance=this;
            audioSrc = GetComponent<AudioSource>();
        }
    }
    public static void PlayShuffle()
    {
        audioSrc.clip = instance.shuffleCards;
        audioSrc.Play();
    }
    public static void PlayCardPull()
    {
        audioSrc.clip = instance.cardPull;
        audioSrc.Play();
    }
    public static void PlayCoinSingle()
    {
        audioSrc.clip = instance.coinSingle;
        audioSrc.Play();
    }
    public static void PlayCoinStack()
    {
        audioSrc.clip = instance.coinStack;
        audioSrc.Play();
    }
    public static void PlayVictory()
    {
        audioSrc.clip = instance.victoryStinger;
        audioSrc.Play();
    }
}
