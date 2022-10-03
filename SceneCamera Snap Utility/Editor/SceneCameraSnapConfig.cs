using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SceneCameraSnapConfig : ScriptableObject
{
    [HideInInspector] public List<CameraData> cameraData = new List<CameraData>();
}

[System.Serializable]
public class CameraData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}