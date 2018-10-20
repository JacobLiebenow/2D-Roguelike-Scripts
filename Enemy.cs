using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    // Use this for initialization
    protected override void Start () {
        //Add the enemy to the GameManager list, add the local animator, and find the player to provide it as the main target
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}

    //Override the generic MoveObject function as it relates to the enemies
    protected override void AttemptMove<T> (int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy ()
    {
        int xDir = 0;
        int yDir = 0;

        //If the player's x position is higher than the enemy's, move position up 1.  Otherwise, move down 1.
        //Likewise, if the player's y position is higher, move right 1 and vice versa
        //
        //In other words, move by y first, followed by x.
        if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        } else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir);
    }

    //If an enemy can't move, it's because a player is there.  Take their health away using this overriden generic function as previously defined in MovingObject
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("enemyAttack");

        hitPlayer.LoseFood(playerDamage);

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}
