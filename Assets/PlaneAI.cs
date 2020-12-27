using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using ArcadeJets;

public class PlaneAI : Agent
{

    [SerializeField] Transform target;
    [SerializeField] float ceiling, floor;
    [SerializeField] float Xbound, Zbound;


    StickInput stickInput;
    Rigidbody planeRg;
    
    private void Start()
    {
        planeRg = GetComponent<Rigidbody>();
        stickInput = GetComponent<ArcadeJets.StickInput>();
    }


    public override void OnEpisodeBegin()
    {
        transform.position = RandomLocation();
        transform.rotation=Quaternion.Euler(0f,0f,0f);
        target.position= RandomLocation();
        stickInput.Throttle = 0.5f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.y); //altitude 
        sensor.AddObservation(target.position); //where the target is
        sensor.AddObservation(transform.rotation.eulerAngles.x); //current pitch
        sensor.AddObservation(transform.rotation.eulerAngles.z); //current bank angle
        sensor.AddObservation(ceiling); //max altitude
        sensor.AddObservation(floor); //min altitude
        sensor.AddObservation(planeRg.velocity.magnitude); //Aircraft speed;

    }


    public override void OnActionReceived(float[] vectorAction)
    {
        float pitchCommand = vectorAction[0];
        float rollCommand = vectorAction[1];
        float throttle = vectorAction[2];
       

        
        stickInput.ApplyThrottle(throttle);
        stickInput.Pitch = pitchCommand;
        stickInput.Roll = rollCommand;

    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Vertical");
        actionsOut[1] = -Input.GetAxis("Horizontal");
        actionsOut[2] = _ApplyThrust();

        stickInput.ApplyThrottle(actionsOut[2]);
        stickInput.Pitch = actionsOut[0];
        stickInput.Roll = actionsOut[1];
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }
        if (other.tag.Equals("Finish"))
        {
            SetReward(1f);
            EndEpisode();
        }
    }   

    Vector3 RandomLocation()
    {
        return new Vector3(Random.Range(-Xbound, Xbound), Random.Range(ceiling, floor), Random.Range(-Zbound, Zbound));
    }
    float _ApplyThrust()
    {
        if (Input.GetButton("ThrottleUp"))
            return 1f;
        if (Input.GetButton("ThrottleDown"))
            return -1f;
        else
            return 0f;
    }
}
