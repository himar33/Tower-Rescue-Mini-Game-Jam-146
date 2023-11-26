using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float mSpeed = 5f;
    [SerializeField] private Transform mFloor;

    private Rigidbody mRb;
    private Vector3 mMovementDirection;
    private Animator mAnimator;
    private SpriteRenderer mSpriteRenderer;

    private void Awake()
    {
        mRb = GetComponent<Rigidbody>();
        mMovementDirection = Vector3.zero;
        mAnimator = GetComponentInChildren<Animator>();
        mSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(hInput, vInput);
        Vector3 movementDirection = mFloor.rotation * inputDirection;

        movementDirection.Normalize();

        mMovementDirection = movementDirection;
        mAnimator.SetFloat("Speed", inputDirection.magnitude);
        if (hInput != 0)
        {
            mSpriteRenderer.flipX = (hInput > 0) ? false : true;
        } 
    }

    private void FixedUpdate()
    {
        mRb.velocity = mMovementDirection * mSpeed * Time.fixedDeltaTime;
    }
}
