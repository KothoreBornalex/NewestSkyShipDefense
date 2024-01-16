using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectifClass;


public class MapManager : MonoBehaviour
{
  

    [System.Serializable]
    public struct LightStruct
    {
        public bool onFlame;
        public ObjectifEnum lightPlace;
        public Light light;
    }

    [System.Serializable]
    public struct CanonStruct
    {
        public bool hasShooted;
        public MMFeedbacks feedBack;
    }


    public static MapManager instance;


    [Space(30.0f), Header("Time Fields")]
    [SerializeField] private bool _isTimeSpeeding;
    private float _currentDeltaTime;
    private float _currentTimeSpeed;
    [SerializeField] private float _slowTimeSpeed;
    [SerializeField] private float _fastTimeSpeed;
    public bool IsTimeSpeeding { get => _isTimeSpeeding; set => _isTimeSpeeding = value; }



    [Space(30.0f), Header("SunLight Fields")]
    [SerializeField] private bool _isSunRotating;
    [SerializeField] private float _rotatingSpeed;
    [SerializeField] private Transform _directionalLightTransform;
    private Light _directionalLight;
    private float _angleValue;



    [Space(30.0f), Header("Player Ship Light Fields")]
    [SerializeField] private LightStruct[] _lights;
    [SerializeField] private Color fireLightColor;
    [SerializeField, Range(10.0f, 30.0f)] private float fireLighFlickerIntensity = 20.0f;
    private float _currentLightIntensity;
    float maxLightIntensity = 25.0f;
    float minLightIntensity = 0.0f;
    public LightStruct[] Lights { get => _lights; set => _lights = value; }



    [Space(30.0f), Header("Player Ship Canon Fields")]
    [SerializeField] private bool playerShipShooting;
    private float timeSincePlayerShipIsShooting;
    private float maxTimePlayerShipShooting = 3.1f;
    [SerializeField] private CanonStruct[] leftCanonRange;
    [SerializeField] private CanonStruct[] rightCanonRange;
    private float timeSinceLastShotVoley;
    private int currentRange;
    private bool firstRangeHasShooted;
    private bool secondRangeHasShooted;


    [Space(30.0f), Header("BackGround Fields")]
    [SerializeField] private bool _isBackGroundMoving;
    [SerializeField] private Transform _backgroundElementsParent;
    [SerializeField] private Vector3 _backgroundBoundingBox;
    [SerializeField] private Color _boundingBoxColor;
    private float _currentBackGroundMovingSpeed;
    [SerializeField, Range(0, 750)] private float _slowBackGroundMovingSpeed;
    [SerializeField, Range(0, 750)] private float _fastBackGroundMovingSpeed;

    [SerializeField] private Transform[] _backgroundElements;




    [Space(30.0f), Header("Boats Traffic Fields")]
    [SerializeField] private bool _isBoatTrafficMoving;
    [SerializeField] private Transform _boatTrafficElementsParent;
    [SerializeField] private Vector3 _boatTrafficBoundingBox;
    [SerializeField] private Color _boatTrafficBoundingBoxColor;
    private float _currentBoatTrafficMovingSpeed;
    [SerializeField, Range(0, 750)] private float _slowBoatTrafficMovingSpeed;
    [SerializeField, Range(0, 750)] private float _fastBoatTrafficMovingSpeed;

    [SerializeField] private Transform[] _boatTrafficBackgroundElements;




    [Space(30.0f), Header("Ship Fields")]
    [SerializeField, Range(0, 500)] private float _shipMovingSpeed;
    [SerializeField, Range(0, 20)] private float _shipRotatingSpeed;

    [SerializeField] private ShipPatrolStruct[] _shipsArray;


    [System.Serializable]
    public struct ShipPatrolStruct
    {
        public Transform _shipTransform;
        public bool _finishedPatrol;
        public bool _startPatrol;
        public int _currentPatrolPoint;
        public Vector3 velocity;

        [HideInInspector] public Transform _baseShipPoint;
        public Transform[] _startAnimationPoints;
        public Transform[] _endAnimationPoints;
    }



