﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatus : MonoBehaviour
{
    public PlayerStatus playerStatus;
    public CharacterMovement characterMovement;

    [Header("Player Status")]
    public string playerName;
    public Animator anim;

    public List<MonsterAttack> monsterAttack = new List<MonsterAttack>();
    public bool isDamaged;

    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<PlayerStatus>();
        anim = GetComponent<Animator>();
    }

    public void Damaged(float dmg, Vector3 sourceVec)
    {
        anim.SetFloat("damagedPosY", 0);
        anim.SetFloat("damagedPosX", 0);
        Vector3 impact = sourceVec - transform.position;
        if (Mathf.Abs(impact.x) > Mathf.Abs(impact.z))
        {
            if (impact.x > 0)
                anim.SetFloat("damagedPosX", 1);
            else if (impact.x < 0)
                anim.SetFloat("damagedPosX", -1);
        }
        else {
            if (impact.y > 0)
                anim.SetFloat("damagedPosY", 1);
            else if (impact.y < 0)
                anim.SetFloat("damagedPosY", -1);
        }

        anim.SetTrigger("damaged");
        playerStatus.curHealth -= dmg;
        playerStatus.RefreshHp();
    }

    public void Blocked(float dmg)
    {
        anim.SetTrigger("blocking");
    }
}