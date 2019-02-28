﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSpawnManager : MonoBehaviour
{
    [SerializeField]
    private WaveSetup_SO[] _waveSetup;
    [SerializeField]
    private Text _waveCountDown;

    public Transform spawnPoint;

    public float timeBetweenWaves = 5f;
        
    private float _counterToNextWave;


    private int _waveIndex = 0;

    private bool _startNewWave = false;
    private bool _startCounter = false;
    
    void Start()
    {
        _startNewWave = true;        
    }

    // Update is called once per frame
    void Update()
    {
        if (_startNewWave)
        {
            StartCoroutine(WaveSpawner());
            
            _startNewWave = false;
            
        }
        if(_startCounter)
        {
            //_counterToNextWave += Time.deltaTime;
            _counterToNextWave -= Time.deltaTime;
            if(_counterToNextWave <= 0)
            {
                _startNewWave = true;
                _startCounter = false;
            }
        }

        _counterToNextWave = Mathf.Clamp(_counterToNextWave, 0f, Mathf.Infinity);

        _waveCountDown.text = string.Format("{0:00.00}", _counterToNextWave);

    }


    IEnumerator WaveSpawner()
    {        
        for (int i = 0; i < _waveSetup[_waveIndex].count; i++)
        {
            SpawnEnemy(_waveSetup[_waveIndex].enemy);
            yield return new WaitForSeconds(_waveSetup[_waveIndex].rate); //how long to spawn an enemy during the wave
        }
        
        _waveIndex++;

        if(_waveIndex == _waveSetup.Length)
        {
            //TODO: LEVEL FINISHED! GOTO NEXT LEVEL
            Debug.Log("Level Finished!");
            this.enabled = false;
        }

        //_counterToNextWave = 0f;
        _counterToNextWave = timeBetweenWaves;
        _startCounter = true;        
    }

    void SpawnEnemy(GameObject enemyToSpawn)
    {
        Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
    }

    
}