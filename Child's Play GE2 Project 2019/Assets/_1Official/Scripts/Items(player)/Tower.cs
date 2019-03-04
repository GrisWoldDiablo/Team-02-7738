﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    [SerializeField]
    private Transform towerTarget;

    [SerializeField]
    private Transform pivot;
    [SerializeField]
    private float rotationSpeed = 10f;
    [SerializeField]
    private string enemyTag = "Enemy";


    //FIRING PART
    [Header("Tower Option")]
    [SerializeField] private bool LookAtTarget = false;
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;

    private float range;// = 15f;
    private float rateOfFire;// = 1f;
    private float CountdownToNextFire = 0f;
    private float innerRadius;

    [SerializeField]
    private Tower_SO tower_SO;

    [Header("Tower VFX")]
    [SerializeField] private Light _lightEffect;
    [SerializeField] private ParticleSystem _vfxLaser;
    [SerializeField] private LineRenderer _lineRendererComponent;


    public Transform GetProjectileSpawnPoint
    { get { return projectileSpawnPoint; } }

    public Transform GetTowerTarget
    { get { return towerTarget; } }

    private void Start()
    {
        if (tower_SO.towerType == ProjectTileType.LASER)
        {
            _lightEffect = GetComponentInChildren<Light>();
            _vfxLaser = GetComponentInChildren<ParticleSystem>();
            _lineRendererComponent = GetComponentInChildren<LineRenderer>();
        }

        InvokeRepeating("UpdateTarget", 0f, 0.5f);

        SO_Reference();
    }

    void SO_Reference()
    {         
         range = tower_SO.range;
         rateOfFire = tower_SO.bulletPerSecond;
         innerRadius = tower_SO.innerRadius;
         projectilePrefab = tower_SO.projectilePrefab;
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float shortestDistance = Mathf.Infinity;

        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<Enemy>().IsDying)
            {
                continue;
            }
            float distanceToEnemy = Vector3.Distance(this.transform.position, enemy.transform.position);

            if(distanceToEnemy < shortestDistance && distanceToEnemy >= innerRadius)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if(nearestEnemy != null && shortestDistance <= range && shortestDistance >= innerRadius)
        {
            towerTarget = nearestEnemy.transform;
        }
        else
        {
            towerTarget = null;
        }
    }

    private void Update()
    {
        CountdownToNextFire -= Time.deltaTime;
        if (towerTarget == null)
        {
            StopLaser();
            return;
        }

        //Target Lock
        if (LookAtTarget)
        {
            pivot.LookAt(towerTarget);
        }
        else
        {
            Vector3 direction = towerTarget.position - this.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 rotation = Quaternion.Lerp(pivot.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;
            pivot.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }
        //FIRING PART


        if (CountdownToNextFire <= 0f)
        {
            Shoot();
            CountdownToNextFire = 1f / rateOfFire;
        }
        if (tower_SO.towerType == ProjectTileType.LASER)
        {
            //ShootLaser();
            FireLaserBeam();
        }


        //CountdownToNextFire -= Time.deltaTime; // move to top of method so the countdown continues even if the tower has no target.
    }

    private void Shoot()
    {
        //if(this.GetComponent<Laser>() != null)
        //{
        //    Laser laser = this.GetComponent<Laser>();

        //    laser.FireLaserBeam();

        //    return;
        //}

        GameObject projectileGameObject = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        Projectile projectile = projectileGameObject.GetComponent<Projectile>();

        if (projectile is Missile)
        {
            projectile.GetComponent<Missile>().AssignTarget(towerTarget);
        }
        else if(projectile is Bullet)
        {
            projectile.GetComponent<Bullet>().AssignTarget(towerTarget);
        }
        else if (projectile is Laser)
        {
            projectile.GetComponent<Laser>().AssignTarget(towerTarget);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, range);
        Gizmos.DrawWireSphere(this.transform.position, innerRadius);        
    }

    public void FireLaserBeam()
    {
        //GRAPHICS PART
        _lineRendererComponent.enabled = true;

        _lineRendererComponent.SetPosition(0, this.GetProjectileSpawnPoint.position);
        _lineRendererComponent.SetPosition(1, this.GetTowerTarget.position);
        
        if (!this._vfxLaser.isPlaying)
        {
            this._vfxLaser.Play();
        }    
        this._lightEffect.enabled = true;
        Vector3 direction = this.GetProjectileSpawnPoint.position - this.GetTowerTarget.position;
        this._vfxLaser.transform.position = this.GetTowerTarget.position + direction.normalized;
        this._vfxLaser.transform.rotation = Quaternion.LookRotation(direction);


        //this._vfxCloud.transform.position = _tower.GetTowerTarget.position + direction.normalized;

        //_vfxLaser.Play();

        //_vfxLaser.transform.position = this.GetProjectileSpawnPoint.position;
    }

    public void StopLaser()
    {
        //_vfxLaser.Stop();

        if (tower_SO.towerType == ProjectTileType.LASER)
        {
            _lineRendererComponent.enabled = false;
            this._vfxLaser.Stop();
            this._lightEffect.enabled = false; 
        }

    }
}