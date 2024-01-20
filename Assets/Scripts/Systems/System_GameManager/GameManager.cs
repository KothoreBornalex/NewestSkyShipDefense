using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static AI_Class;
using static UnityEngine.Rendering.DebugUI;
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

    private bool _isDefeated;
    public bool IsDefeated { get => _isDefeated; set => _isDefeated = value; }

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

    [Header("Spawns Fields")]
    [SerializeField] private FactionsEnum _leftBoatFaction;
    [SerializeField] private Transform[] _leftBoatSpawnPoints;
    [SerializeField] private FactionsEnum _rightBoatFaction;
    [SerializeField] private Transform[] _rightBoatSpawnPoints;

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
            AISpawner_Manager.instance.Spawn(FactionsEnum.Elf, SoldiersEnum.Infantry, objectif.transform.position);
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

    public void StartRandomFactionsBoat()
    {
        FactionsEnum newLeftBoatFaction = (FactionsEnum)Random.Range(0, (int)FactionsEnum.FactionsCount);

        FactionsEnum newRightBoatFaction = (FactionsEnum)Random.Range(0, (int)FactionsEnum.FactionsCount);

        SetNewFactions(newLeftBoatFaction, newRightBoatFaction);
    }

    public void SetNewFactions(FactionsEnum newLeftBoatFaction, FactionsEnum newRightBoatFaction)
    {
        // Check if the new factions are not the same as the current ones
        if (newLeftBoatFaction != _leftBoatFaction && newRightBoatFaction != _rightBoatFaction && newLeftBoatFaction != newRightBoatFaction)
        {
            // Set the new factions
            _leftBoatFaction = newLeftBoatFaction;
            _rightBoatFaction = newRightBoatFaction;

            // Optionally, you can perform additional actions here
            Debug.Log("New factions set successfully.");
        }
        else
        {
            StartRandomFactionsBoat();
        }
    }

    public void UpdateInWave()
    {

        if (_unitsSpawnedThisRound < _currentSpawnCount)
        {
            _currentSpawnerTimer += Time.deltaTime;

            if (_currentSpawnerTimer > _timeBetweenWave)
            {
                AISpawner_Manager.instance.Spawn(_leftBoatFaction, (SoldiersEnum)Random.Range(0, (int)SoldiersEnum.UnitsTypesCount), _leftBoatSpawnPoints[Random.Range(0, _leftBoatSpawnPoints.Length)].position);
                AISpawner_Manager.instance.Spawn(_rightBoatFaction, (SoldiersEnum)Random.Range(0, (int)SoldiersEnum.UnitsTypesCount), _rightBoatSpawnPoints[Random.Range(0, _rightBoatSpawnPoints.Length)].position);

                _unitsSpawnedThisRound++;
                _unitsSpawnedThisRound++;

                _currentSpawnerTimer = 0;
            }
        }
        else if(_unitsDeadThisRound == _unitsSpawnedThisRound)
        {
            EndInWave();
        }

        if (!_isDefeated)
        {
            bool allObjectifsDestroyed = true;
            foreach (ObjectifStats objectif in _objectifs)
            {
                if (objectif.ObjectifHealth._statCurrentValue > 0) allObjectifsDestroyed = false;
            }

            if (allObjectifsDestroyed)
            {
                StartDefeat();
                _isDefeated = true;
            }
        }
        
    }


    public void StartDefeat()
    {
        CheckHighscore();
    }
    private void CheckHighscore()
    {
        if (PlayerPrefs.HasKey("FirstHighscore"))
        {
            if (CurrentRound > PlayerPrefs.GetInt("FirstHighscore"))
            {
                PlayerPrefs.SetInt("FirstHighscore", CurrentRound);
                Debug.Log("New first highscore");
            }
            else if (PlayerPrefs.HasKey("SecondHighscore"))
            {
                if (CurrentRound > PlayerPrefs.GetInt("SecondHighscore"))
                {
                    PlayerPrefs.SetInt("SecondHighscore", CurrentRound);
                    Debug.Log("New second highscore");
                }
                else if (PlayerPrefs.HasKey("ThirdHighscore"))
                {
                    if (CurrentRound > PlayerPrefs.GetInt("ThirdHighscore"))
                    {
                        PlayerPrefs.SetInt("ThirdHighscore", CurrentRound);
                        Debug.Log("New third highscore");
                    }
                    else
                    {
                        Debug.Log("No new highscore");
                    }
                }
                else
                {
                    PlayerPrefs.SetInt("ThirdHighscore", CurrentRound);
                }
            }
            else
            {
                PlayerPrefs.SetInt("SecondHighscore", CurrentRound);
            }
        }
        else
        {
            PlayerPrefs.SetInt("FirstHighscore", CurrentRound);
        }
    }

    #endregion






    #region PostWave State
    private void StartPostWave()
    {

        MapManager.instance.StartCanonsVoleys();
        StartCoroutine(MapManager.instance.StartTimeSpeeding());

        StartRandomFactionsBoat();
        //SetUpBoats();
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
