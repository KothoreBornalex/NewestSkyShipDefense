using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static AI_Class;
public class GameManager : MonoBehaviour
{

    #region Setup Instance
    public static GameManager instance;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 45;

        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else
        {
            Destroy(GameManager.instance);
        }
    }
    #endregion


    #region Game State Fields
    public enum GameState
    {
        PreWave,
        InWave,
        PostWave,
        Paused
    }
    [Header("Game State Fields")]
    [SerializeField] private GameState _currentGameState;
    [SerializeField] private ObjectifStats[] _objectifs;

    public GameState CurrentGameState { get => _currentGameState; set => _currentGameState = value; }
    public ObjectifStats[] Objectifs { get => _objectifs; set => _objectifs = value; }

    private bool _preWaveHasStarted;
    private bool _inWaveHasStarted;
    private bool _postWaveHasStarted;


    #endregion


    #region Spawn Fields
    [Header("Spawn Waves Fields")]
    [SerializeField, Expandable] private SO_AI_SpawnList _spawnData;
    private int _currentRound;
    private int _currentSpawnCount;
    private int _unitsSpawnedThisRound;
    private int _unitsDeadThisRound;
    private int _unitsDeadAllRounds;
    private float _postWaveTimer;

    [SerializeField] private bool _isCrashTesting;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _timeBetweenWave;
    private float _currentSpawnerTimer;

    public int CurrentRound { get => _currentRound;}
    public int UnitsDeadThisRound { get => _unitsDeadThisRound; set => _unitsDeadThisRound = value; }

    #endregion



    private void Update()
    {
        UpdateGameStates();
    }

    private void SpawnUnits()
    {
        foreach(ObjectifStats objectif in _objectifs)
        {
            AISpawner_Manager.instance.Spawn(FactionsEnum.Elf, SoldiersEnum.Larbin_A, objectif.transform.position);
        }
    }


    public int GetObjectif()
    {
        int index = 0;

        foreach (ObjectifStats objectif in _objectifs)
        {
            if (objectif.ObjectScript.GetObjectState() != IObjects.ObjectStates.Destroyed)
            {
                return index;
            }

            index++;
        }

        return 0;
    }


    public bool ObjectiveExist()
    {
        foreach (ObjectifStats objectif in _objectifs)
        {
            if(objectif.ObjectScript.GetObjectState() != IObjects.ObjectStates.Destroyed)
            {
                return true;
            }
        }

        return false;
    }

    public void UpdateGameStates()
    {
        switch(_currentGameState)
        {
            case GameState.PreWave:
                UpdatePreWave();
                break;

            case GameState.InWave:
                if (!_inWaveHasStarted) StartInWave();
                UpdateInWave();
                break;

            case GameState.PostWave:
                if (!_postWaveHasStarted) StartPostWave();
                UpdatePostWave();
                break;

            default: 
                break;
        }
    }

    #region PreWave State

    private void UpdatePreWave()
    {
        EndPreWave();
    }

    private void EndPreWave()
    {
        _currentGameState = GameState.InWave;
    }

    #endregion






    #region InWave State
    private void StartInWave()
    {
         if (_isCrashTesting)
         {
             _currentSpawnCount = _spawnData.CrashTestSpawnCount;
         }
         else
         {
             if (_currentRound == 0)
             {
                 _currentSpawnCount = _spawnData.BaseSpawnCount;
             }
             else
             {
                 _currentSpawnCount = _spawnData.BaseSpawnCount * (_currentRound * (_spawnData.BaseSpawnCount / 2));
             }
         }


        _currentRound++;
        _inWaveHasStarted = true;
    }

    private void EndInWave()
    {
        _currentGameState = GameState.PostWave;
        _inWaveHasStarted = false;
        _unitsSpawnedThisRound = 0;
        _unitsDeadThisRound = 0;
    }
    public void UpdateInWave()
    {

        if (_unitsSpawnedThisRound < _currentSpawnCount)
        {
            _currentSpawnerTimer += Time.deltaTime;

            if (_currentSpawnerTimer > _timeBetweenWave)
            {
                AISpawner_Manager.instance.Spawn(FactionsEnum.Elf, SoldiersEnum.Larbin_A, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position);

                _unitsSpawnedThisRound++;
                _currentSpawnerTimer = 0;
            }
        }
        else if(_unitsDeadThisRound == _unitsSpawnedThisRound)
        {
            EndInWave();
        }

    }

    #endregion






    #region PostWave State
    private void StartPostWave()
    {
        MapManager.instance.IsTimeSpeeding = true;


        _postWaveHasStarted = true;
    }

    private void UpdatePostWave()
    {
        _postWaveTimer += Time.deltaTime;

        if(MapManager.instance.AllShipsPatrolTurnEnded())
        {
            MapManager.instance.IsTimeSpeeding = false;
            MapManager.instance.ResetAllShips();

            EndPostWave();
        }
    }
    private void TogglePostWaveTimer(bool activated)
    {
        if(activated)
        {

        }
        else
        {

        }
    }
    private void EndPostWave()
    {
        _postWaveTimer = 0.0f;
        _currentGameState = GameState.PreWave;

        _postWaveHasStarted = false;
    }

    #endregion
}
