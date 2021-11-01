using System.Collections;
using UnityEngine;

public class Endblaze_Manager : Player_Manager
{

    private float[] spAttackCost;

    //Start
    protected void Start()
    {

        ManagerStart();
        SetCombos();

        SetSpecialAttacks();

        if (aiMode)
        {
            StartCoroutine("AIMovement");
            StartCoroutine("AINormalAttacks");
            StartCoroutine("AISpecialAttacks");
        }

    }

    //Update
    private void Update()
    {

        if (dummyMode || dead || Game_Controller.Instance.initialTime > 1) { return; }

        ManagerUpdate();

        NormalAttacks();
        SpecialAttacks();

        UpdateSpecialAttacks();

        UpdateAnimations();

    }

    //LateUpdate
    private void LateUpdate()
    {

        if (dummyMode) { return; }

        anim.SetFloat("Speed", finalAnimSpeed);
        anim.SetFloat("Dodge", animDodge);

    }


    //Asignación de todos los combos usando el array "hitboxes"
    private void SetCombos()
    {

        //Idle Combo
        comboIdle = new Transform[2];
        comboIdle[0] = hitboxes[0];
        comboIdle[1] = hitboxes[1];

        //Move Combo
        comboMove = new Transform[2];
        comboMove[0] = hitboxes[3];
        comboMove[1] = hitboxes[2];

        //Up Combo
        comboUp = new Transform[1];
        comboUp[0] = hitboxes[3];

        //Down Combo
        comboDown = new Transform[1];
        comboDown[0] = hitboxes[1];

    }

    //Normal Attacks
    private void NormalAttacks()
    {

        if(beaten || rightSpeed!=0) { return; } //Si estamos siendo golpeados o esquivando no atacaremos

        if (Input.GetKeyDown(k_hit) || ai_hit)
        {

            stunned = true;      //Stun que evita el movimiento al atacar
            running = 0;        //Después de pegar deja de correr

            anim.SetTrigger("Attack");  //Activamos el trigger de ataque

            //Move Combo
            if (Input.GetKey(k_right) || ai_right)
            {

                anim.SetTrigger("Right Button");  //Activamos el trigger de ataque
                anim.SetInteger("Move Combo", comboMove_count);
                anim.SetBool("Idle Button", false);

                return;

            }

            //Up Combo
            if ((tag == "PlayerOne" && (Input.GetKey(k_up) || ai_up)) || (tag == "PlayerTwo") && (Input.GetKey(k_down) || ai_down))
            {

                anim.SetTrigger("Up Button");
                anim.SetInteger("Up Combo", comboUp_count);
                anim.SetBool("Idle Button", false);

                return;

            }
            
            //Down Combo
            if ((tag == "PlayerOne" && (Input.GetKey(k_down) || ai_down)) || (tag == "PlayerTwo") && (Input.GetKey(k_up) || ai_up))
            {

                anim.SetTrigger("Down Button");
                anim.SetInteger("Down Combo", comboDown_count);
                anim.SetBool("Idle Button", false);

                return;

            }

            //Idle Combo

            anim.SetInteger("Idle Combo", comboIdle_count);
            anim.SetBool("Idle Button", true);

        }

    }

    //Special Attacks
    private void SpecialAttacks()
    {

        if ((!Input.GetKeyDown(k_special) && !ai_special) || Input.GetKey(k_hit) || ai_hit) { return; }

        //Up Special
        if ((tag == "PlayerOne" && Input.GetKey(k_up)) || (tag == "PlayerTwo") && (Input.GetKey(k_down)) || ai_sp3)
        {

            if (energy - spAttackCost[2] < 0) { return; }

            SpAttack3();

            GenerateSound(sounds[2], false);
            particlesMain[0].Play();

            if (ai_special) {
                ai_special = false;
                ai_sp3 = false;
            }

            return;

        }

        if (beaten) { return; }

        //Move Special
        if (Input.GetKey(k_right) || ai_sp2)
        {

            if (energy - spAttackCost[1] < 0) { return; }

            SpAttack2();

            GenerateSound(sounds[2], false);
            particlesMain[0].Play();


            if (ai_special) {
                ai_special = false;
                ai_sp2 = false;
            }

            return;

        }

        if (stunned) { return; }

        //Down Special Attack - Charge
        if ((tag == "PlayerOne" && ((Input.GetKey(k_down)))) || (tag == "PlayerTwo") && ((Input.GetKey(k_up))) || ai_sp4)
        {

            SpAttack4();

            GenerateSound(sounds[2], false);
            particlesMain[0].Play();

            if (ai_special) { ai_special = false; return; }

            return;

        }

        //Idle Special
        if (fireBall.activeSelf) { return; }

        if (energy - spAttackCost[0] < 0) { return; }

        if (ai_special && !ai_sp1) { return; }

        SpAttack1();

        GenerateSound(sounds[2], false);
        particlesMain[0].Play();


        if (ai_special) { 
            ai_special = false;
            ai_sp1 = false;
        }

    }

