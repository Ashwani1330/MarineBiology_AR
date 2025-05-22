using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class OceanLayer
{
    public string name;
    public float depthOffSet;
    public VolumeProfile volumeProfile;
}

public class OceanDepthController : MonoBehaviour
{
    public Transform arCamera;
    public Volume postProcessVolume;
    public List<OceanLayer> oceanLayers;
    public TextMeshProUGUI textMeshProUGUI;

    private int _currentLayerIndex = 0;
    private Transform _waterSurface;
    private UnderWaterDetector _underWaterDetector;
    private bool _isTransitioning = false;

    private void Start()
    {
        _underWaterDetector = FindAnyObjectByType<UnderWaterDetector>();
        if (!_underWaterDetector)
            Debug.LogWarning("UnderWaterDetector not found in the scene!");

        if (oceanLayers.Count > 0)
            ApplyLayer(oceanLayers[_currentLayerIndex]);
    }

    public void ApplyLayer(OceanLayer layer)
    {
        if (postProcessVolume)
        {
            postProcessVolume.profile = layer.volumeProfile;
            postProcessVolume.weight = 1f;
        }

        if (_underWaterDetector)
            _underWaterDetector.SetCurrentLayer(layer);

        if (textMeshProUGUI)
            textMeshProUGUI.text = $"{layer.name} layer";
    }

    public void SetWaterSurface(Transform surface)
    {
        _waterSurface = surface;
        if (_underWaterDetector != null && _waterSurface != null)
            _underWaterDetector.waterHeight = _waterSurface.position.y;
    }

    public void GoDeeper()
    {
        if (_isTransitioning || _currentLayerIndex >= oceanLayers.Count - 1) return;
        StartCoroutine(Transition(_currentLayerIndex, _currentLayerIndex + 1));
        _currentLayerIndex++;
    }

    public void GoUpper()
    {
        if (_isTransitioning || _currentLayerIndex <= 0) return;
        StartCoroutine(Transition(_currentLayerIndex, _currentLayerIndex - 1));
        _currentLayerIndex--;
    }

    private IEnumerator Transition(int fromIndex, int toIndex)
    {
        _isTransitioning = true;

        OceanLayer fromLayer = oceanLayers[fromIndex];
        OceanLayer toLayer = oceanLayers[toIndex];

        float offset = toLayer.depthOffSet - fromLayer.depthOffSet;
        float targetY = arCamera.position.y + offset;
        Vector3 targetPosition = new Vector3(arCamera.position.x, targetY, arCamera.position.z);

        float t = 0f;
        float duration = 1f;
        Vector3 startPos = arCamera.position;

        Vector3 waterStart = _waterSurface ? _waterSurface.position : Vector3.zero;
        Vector3 waterTarget = new Vector3(waterStart.x, waterStart.y + offset, waterStart.z);

        while (t < 1f)
        {
            float smoothStep = Mathf.SmoothStep(0, 1, t);
            arCamera.position = Vector3.Lerp(startPos, targetPosition, smoothStep);

            if (_waterSurface)
            {
                _waterSurface.position = Vector3.Lerp(waterStart, waterTarget, smoothStep);
                if (_underWaterDetector)
                {
                    _underWaterDetector.waterHeight = _waterSurface.position.y;
                    _underWaterDetector.SetCurrentLayer(toLayer);
                }
            }

            t += Time.deltaTime / duration;
            yield return null;
        }

        arCamera.position = targetPosition;
        if (_waterSurface) _waterSurface.position = waterTarget;

        ApplyLayer(toLayer);
        _isTransitioning = false;
    }
}
