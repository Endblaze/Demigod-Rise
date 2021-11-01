using System.Collections;
using UnityEngine;

public class Clayrak_Manager : Player_Manager
{

    private float[] spAttackCost;

    //Start
    protected void Start()
    {

        //Time.timeScale = .1f;

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
        comboIdle[0] = hitboxes[1];
        comboIdle[1] = hitboxes[0];

        //Move Combo
        comboMove = new Transform[2];
        comboMove[0] = hitboxes[1];
        comboMove[1] = hitboxes[1];

        //Up Combo
        comboUp = new Transform[1];
        comboUp[0] = hitboxes[1];

        //Down Combo
        comboDown = new Transform[1];
        comboDown[0] = hitboxes[1];

    }

    //Normal Attacks
    private void NormalAttacks()
    {

        if (beaten || rightSpeed != 0) { return; } //Si estamos siendo golpeados o esquivando no atacaremos

        if (Input.GetKeyDown(k_hit) || ai_hit)
        {

            stunned = true;      //Stun que evita el movimiento al atacar
            running = 0;

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
            if ((tag == "PlayerOne" && ((Input.GetKey(k_up) || ai_up))) || (tag == "PlayerTwo") && ((Input.GetKey(k_down) || ai_up)))
            {

                anim.SetTrigger("Up Button");
                anim.SetInteger("Up Combo", comboUp_count);
                anim.SetBool("Idle Button", false);

                return;

            }

            //Down Combo
            if ((tag == "PlayerOne" && ((Input.GetKey(k_down) || ai_down))) || (tag == "PlayerTwo") && ((Input.GetKey(k_up) || ai_down)))
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

    #region Special Attacks

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

        if (stunned || beaten) { return; }

        //Down Special Attack - Charge
        if ((tag == "PlayerOne" && ((Input.GetKey(k_down)))) || (tag == "PlayerTwo") && ((Input.GetKey(k_up))) || ai_sp4)
        {

            SpAttack4();

            if (darkPower) { SpAttack1(false); }

            GenerateSound(sounds[2], false);
            particlesMain[0].Play();

            if (ai_special) { ai_special = false; }

            return;

        }

        if (rightSpeed != 0) { return; }

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

        //Idle Special Attack

        if (energy - spAttackCost[0] <= 0) { return; }

        if(ai_special && !ai_sp1) { return; }

        SpAttack1(true);

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

        spAttackCost[0] = 0;
        spAttackCost[1] = 25;
        spAttackCost[2] = 50;

        //Special Attack 1
        particlesEmission[2].enabled = false;

        //Special Attack 4 - Charge
        particlesEmission[1].enabled = false;

    }

    private void UpdateSpecialAttacks()
    {

        //Disable DarkPower
        if ((energy <= 0 && darkPower) || (life <= 0 && darkPower))
        {

            SpAttack1(false);

        }
        else if (energy > 0 && particlesEmission[2].enabled)
        {
            energy -= 5 * Time.deltaTime;
        }

        //Dark Dash
        if (darkDash > 0)
        {

            DarkDash();

        }
        else if (dashTrail.time > 0)
        {
            dashTrail.time -= Time.deltaTime;
        }

        //Spin Attack
        if (spinCount > 0)
        {

            anim.ResetTrigger("Beaten");

            spinCount -= Time.deltaTime;

            transform.Rotate(0, 1000 * Time.deltaTime, 0);

            //Create hit
            if ((tag == "PlayerOne" && !GameObject.FindGameObjectWithTag("Hit_PlayerOne")) || (tag == "PlayerTwo" && !GameObject.FindGameObjectWithTag("Hit_PlayerTwo")))
            {

                GenerateSound(sounds[1], true);
                PoolManager.Instance.RequestPool(tag, hitboxes[0], damage, .2f);
                PoolManager.Instance.RequestPool(tag, hitboxes[1], damage, .2f);
            }

        }
        else if (invincible)
        {

            invincible = false;

            QuitBeaten();

            anim.SetBool("Invincible", false);

        }

        //Charge
        if (charging && (Input.GetKey(k_special) || ai_sp4) && !beaten && !darkPower)
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

    private bool darkPower;

    private float darkSpeed = 3, darkAttack = 1;

    private void SpAttack1(bool stun)
    {

        if (stun)
        {

            GenerateSound(sounds[4], false);

            stunned = true;
            forwardSpeed = 0;

            anim.SetTrigger("Special");

        }

        if (!darkPower)
        {

            darkPower = true;

            particlesEmission[2].enabled = true;

            walkSpeed += darkSpeed;
            runSpeed += darkSpeed;
            damage += darkAttack;

        }
        else
        {

            DisableDarkPower();

        }

    }

    private void DisableDarkPower()
    {

        darkPower = false;

        particlesEmission[2].enabled = false;

        walkSpeed -= darkSpeed;
        runSpeed -= darkSpeed;
        damage -= darkAttack;

    }

    //Special Attack 2

    private bool darkDashBool;

    public TrailRenderer dashTrail;

    private float darkDash;
    private float dashTime = 0.3f;

    private void SpAttack2()
    {

        stunned = true;
        forwardSpeed = 0;

        anim.SetTrigger("Special");
        anim.SetTrigger("Right Button");

        GenerateSound(sounds[5], false);

        energy -= spAttackCost[1];

        darkDash = dashTime;

        darkDashBool = true;

        dashTrail.time = .2f;

    }

    private void DarkDash()
    {

        transform.position += transform.forward * (runSpeed + 2) * Time.deltaTime;

        darkDash -= Time.deltaTime;

        if (darkDashBool)
        {

            darkDashBool = false;

            GameObject[] currentlyHit = new GameObject[2];

            currentlyHit[0] = PoolManager.Instance.RequestPool(tag, hitboxes[0], damage, .9f);
            currentlyHit[1] = PoolManager.Instance.RequestPool(tag, hitboxes[1], damage, .9f);

        }

    }

    //Special Attack 3

    private float spinTime = 1f, spinCount;

    private void SpAttack3()
    {

        stunned = true;
        beaten = true;

        forwardSpeed = 0;

        energy -= spAttackCost[2];

        anim.SetTrigger("Special");
        anim.SetTrigger("Up Button");

        spinCount = spinTime;

        invincible = true;
        anim.SetBool("Invincible", true);

        anim.ResetTrigger("Beaten");

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

    #endregion

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

            random = Random.Range(0,4);

            distance = Vector3.Distance(transform.position, target.position);

            if(distance < 0.2f || (energy < 10 && !ai_sp4))
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

            if(distance < 1.3f)
            {

                ai_hit = true;

                yield return new WaitForSeconds(Random.Range(0.2f,0.5f));

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

            //Spin
            if (beaten && (life < maxLife - maxLife/4) && distance < 2 && (energy - spAttackCost[2] >= 0))
            {

                ai_special = true;
                ai_sp3 = true;

                yield return new WaitForSeconds(Random.Range(2, 4));

                continue;

            }

            if (!ai_sp4)
            {

                //DarkPower
                if ((energy > 40 && !darkPower && !ai_sp4))
                {

                    ai_special = true;
                    ai_sp1 = true;

                    yield return new WaitForSeconds(1f);

                    continue;

                }
                else if (distance <= 5 && energy >= spAttackCost[1])
                {

                    //DarkDash
                    ai_special = true;
                    ai_sp2 = true;

                    yield return new WaitForSeconds(Random.Range(1, 3));

                    continue;

                }
                else if (distance > 5 && energy < 50)
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