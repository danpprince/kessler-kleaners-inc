using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starting_Position : MonoBehaviour
{
    public GameObject platform;
    public Vector3 starting_position;
    public Vector3 size;
    public MeshRenderer renderer;
    private Renderer player_render;

    public GameObject player;
    public Vector3 player_size;
    private float width;
    public bool before_stroke = true;
    private float hitInput;
    private float stopInput;
    private float stop_timer;
    public float stop_timer_value;
    private float default_movementSpeed;
    public float hit_power;

    private float h;

    private void OnEnable()
    {
        stop_timer = stop_timer_value;
    }

    void Start()
    {

        player_size = player.GetComponent<MeshRenderer>().bounds.size;
        width = player_size.x;
        renderer = platform.GetComponent<MeshRenderer>();
        h = renderer.bounds.size.y;
        starting_position = renderer.bounds.center + new Vector3 (0f, ((h/2) + (width/2)), 0f);
        gameObject.transform.position = starting_position;
        before_stroke = true;
        default_movementSpeed = player.GetComponent<KatamariMovement>().movementSpeed;

    }

    // Update is called once per frame *****Move only the counter to Fixed !!!!!********
    void FixedUpdate()
    {

        hitInput = Input.GetAxis("Jump") * 10;
        stopInput = Input.GetAxis("Stop") * 10;


        // Once the player "hits" the ball unfreeze its position
        if (hitInput > 0.5)
        {
            before_stroke = false;
            
        }

        

        if (before_stroke)
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            player.GetComponent<KatamariMovement>().movementSpeed = hit_power;
        }

        else
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            player.GetComponent<KatamariMovement>().movementSpeed = default_movementSpeed;
        }

        // Start a timer that counts down while player is trying to stop the ball
        if (stopInput > 0.5)
        {
            stop_timer -= 0.1f;
        }
        else
        {
            stop_timer = stop_timer_value;
        }

        // After the timer runs out, freeze the position and set the Before Stroke bool to true again

        if (stopInput > 0.5 && stop_timer < 0.5)
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            before_stroke = true;
        }

        //print(stop_timer);
        print(before_stroke);
    }
}
