using System.Collections;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{

    //Variables públicas

    public float maxLife = 100, life, energy = 100;                         //Vida y energía del personaje

    public GameObject hitbox;                                               //Hitbox que usará el personaje en los ataques
    public Transform[] hitboxes;                                            //Array con todos los puntos que golpearán

    public AudioClip[] sounds;                                              //Sonidos

    //Variables de controles y movimiento

    protected KeyCode k_right, k_left, k_up, k_down, k_hit, k_special;                  //Controles jugador
    protected bool ai_right, ai_left, ai_up, ai_down, ai_hit, ai_special, ai_sp1, ai_sp2, ai_sp3, ai_sp4;    //Controles AI

    protected bool aiMode;                                                  //Modo AI
    public bool dummyMode;                                                  //Modo dummy para el selector de personajes

    protected bool dead;                                                    //Controla si el player está muerto

    protected float walkSpeed = 2, runSpeed = 10;                           //Constantes de velocidad

    protected float doubleTap = 0.3f, forwardSpeed, rightSpeed;             //Tiempo de doble tap y velocidad actual

    protected float runTap, running;                                        //Para detectar el doble tap al correr
    protected float dodgeTap, dodging;                                      //Para detectar el doble tap al dodgear

    private Vector3 flinchDir = -Vector3.one;                               //Dirección de empuje al ser golpeado

    protected Transform target;                                             //Target al que mira

    //Variables para animaciones y control de acciones
    
    protected float animSpeed, animDodge = 0, finalAnimSpeed = 0;                   //Controlar animación a ejecutar

    protected bool stunned, beaten, invincible;                             //Parar movimiento y acciones al atacar y ser golpeado

    private bool showEnd = true;                                            //Para los efecos al final


    //Variables que referencian componentes

    protected Animator anim;
    private Rigidbody rb;
    public AudioSource aS1, aS2;                                         //El aS1 lo usamos para pitchs random, el segundo para pitchs = 1

    //Variables para control de combos

    protected Transform[] comboIdle, comboMove, comboUp, comboDown; //Array de combos (cada uno contiene sus correspondientes puntos de combo en orden de ejecución)
    protected int comboIdle_count, comboMove_count, comboUp_count, comboDown_count; //Contadores de combo

    //Variables para partículas
    public ParticleSystem[] particlesMain;
    protected ParticleSystem.EmissionModule[] particlesEmission;

    //Variables de daño
    public float damage = 1;

    //Start
    protected void ManagerStart()
    {

        if(Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }

        //Asignamos los EmissionModule
        particlesEmission = new ParticleSystem.EmissionModule[particlesMain.Length];

        for (int i = 0; i < particlesMain.Length; i++)
        {

            particlesEmission[i] = particlesMain[i].emission;

        }

        //Dummy Mode >> Return
        if (dummyMode) { return; }

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        //Asignación de la vida a las lifebars
        life = maxLife;
        Game_Controller.Instance.lifebar1.maxValue = life;
        Game_Controller.Instance.lifebar2.maxValue = life;
        Game_Controller.Instance.energybar1.maxValue = energy;
        Game_Controller.Instance.energybar2.maxValue = energy;

        //Asignamos el target tras determinar nuestro tag
        if (tag == "PlayerOne")
        {

            target = GameObject.FindGameObjectWithTag("PlayerTwo").transform;
            transform.position = new Vector3(-2, 0, 0);
            transform.eulerAngles = new Vector3(0, 90, 0);

        }
        else if (tag == "PlayerTwo")
        {

            target = GameObject.FindGameObjectWithTag("PlayerOne").transform;
            transform.position = new Vector3(2, 0, 0);
            transform.eulerAngles = new Vector3(0, -90, 0);

        }

        //Asignamos los controles del jugador
        PlayerControls();

    }

    //Update
    protected void ManagerUpdate()
    {

        Movement();     //Caminar y correr
        Dodge();        //Esquivar
        LookAtEnemy();  //Mirar hacia el enemigo

        LifeControl();

        if(energy < 0) { energy = 0; }
        else if(energy > 100) { energy = 100; }

        if(transform.position.y < 0)
        {
            transform.position = transform.position - new Vector3(0, transform.position.y, 0);
        }

    }

    private void FixedUpdate()
    {
        
        if(flinchDir != -Vector3.one)
        {
            rb.AddForce(flinchDir * 2, ForceMode.Impulse);
            flinchDir = -Vector3.one;
        }

    }

    //Movimiento
    private void Movement()
    {

        if (stunned || beaten) { return; }

        //Al pulsar solo una vez (Caminar)
        if (Input.GetKey(k_left) || ai_left)
        {

            forwardSpeed = -walkSpeed;

            if (running > 0 && animDodge == 0)
            {
                forwardSpeed = -runSpeed;
                running = doubleTap;
            }

        }
        else if (Input.GetKey(k_right) || ai_right)
        {

            forwardSpeed = walkSpeed;

            if (running > 0 && animDodge == 0)
            {
                forwardSpeed = runSpeed;
                running = doubleTap;
            }

        }
        else
        {
            forwardSpeed = 0;
        }


        //Al pulsar por segunda vez (Correr)
        if (Input.GetKeyDown(k_left) || Input.GetKeyDown(k_right) || ai_left || ai_right)
        {

            if ((Time.time - runTap) < doubleTap)
            {
                running = doubleTap;
            }

            runTap = Time.time;

        }

        if (running > 0) { 
            
            running -= Time.deltaTime;
            
            if(running < 0) { running = 0; }

        }

        //Asignamos la velocidad final
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;

    }

    //Esquivar
    private void Dodge()
    {

        if (stunned || beaten) {

            if (animDodge != 0) { animDodge = 0; }

            return;
        
        }

        if ((Input.GetKey(k_up) || ai_up) && dodging>0)
        {
            
            rightSpeed = -runSpeed;
            dodging = doubleTap;
            animDodge = 1;

        }
        else if ((Input.GetKey(k_down) || ai_down) && dodging > 0)
        {

            rightSpeed = runSpeed;
            dodging = doubleTap;
            animDodge = -1;

        }
        else
        {
            rightSpeed = 0;
            animDodge = 0;
        }

        if (Input.GetKeyDown(k_up) || Input.GetKeyDown(k_down) || ai_up || ai_down)
        {

            if ((Time.time - dodgeTap) < doubleTap)
            {
                dodging = doubleTap;
            }

            dodgeTap = Time.time;

        }

        if (dodging> 0) { dodging -= Time.deltaTime; }

        transform.position += transform.right * rightSpeed * Time.deltaTime;

    }

    //Mirar hacia el enemigo
    private void LookAtEnemy()
    {

        if (stunned || beaten) { return; }

        Vector3 targetPos = target.position;

        targetPos.y = transform.position.y;

        transform.LookAt(targetPos);

    }

    //Actualizar animaciones
    protected void UpdateAnimations()
    {

        if (dummyMode) { return; }

        
        if(forwardSpeed == walkSpeed)
        {
            animSpeed = 1;
        }
        else if(forwardSpeed == -walkSpeed)
        {
            animSpeed = -1;
        }
        else if (forwardSpeed == runSpeed)
        {
            animSpeed = 2;
        }
        else if (forwardSpeed == -runSpeed)
        {
            animSpeed = -2;
        }
        else if(forwardSpeed == 0)
        {
            animSpeed = 0;
        }

        //Smooth animation

        if (Mathf.Abs(animSpeed) == 2)
        {
            finalAnimSpeed = animSpeed;
        }

        if (Mathf.Abs(finalAnimSpeed) <= 0.01f && animSpeed == 0)
        {
            finalAnimSpeed = animSpeed;
        }

        if (Mathf.Abs((finalAnimSpeed - animSpeed)) > 0.1f)
        {

            if (finalAnimSpeed < animSpeed)
            {
                finalAnimSpeed += Time.deltaTime * 10;
            }
            else if (finalAnimSpeed > animSpeed)
            {
                finalAnimSpeed -= Time.deltaTime * 10;
            }

        }
        
    }

    //Controles del jugador
    private void PlayerControls()
    {

        Controls_Object controls = Controls_Manager.Instance.controls;

        if (!aiMode) {

            if (tag == "PlayerOne")
            {
                k_left = controls.player1[1];
                k_right = controls.player1[0];
                k_up = controls.player1[2];
                k_down = controls.player1[3];
                k_hit = controls.player1[4];
                k_special = controls.player1[5];
            }

            if (tag == "PlayerTwo")
            {
                k_left = controls.player2[0];
                k_right = controls.player2[1];
                k_up = controls.player2[3];
                k_down = controls.player2[2];
                k_hit = controls.player2[4];
                k_special = controls.player2[5];
            }

        }
        else
        {
            EnableAI();
        }

    }

    //Collisions con los ataques
    private void OnTriggerEnter(Collider c)
    {

        if((tag == "PlayerOne" && c.tag == "Hit_PlayerTwo") || (tag == "PlayerTwo" && c.tag == "Hit_PlayerOne"))
        {

            if (!invincible) {

                anim.Play("Beaten");

                flinchDir = -transform.forward;

                float damage = c.GetComponent<Hitbox_Manager>().damage;

                GetDamage(damage);

                energy += 3 * damage;

                if (damage != 0)
                {
                    target.GetComponent<Player_Manager>().energy += 2;
                }

                stunned = true;
                beaten = true;

                Game_Controller.Instance.ResetHitCounter(tag);

            }

            if (aiMode) { ai_sp4 = false; }

            GenerateSound(sounds[0], true);

            c.gameObject.SetActive(false);

        }

    }

    //Generar sonidos
    protected void GenerateSound(AudioClip snd, bool pitch)
    {

        if (pitch) {

            aS1.pitch = Random.Range(1, 3);
            aS1.PlayOneShot(snd);

        }
        else
        {

            aS2.PlayOneShot(snd);

        }

    }

    //Animation Event - Quitar stun una vez termine la animación de ataque
    private void QuitStun()
    {
        stunned = false;
    }

    //Animation Event - Quitar stun una vez termine la animación de ser golpeado
    protected void QuitBeaten()
    {

        stunned = false;
        beaten = false;

    }

    //Animation Event - Generar golpes (hitbox del golpe)
    private GameObject createdPunch;
    private void CreateHit(string combo)
    {

        running = 0;
        forwardSpeed = 0;

        GameObject currentlyHit;

        GenerateSound(sounds[1], true);

        switch (combo)
        {

            case "idle":

                currentlyHit = PoolManager.Instance.RequestPool(tag, comboIdle[comboIdle_count], damage, 0);

                if (comboIdle_count < comboIdle.Length - 1) { comboIdle_count++; } else { comboIdle_count = 0; }
                break;

            case "move":

                currentlyHit = PoolManager.Instance.RequestPool(tag, comboMove[comboMove_count], damage, 0);

                if (comboMove_count < comboMove.Length - 1) { comboMove_count++; } else { comboMove_count = 0; }
                break;

            case "up":

                currentlyHit = PoolManager.Instance.RequestPool(tag, comboUp[comboUp_count], damage, 0);

                if (comboUp_count < comboUp.Length - 1) { comboUp_count++; } else { comboUp_count = 0; }
                break;

            case "down":

                currentlyHit = PoolManager.Instance.RequestPool(tag, comboDown[comboDown_count], damage, 0);

                if (comboDown_count < comboDown.Length - 1) { comboDown_count++; } else { comboDown_count = 0; }
                break;

        }

    }

    //Recibir daño
    private void GetDamage(float dmg)
    {

        life -= dmg * 2;

    }

    //Control de vida
    private void LifeControl()
    {

        //Cambio de la lifebar y energybar
        if (tag == "PlayerOne") {
            Game_Controller.Instance.lifebar1.value = life;
            Game_Controller.Instance.energybar1.value = energy;

        }
        else if(tag == "PlayerTwo") {

            Game_Controller.Instance.lifebar2.value = life;
            Game_Controller.Instance.energybar2.value = energy;

        }

        //La vida baja con el contador en negativo
        if (Game_Controller.Instance.time < 0)
        {
            life -= Time.deltaTime * 2;
        }

        //Training Mode
        if(Game_Controller.gameMode == 2)
        {

            if (life < maxLife)
            {
                life += Time.deltaTime * 1.5f;

                if(life <= maxLife / 2)
                {

                    life += Time.deltaTime * 2;

                    if(life <= 0)
                    {

                        life = 0;

                    }

                }

            }
            else
            {
                life = maxLife;
            }

            return;

        }

        //Muerte
        if (life <= 0)
        {

            Game_Controller.Instance.time = 0;
            
            if (!dead)
            {
                anim.SetTrigger("Death");
                dead = true;

                if (tag == "PlayerOne") { Game_Controller.Instance.lifebar1.value = 0; }
                else if (tag == "PlayerTwo") { Game_Controller.Instance.lifebar2.value = 0; }

            }

            rb.isKinematic = true;
            GetComponent<BoxCollider>().enabled = false;

        }

        //Los jugadores paran completamente al terminar la partida
        if(Game_Controller.Instance.lifebar1.value <= 0 || Game_Controller.Instance.lifebar2.value <= 0)
        {

            beaten = true;
            anim.SetFloat("Speed", 0);
            anim.SetFloat("Dodge", 0);

            if (showEnd)
            {

                showEnd = false;

                GenerateSound(sounds[0], false);
                aS1.pitch = 0.5f;

            }

        }

    }

    public void EnableAI()
    {
        aiMode = true;
    }
    
}