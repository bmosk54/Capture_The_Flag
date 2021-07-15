using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    
    public float gunRange = 30f;
    private int ammo = 5;
    public Camera gunCam;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public LayerMask flagMask;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public AudioSource gunshot;
    Transform flag = null;

    public float fireRate = 15f;
    private float nextFire = 0f;
    private float reloadTime = 2.5f;

    void Update() {
        if (ammo > 0 && 
            (transform.name.Contains("1") && Input.GetKeyDown(KeyCode.Space) 
            || transform.name.Contains("2") && Input.GetKeyDown(KeyCode.Q)) 
            && Time.time >= nextFire) {
            nextFire = Time.time + 1f / fireRate;
            Shoot();
            ammo--;
        }
        if (ammo == 0) {
            StartCoroutine(reload(reloadTime));
        }
    }

    void Shoot() {
        muzzleFlash.Play();
        gunshot.Play();

        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hitInfo, gunRange, obstacleMask)) {
            GameObject impact = Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            impact.GetComponent<ParticleSystem>().Play();
            Destroy(impact, 1f);
        } else if (Physics.Raycast(ray, out hitInfo, gunRange, playerMask)) {
            Transform targetHit = hitInfo.transform;
            foreach(Transform child in targetHit) {
                if (child.tag == "Flag") {
                    flag = child;
                }
            }
            if (flag != null) {
                flag.parent = null;
            }
            Destroy(hitInfo.transform.gameObject);
        } else if (Physics.Raycast(ray, out hitInfo, gunRange, flagMask)) { 
            if (hitInfo.transform.tag == "Player") {
                Transform destroyedPlayer = hitInfo.transform;
                foreach(Transform child in destroyedPlayer) {
                    if (child.tag == "Flag") {
                        flag = child;
                    }
                }
                if (flag != null) {
                    flag.parent = null;
                }
                Destroy(destroyedPlayer.gameObject);
            }
        }
    }

    IEnumerator reload(float reloadTime) {
        yield return new WaitForSeconds(reloadTime);
        ammo = 5;
    }

}