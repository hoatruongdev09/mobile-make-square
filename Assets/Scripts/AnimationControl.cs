using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour {

    public string[] stateNames;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }
   
    public void PlayClip(int clipIndex) {
        animator.Play(stateNames[clipIndex]);
    }
    public void PlayReverseClip(int clipIndex) {
        Animation anim = GetComponent<Animation>();
        anim[stateNames[clipIndex]].speed = -1;
        anim[stateNames[clipIndex]].time = anim[stateNames[clipIndex]].length;
        animator.Play(stateNames[clipIndex]);
    }
}