    #region Awake & Start Functions
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        InitializedAllCanons();
    }


    private void Start()
    {
        _directionalLight = _directionalLightTransform.GetComponent<Light>();

        for(int i = 0; i < _shipsArray.Length; i++)
        {
            _shipsArray[i]._baseShipPoint = Instantiate<GameObject>(new GameObject(), transform).transform;
            _shipsArray[i]._baseShipPoint.SetParent(transform);
            _shipsArray[i]._baseShipPoint.SetPositionAndRotation(_shipsArray[i]._shipTransform.position, _shipsArray[i]._shipTransform.rotation);
        }
        
    }
    #endregion


    // Update is called once per frame
    void Update()
    {
        HandlingLights();



        if (_isTimeSpeeding)
        {
            UpdatingShipFunction();

            _currentTimeSpeed = Mathf.MoveTowards(_currentTimeSpeed, _fastTimeSpeed, Time.deltaTime * 0.7F);
            _currentBackGroundMovingSpeed = Mathf.MoveTowards(_currentBackGroundMovingSpeed, _fastBackGroundMovingSpeed, Time.deltaTime * 3.0f);
            _currentBoatTrafficMovingSpeed = Mathf.MoveTowards(_currentBoatTrafficMovingSpeed, _fastBoatTrafficMovingSpeed, Time.deltaTime * 3.0f);
        }
        else
        {
            _currentTimeSpeed = Mathf.MoveTowards(_currentTimeSpeed, _slowTimeSpeed, Time.deltaTime * 5.0F);
            _currentBackGroundMovingSpeed = Mathf.MoveTowards(_currentBackGroundMovingSpeed, _slowBackGroundMovingSpeed, Time.deltaTime * 3.0f);
            _currentBoatTrafficMovingSpeed = Mathf.MoveTowards(_currentBoatTrafficMovingSpeed, _slowBoatTrafficMovingSpeed, Time.deltaTime * 3.0f);
        }

        _currentDeltaTime = Time.deltaTime * _currentTimeSpeed;


        if (_isBackGroundMoving)
        {
            UpdateMovingBackgroundFunction();
        }

        if (_isBoatTrafficMoving)
        {
            UpdateMovingTrafficFunction();
        }



        if(playerShipShooting)
        {
            CanonHandling();
            timeSincePlayerShipIsShooting += Time.deltaTime;

            if(timeSincePlayerShipIsShooting >= maxTimePlayerShipShooting)
            {
                timeSincePlayerShipIsShooting = 0;
                playerShipShooting = false;
                ResetAllCanons();
            }
        }
    }

    public IEnumerator StartTimeSpeeding()
    {
        yield return new WaitForSeconds(2.5f);
        IsTimeSpeeding = true;
    }


    #region Lights Functions
    void HandlingLights()
    {
        if (_isSunRotating)
        {
            _directionalLightTransform.Rotate(new Vector3(_rotatingSpeed * _currentDeltaTime, 0, 0));
        }

        _angleValue = Quaternion.Angle(Quaternion.LookRotation(Vector3.up), _directionalLightTransform.rotation);
        if (_angleValue >= 110)
        {
            _currentLightIntensity = Mathf.Lerp(_currentLightIntensity, minLightIntensity, Time.deltaTime);
        }
        else
        {
            _currentLightIntensity = Mathf.Lerp(_currentLightIntensity, maxLightIntensity, Time.deltaTime);
        }


        if (_angleValue >= 90)
        {
            _directionalLight.intensity = Mathf.Lerp(_directionalLight.intensity, 1, Time.deltaTime);

        }
        else
        {
            _directionalLight.intensity = Mathf.Lerp(_directionalLight.intensity, 0, Time.deltaTime);
        }

        foreach (LightStruct lightStruct in _lights)
        {
            if(!lightStruct.onFlame) lightStruct.light.intensity = _currentLightIntensity;
            else
            {
                // Simulating Fire lighting
                float randomness = 3.0f;
                float flickerPerSecond = 3.0f;
                float time = Time.deltaTime * (1 - Random.Range(-randomness, randomness)) * Mathf.PI;
                lightStruct.light.intensity = Mathf.Lerp(lightStruct.light.intensity, (maxLightIntensity * 0.75f) + Mathf.Sin(time * flickerPerSecond) * fireLighFlickerIntensity, Time.deltaTime * 2.0f);

                lightStruct.light.color = Vector4.Lerp(lightStruct.light.color, fireLightColor, Time.deltaTime);
            }
        }
    }
    #endregion

    #region Background & Boat Traffic Functions
    void UpdateMovingBackgroundFunction()
    {
        foreach (Transform t in _backgroundElements)
        {
            MovingBackgroundFunction(t);
        }
    }

    void UpdateMovingTrafficFunction()
    {
        foreach (Transform t in _boatTrafficBackgroundElements)
        {
            MovingTrafficFunction(t);
        }
    }

    private void MovingBackgroundFunction(Transform t)
    {
        t.Translate(-Vector3.forward * _currentDeltaTime * _currentBackGroundMovingSpeed, Space.World);


        if (t.localPosition.z <= -(_backgroundBoundingBox.z / 2))
        {
            t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, (_backgroundBoundingBox.z / 2));
        }
    }


    private void MovingTrafficFunction(Transform t)
    {
        //t.Translate(-Vector3.forward * _currentDeltaTime * _currentBoatTrafficMovingSpeed, Space.World);
        t.Translate(t.forward * _currentDeltaTime * _currentBoatTrafficMovingSpeed, Space.World);


        if (t.localPosition.z < -(_boatTrafficBoundingBox.z / 2))
        {
            t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, (_boatTrafficBoundingBox.z / 2));
        }else if(t.localPosition.z > (_boatTrafficBoundingBox.z / 2))
        {
            t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, -(_boatTrafficBoundingBox.z / 2));
        }
    }
    #endregion


    #region Enemy Ships Functions
    void UpdatingShipFunction()
    {

        for (int i = 0; i < _shipsArray.Length; i++)
        {

            if (!_shipsArray[i]._finishedPatrol)
            {
                if (_shipsArray[i]._startAnimationPoints.Length != 0 && _shipsArray[i]._startPatrol)
                {

                    Vector3 targetPosition = _shipsArray[i]._startAnimationPoints[_shipsArray[i]._currentPatrolPoint].transform.position;
                    Vector3 targetPositionRounded = new Vector3(Mathf.Round(targetPosition.x), Mathf.Round(targetPosition.y), Mathf.Round(targetPosition.z));

                    Vector3 shipPosition = _shipsArray[i]._shipTransform.position;
                    Vector3 shipPositionRounded = new Vector3(Mathf.Round(shipPosition.x), Mathf.Round(shipPosition.y), Mathf.Round(shipPosition.z));

                    Vector3 result = targetPositionRounded - shipPositionRounded;
                    if (result.magnitude < 10.0f)
                    {
                        if (_shipsArray[i]._currentPatrolPoint == _shipsArray[i]._startAnimationPoints.Length - 1)
                        {
                            _shipsArray[i]._startPatrol = false;
                            _shipsArray[i]._currentPatrolPoint = 0;
                            _shipsArray[i]._shipTransform.SetPositionAndRotation(_shipsArray[i]._endAnimationPoints[0].position, _shipsArray[i]._endAnimationPoints[0].rotation);
                        }
                        else
                        {
                            _shipsArray[i]._currentPatrolPoint++;
                        }

                    }

                    /*if (targetPositionRounded == shipPositionRounded)
                    {
                        if (_shipsArray[i]._currentPatrolPoint == _shipsArray[i]._startAnimationPoints.Length - 1)
                        {
                            _shipsArray[i]._startPatrol = false;
                            _shipsArray[i]._currentPatrolPoint = 0;
                            _shipsArray[i]._shipTransform.SetPositionAndRotation(_shipsArray[i]._endAnimationPoints[0].position, _shipsArray[i]._endAnimationPoints[0].rotation);
                        }
                        else
                        {
                            _shipsArray[i]._currentPatrolPoint++;
                        }

                    }*/

                    TranslateShipTransform(i, _shipsArray[i]._startAnimationPoints[_shipsArray[i]._currentPatrolPoint].transform);

                }


                if (_shipsArray[i]._endAnimationPoints.Length != 0 && !_shipsArray[i]._startPatrol)
                {

                    Vector3 targetPosition = _shipsArray[i]._baseShipPoint.position;
                    Vector3 targetPositionRounded = new Vector3(Mathf.Round(targetPosition.x), Mathf.Round(targetPosition.y), Mathf.Round(targetPosition.z));

                    Vector3 shipPosition = _shipsArray[i]._shipTransform.position;
                    Vector3 shipPositionRounded = new Vector3(Mathf.Round(shipPosition.x), Mathf.Round(shipPosition.y), Mathf.Round(shipPosition.z));


                    if (targetPositionRounded == shipPositionRounded)
                    {
                        _shipsArray[i]._finishedPatrol = true;
                    }

                    EndPatrol(_shipsArray[i]._shipTransform, _shipsArray[i]._baseShipPoint);
                }
            }
        }
    }

    public void ResetAllShips()
    {
        for(int i = 0; i < _shipsArray.Length; i++)
        {
            _shipsArray[i]._finishedPatrol = false;
            _shipsArray[i]._currentPatrolPoint = 0;
            _shipsArray[i]._startPatrol = true;
        }
    }


    public void TranslateShipTransform(int shipIndex, Transform target)
    {
        Transform ship = _shipsArray[shipIndex]._shipTransform;

        Vector3 relativePos = target.position - ship.position;

        Quaternion newRotation = Quaternion.LookRotation(relativePos, ship.up);
        
        ship.rotation = Quaternion.Lerp(ship.rotation, newRotation, Time.deltaTime * _shipRotatingSpeed);

        //ship.position = Vector3.Slerp(ship.position, target.position, Time.deltaTime * _shipMovingSpeed);
        ship.position = Vector3.SmoothDamp(ship.position, target.position, ref _shipsArray[shipIndex].velocity, Time.deltaTime * _shipMovingSpeed);
    }




    public void EndPatrol(Transform ship, Transform target)
    {

        Vector3 relativePos = target.position - ship.position;

        Quaternion newRotation = Quaternion.LookRotation(relativePos, ship.up);

        //ship.rotation = Quaternion.Lerp(ship.rotation, newRotation, Time.deltaTime * _shipRotatingSpeed);
        ship.rotation = Quaternion.Lerp(ship.rotation, target.rotation, Time.deltaTime * _shipRotatingSpeed);

        ship.position = Vector3.Lerp(ship.position, target.position, Time.deltaTime);
    }

    public bool AllShipsPatrolTurnEnded()
    {
        foreach (var ship in _shipsArray)
        {
            if (ship._finishedPatrol)
            {
                continue;
            }
            else
            {
                return false;
            }
            
        }

        return true;
    }

    #endregion

    #region Canons Functions

    public void CanonHandling()
    {
        #region GetCanonIndex Function
        int GetCanonIndex()
        {
            int newCanonIndex = 0;
            switch (currentRange)
            {
                case 0:
                    newCanonIndex = Random.Range(0, leftCanonRange.Length);

                    if (leftCanonRange[newCanonIndex].hasShooted)
                    {
                        return GetCanonIndex();
                    }
                    else
                    {
                        return newCanonIndex;
                    }

                case 1:
                    newCanonIndex = Random.Range(0, rightCanonRange.Length);

                    if (rightCanonRange[newCanonIndex].hasShooted)
                    {
                        return GetCanonIndex();
                    }
                    else
                    {
                        return newCanonIndex;
                    }

            }

            return 0;
        }
        #endregion

        // range = 0 signifie que c'est la rangée gauche.
        // range = 1 signifie que c'est la rangée droite.
        if (timeSinceLastShotVoley == 0) currentRange = Random.Range(0, 2);

        timeSinceLastShotVoley += Time.deltaTime;

        if (!firstRangeHasShooted)
        {
            if (timeSinceLastShotVoley < 0.5f) return;
            int canonIndex = 0;
            canonIndex = GetCanonIndex();

            switch (currentRange)
            {
                case 0:
                    leftCanonRange[canonIndex].feedBack?.PlayFeedbacks();
                    leftCanonRange[canonIndex].hasShooted = true;
                    break;

                case 1:
                    rightCanonRange[canonIndex].feedBack?.PlayFeedbacks();
                    rightCanonRange[canonIndex].hasShooted = true;
                    break;
            }

            firstRangeHasShooted = true;
        }
        
        if(!secondRangeHasShooted)
        {
            if (timeSinceLastShotVoley < 1.0f) return;

            if (currentRange == 0) currentRange = 1;
            else currentRange = 0;

            int canonIndex = 0;
            canonIndex = GetCanonIndex();

            

            switch (currentRange)
            {
                case 0:
                    leftCanonRange[canonIndex].feedBack?.PlayFeedbacks();
                    leftCanonRange[canonIndex].hasShooted = true;
                    break;

                case 1:
                    rightCanonRange[canonIndex].feedBack?.PlayFeedbacks();
                    rightCanonRange[canonIndex].hasShooted = true;
                    break;
            }

            secondRangeHasShooted = true;
        }


        if(timeSinceLastShotVoley >= 1.5f)
        {
            timeSinceLastShotVoley = 0.0f;
            firstRangeHasShooted = false;
            secondRangeHasShooted = false;
        }
    }


    public void ResetAllCanons()
    {
        for(int i = 0; i < leftCanonRange.Length; i++)
        {
            leftCanonRange[i].hasShooted = false;
            leftCanonRange[i].feedBack.Initialization();
        }

        for (int i = 0; i < rightCanonRange.Length; i++)
        {
            rightCanonRange[i].hasShooted = false;
            rightCanonRange[i].feedBack.Initialization();
        }

        timeSinceLastShotVoley = 0.0f;
        firstRangeHasShooted = false;
        secondRangeHasShooted = false;
    }

    public void InitializedAllCanons()
    {
        for (int i = 0; i < leftCanonRange.Length; i++)
        {
            leftCanonRange[i].feedBack.Initialization();
        }

        for (int i = 0; i < rightCanonRange.Length; i++)
        {
            rightCanonRange[i].feedBack.Initialization();
        }
    }

    public void StartCanonsVoleys()
    {
        playerShipShooting = true;
    }
    #endregion




    #region Gizmos

    void OnDrawGizmos()
    {
        Gizmos.color = _boundingBoxColor;
        Gizmos.DrawCube(_backgroundElementsParent.position, _backgroundBoundingBox);

        Gizmos.color = _boatTrafficBoundingBoxColor;
        Gizmos.DrawCube(_boatTrafficElementsParent.position, _boatTrafficBoundingBox);


        Gizmos.color = Color.green;
        for(int i = 0; i < _shipsArray.Length; i++)
        {

            if (_shipsArray[i]._startAnimationPoints.Length != 0)
            {

                for (int x = 0; x < _shipsArray[i]._startAnimationPoints.Length; x++)
                {
                    Gizmos.DrawSphere(_shipsArray[i]._startAnimationPoints[x].transform.position, 0.85f);
                }
            }


            if (_shipsArray[i]._endAnimationPoints.Length != 0)
            {
                Gizmos.DrawSphere(_shipsArray[i]._endAnimationPoints[0].transform.position, 0.85f);


                /*for (int x = 0; x < _shipsArray[i]._endAnimationPoints.Length; x++)
                {
                    if (x != _shipsArray[i]._endAnimationPoints.Length - 1)
                    {
                        Gizmos.DrawSphere(_shipsArray[i]._endAnimationPoints[x].transform.position, 0.85f);
                    }
                    else
                    {
                        Gizmos.DrawSphere(_shipsArray[i]._endAnimationPoints[x].transform.position, 0.85f);
                    }
                }*/
            }
        }
    }

    #endregion
}
