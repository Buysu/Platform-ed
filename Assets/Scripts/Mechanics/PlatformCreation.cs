using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;

public class PlatformCreation : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private GameObject _placeholderPrefab;

    private GameObject _player;
    private GameObject _placeHolder;
    private List<GameObject> _platforms = new List<GameObject>();
    
    private float _maxDistance = 3;
    private float _minDistance = 0.75f;
    [SerializeField] private int _maxPlatforms = 3;

    private float _anglePlatform = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GameObject.Find("Player");
        _placeHolder = Instantiate(_placeholderPrefab);
    }
    // Update is called once per frame
    void Update()
    {    
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            _anglePlatform += 15f;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backward 
        {
            _anglePlatform -= 15f;
        }
        Vector3 playerInScreen = Camera.main.WorldToScreenPoint(_player.transform.position);
        Vector3 mouseInScreen = Input.mousePosition;
        Vector3 playerInWorld = _player.transform.position;
        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mouseInScreen);
        mouseInWorld += new Vector3(0, 0, Mathf.Abs(mouseInWorld.z) + playerInWorld.z);
        LogVector(playerInWorld);
        LogVector(mouseInWorld);

        var differences = playerInWorld - mouseInWorld;
        var distance = differences.magnitude > _maxDistance ? _maxDistance : differences.magnitude;
        distance = distance < _minDistance ? _minDistance : distance;
        var rotation = Quaternion.Euler(0, 0, _anglePlatform);
        var platformDistanceToPlayer = playerInWorld + ((mouseInWorld - playerInWorld).normalized * distance);
        _placeHolder.transform.SetPositionAndRotation(platformDistanceToPlayer, rotation);

        if (Input.GetMouseButtonDown(0))
        {
            var emptyObj = Instantiate(_prefab, playerInWorld + ((mouseInWorld - playerInWorld).normalized * distance), rotation);
            _platforms.Add(emptyObj);

            if (_platforms.Count > _maxPlatforms)
            {
                var firstObject = _platforms.First();
                _platforms.Remove(firstObject);
                Destroy(firstObject);
            }

        }
        else if (Input.GetMouseButtonDown(1))
        {
            foreach (var platform in (_platforms))
            {
                if (Vector3.Distance(platform.transform.position, mouseInWorld) <= 0.5f)
                {
                    _platforms.Remove(platform);
                    Destroy(platform);
                }
            }
        }
    }

    void LogVector(Vector3 v)
    {
        Debug.Log("Vector:   Z = " + v.z + "    Y = " + v.y + "      X = " + v.x);
    }

    float RoundAngle(float angle)
    {
        return Mathf.Round(angle / 15) * 15;
    }
}