    private void SetSpecialAttacks()
    {

        if (!dummyMode) { particlesEmission[0].enabled = true; }

        spAttackCost = new float[3];

        spAttackCost[0] = 15;
        spAttackCost[1] = 20;
        spAttackCost[2] = 50;

        //Special Attack 1 - Fire Ball
        fireBall = Instantiate(fireBallPref, new Vector3(0, -10, 0), Quaternion.identity);
        fireBall.SetActive(false);
        fireBall.tag = tag;

        //Special Attack 2 - Teleport
        if (dummyMode){ particlesEmission[2].enabled = false; }
        particlesMain[2].Stop();

        //Special Attack 3 - Endhand
        endhand = Instantiate(endhandPref, new Vector3(0, -10, 0), Quaternion.Euler(180,0,0));
        endhand.tag = tag;
        endhand.SetActive(false);

        //Special Attack 4 - Charge
        particlesEmission[1].enabled = false;

    }

    private void UpdateSpecialAttacks()
    {

        //Special Attack 4 - Charge
        if (charging && (Input.GetKey(k_special) || ai_sp4) && !beaten)
        {

            energy += 20 * Time.deltaTime;

            chargeSndCD += Time.deltaTime;

            if (!stunned) { stunned = true; }

            if (chargeSndCD >= Random.Range(0.05f, 0.3f))
            {

                GenerateSound(sounds[Random.Range(0, 2) == 0 ? 2 : 3], false);
                chargeSndCD = 0;

            }

        }
        else if (charging)
        {

            charging = false;

            anim.SetBool("Charge", false);

            particlesEmission[1].enabled = false;

            stunned = false;

        }

    }

    //Special Attack 1
    public GameObject fireBallPref;
    private GameObject fireBall;

    private void SpAttack1()
    {

        stunned = true;
        forwardSpeed = 0;

        anim.SetTrigger("Special");

    }
    
    //Managed by animation
    public void ShootFireBall()
    {

        GenerateSound(sounds[4], false);

        PoolManager.Instance.RequestFireball(tag, hitboxes[0].position + new Vector3(0, 0.1f, 0), transform.forward);

        energy -= spAttackCost[0];

    }
    
    //Special Attack 2
    private void SpAttack2()
    {

        energy -= spAttackCost[1];
        particlesMain[2].Play();

        if (Vector3.Distance(transform.position, target.position) > 2)
        {
            transform.position = target.position - transform.forward;
        }
        else
        {
            transform.position = target.position + transform.forward / 2;
        }

    }

    //Special Attack 3
    public GameObject endhandPref;
    private GameObject endhand;

    private void SpAttack3()
    {

        energy -= spAttackCost[2];

        endhand.SetActive(true);
        endhand.transform.position = target.position + new Vector3(0, 5, 0);

    }

    //Special Attack 4
    private bool charging;

    float chargeSndCD;

    private void SpAttack4()
    {

        charging = true;
        stunned = true;
        particlesEmission[1].enabled = true;

        anim.SetBool("Charge", true);

    }

    #region Artificial Intelligence

    public IEnumerator AIMovement()
    {

        float random;
        float distance;

        while (true)
        {

            ai_right = false;
            ai_left = false;
            ai_up = false;
            ai_down = false;

            random = Random.Range(0, 4);

            distance = Vector3.Distance(transform.position, target.position);

            if (distance < 0.2f || (energy < 10 && !ai_sp4))
            {

                ai_left = true;

                yield return new WaitForSeconds(0.2f);

            }
            else if (distance < 5)
            {

                switch (random)
                {
                    case 0:
                        ai_right = true;
                        break;
                    case 1:
                        ai_up = true;
                        break;
                    case 2:
                        ai_down = true;
                        break;
                }

                if (beaten)
                {
                    ai_right = false;
                    ai_left = true;
                }

                yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));

            }
            else
            {

                ai_right = true;

                yield return new WaitForSeconds(0.5f);

            }

        }

    }

    public IEnumerator AINormalAttacks()
    {

        float random;
        float distance;

        while (true)
        {

            random = Random.Range(0, 5);

            distance = Vector3.Distance(transform.position, target.position);

            if (distance < 1.3f)
            {

                ai_hit = true;

                yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));

                ai_hit = false;

                continue;

            }

            yield return new WaitForSeconds(0.2f);

        }

    }

    public IEnumerator AISpecialAttacks()
    {

        float random;
        float distance;

        while (true)
        {

            random = Random.Range(0, 5);
            distance = Vector3.Distance(transform.position, target.position);

            //Stop Charge AI
            if (ai_sp4 && (energy >= 100 || distance < 5))
            {
                ai_sp4 = false;
            }

            if (!ai_sp4)
            {

                //Endhand
                if (beaten && (life < maxLife - maxLife / 4) && (energy - spAttackCost[2] >= 0))
                {

                    ai_special = true;
                    ai_sp3 = true;

                    yield return new WaitForSeconds(Random.Range(2, 4));

                    continue;

                }

                if (energy >= spAttackCost[0] || energy >= spAttackCost[1])
                {

                    switch (Random.Range(0, 2))
                    {
                        case 0:

                            if (energy >= spAttackCost[0])
                            {
                                ai_special = true;
                                ai_sp1 = true;
                            }

                            yield return new WaitForSeconds(Random.Range(1, 3));

                            break;
                        case 1:

                            if (energy >= spAttackCost[1])
                            {
                                ai_special = true;
                                ai_sp2 = true;
                            }

                            yield return new WaitForSeconds(Random.Range(1, 3));

                            break;
                    }

                    continue;

                }
                else if (distance > 5)
                {

                    //Charge
                    if (!ai_sp4) { ai_special = true; }
                    ai_sp4 = true;

                }

            }

            yield return new WaitForSeconds(0.2f);

        }

    }

    #endregion

}