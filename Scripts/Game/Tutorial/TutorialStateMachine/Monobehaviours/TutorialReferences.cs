using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;

public class TutorialReferences : MonoBehaviour
{
    public Animator hintAnimator;
    public TextMeshProUGUI hintTMP;
    public AudioSource AudioSource { get; private set; }
    public AudioClip newHintSfx;
    public AudioClip objectiveCompleteSfx;
    public Animator gateAnimator;
    public MeeleController PlayerMeeleController;

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
    }
}
