using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Helicopter : MonoBehaviour
{
    private Animator _animator;
    private AudioSource _audioSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GameManager.Instance.OnObjectiveComplete += StartHelicopter;
    }

    private void StartHelicopter()
    {
        _animator.enabled = true;
        _audioSource.enabled = true;
    }
}
