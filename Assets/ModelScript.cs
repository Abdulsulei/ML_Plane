using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class ModelScript : MonoBehaviour
{
    [SerializeField] GameObject afterBurner;
    [HideInInspector]
   public bool burnerOn = false;
    [SerializeField] GameObject elevators;
    public float rate = 5f;
    [SerializeField] ParticleSystem ps;
    ParticleSystem.EmissionModule gun;

    bool firing = false;
    void Start()
    {
        Rigidbody[] rgs = GetComponentsInChildren<Rigidbody>();
        foreach (var rg in rgs)
        {
            rg.isKinematic = true;
        }
        gun = ps.emission;
    }

    private void Update()
    {
       float pitch = Input.GetAxis("Vertical");
        AfterBurner();
        var rot= new Vector3(pitch, 0f, 0f);
        elevators.transform.localRotation = Quaternion.Euler(pitch*-rate, 0f, 0f);
        Shoot();
    }

    private void AfterBurner()
    {
        if (burnerOn && !afterBurner.activeSelf)
        {
            afterBurner.SetActive(true);
        }
        if (!burnerOn && afterBurner.activeSelf)
        {
            afterBurner.SetActive(false);
        }
    }

    public void Shoot()
    {
        firing = Input.GetButton("Fire1");        
        gun.enabled = firing;
    }
}
