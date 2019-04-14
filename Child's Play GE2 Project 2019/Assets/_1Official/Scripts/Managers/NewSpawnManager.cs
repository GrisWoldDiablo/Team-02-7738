using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSpawnManager : MonoBehaviour
{
    #region Singleton
    private static NewSpawnManager instance = null;

    public static NewSpawnManager GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<NewSpawnManager>();
        }
        return instance;
    }
    #endregion

    [SerializeField] private float warmUpSeconds = 10.0f;
    private float warmuUpCounter;
    private bool warmedUp = false;
    [SerializeField]
    private WaveSetup_SO[] _waveSetup;

    public Transform spawnPoint;

    public float timeBetweenWaves = 5f;
        
    private float _counterToNextWave;

    private int _wavesLeft = 0;
    private int _currentWave = 1;


    private int _waveIndex = 0;
    private int _waveMixIndex = 0;
    private int _waveCounter = 0;

    private bool _startNewWave = false;
    private bool _startCounter = false;
    private int enemyLeftToSpawn = 0;

    public float WarmupCounter { get => (int)(warmuUpCounter - Time.time); }
    public int EnemyLeftToSpawn { get => enemyLeftToSpawn; }
    public int GetWavesLeft { get => _wavesLeft; }
    public int GetCurrentWave { get => _currentWave; }
    

    /// <summary>
    /// Called as soon as the GameObject is instanticated
    /// </summary>
    void Awake()
    {
        //_startNewWave = true;       
        warmuUpCounter = Time.time + warmUpSeconds;
        HudManager.GetInstance().ShowWarmUpText(true);

        _wavesLeft = _waveSetup.Length;

        GetEnemiesLeftToSpawn();

        //for (int i = 0; i < _waveSetup[_waveIndex].waveMixArray.Length; i++)
        //{
        //    for (int j = 0; j < _waveSetup[_waveIndex].waveMixArray[_waveCounter].count; j++)
        //    {
        //        enemyLeftToSpawn++;
        //    }
        //    //_waveMixIndex++;
        //    _waveCounter++;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (!warmedUp)
        {
            //Debug.Log($"Countdown warmup : {WarmupCounter}");
            HudManager.GetInstance().UpdateWarmUpText(WarmupCounter);
        }
        if (!warmedUp && warmuUpCounter <= Time.time)
        {
            warmedUp = true;
            _startNewWave = true;            
            HudManager.GetInstance().ShowWarmUpText(false);

            _wavesLeft--;
        }

        if (_startNewWave)
        {
            StartCoroutine(WaveSpawner());

            _startNewWave = false;

        }
        if (_startCounter)
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

        //_waveCountDown.text = string.Format("{0:00.00}", _counterToNextWave);

    }


    IEnumerator WaveSpawner()
    {
        //for (int i = 0; i < _waveSetup[_waveIndex].count; i++)
        //{
        //    SpawnEnemy(_waveSetup[_waveIndex].enemy);
        //    yield return new WaitForSeconds(_waveSetup[_waveIndex].rate); //how long to spawn an enemy during the wave
        //}

        //for (int i = 0; i < _waveSetup[_waveIndex].count; i++)
        //{
        //if (_waveSetup[_waveIndex].waveMixArray[_waveMixIndex] != null)
        //{
            for (int j = 0; j < _waveSetup[_waveIndex].waveMixArray[_waveMixIndex].count; j++)
            {
                SpawnEnemy(_waveSetup[_waveIndex].waveMixArray[_waveMixIndex].enemy);
                //_waveMixIndex++;
                enemyLeftToSpawn--;
                yield return new WaitForSeconds(_waveSetup[_waveIndex].rate); //how long to spawn an enemy during the wave
            }
        //}

        //if (_waveIndex == _waveSetup.Length)
        //{
        //    //LevelManager.CurrentLvl++;
        //    LevelManager.GetInstance().LevelCompleted();
        //    //TODO: LEVEL FINISHED!GOTO NEXT LEVEL
        //    Debug.Log("Level Spawning Completed!");
        //    this.enabled = false;
        //}

        _waveMixIndex++;
        //if (_waveMixIndex == _waveSetup[_waveIndex].waveMixArray.Length)
        //{
        //    _waveMixIndex = 0;

        //}
        //yield return new WaitForSeconds(_waveSetup[_waveIndex].rate); //how long to spawn an enemy during the wave
        //}

        if (_waveMixIndex < _waveSetup[_waveIndex].waveMixArray.Length)
            _startNewWave = true;

        if (_waveMixIndex == _waveSetup[_waveIndex].waveMixArray.Length)
        {
            _waveMixIndex = 0;
            _waveIndex++;
            //_currentWave = _waveIndex + 1;
            _wavesLeft--;
            if (_wavesLeft < 0)
            {
                _wavesLeft = 0;
            }
            _waveCounter = _waveMixIndex;
            //GetEnemiesLeftToSpawn();

            if (_waveIndex == _waveSetup.Length)
            {
                //LevelManager.CurrentLvl++;
                LevelManager.GetInstance().LevelCompleted();
                //TODO: LEVEL FINISHED! GOTO NEXT LEVEL
                Debug.Log("Level Spawning Completed!");
                this.enabled = false;
            }
            //else
            if (_waveIndex < _waveSetup.Length)
            {
                GetEnemiesLeftToSpawn();
                _currentWave = _waveIndex + 1;
            }
            //_currentWave = _waveIndex + 1;
            _counterToNextWave = timeBetweenWaves;
            _startCounter = true;
        }

        //_waveIndex++;

        //if(_waveIndex == _waveSetup.Length)
        //{
        //    //LevelManager.CurrentLvl++;
        //    LevelManager.GetInstance().LevelCompleted();
        //    //TODO: LEVEL FINISHED! GOTO NEXT LEVEL
        //    Debug.Log("Level Spawning Completed!");
        //    this.enabled = false;
        //}

        //_counterToNextWave = 0f;

        //_counterToNextWave = timeBetweenWaves;
        //_startCounter = true;        
    }

    void SpawnEnemy(GameObject enemyToSpawn)
    {
        Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
    }

    public void SkipWarmUp()
    {
        warmedUp = true;
        _startNewWave = true;
        HudManager.GetInstance().ShowWarmUpText(false);
        warmuUpCounter = 0;

        _wavesLeft--;
    }

    public void GetEnemiesLeftToSpawn()
    {
        for (int i = 0; i < _waveSetup[_waveIndex].waveMixArray.Length; i++)
        {
            for (int j = 0; j < _waveSetup[_waveIndex].waveMixArray[_waveCounter].count; j++)
            {
                enemyLeftToSpawn++;
            }
            //_waveMixIndex++;
            _waveCounter++;
        }
    }

    private void OnDestroy()
    {
        //Destroy the Singleton, so it can be recreated from new prefab Spawnpoint.
        instance = null;
    }
}
