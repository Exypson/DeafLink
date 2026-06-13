using System.Collections.Generic;
using DeafLink.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DeafLink.UI;

public class SoundIndicatorHUD : MonoBehaviour
{
    private static SoundIndicatorHUD? _instance;
    
    private const int   IconPoolSize         = 32;
    private const float MinorHearingDistance = 10f; // Monsters, ItemBreak
    private const float MajorHearingDistance = 20f; // Weapons, Explosions

    private SoundIcon[] _pool = null!;
    private readonly Queue<SoundEvent> _pendingEvents = new();

    private Canvas _canvas = null!;
    private RectTransform _canvasRect = null!;
    private CanvasScaler _scaler = null!;

    private void Awake()
    {
        _instance = this;
        BuildCanvas();
        BuildPool();
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    public static void RegisterSoundEvent(SoundEvent evt)
    {
        if (_instance == null) return;
        lock (_instance._pendingEvents)
        {
            _instance._pendingEvents.Enqueue(evt);
        }
    }

    private void Update()
    {
        lock (_pendingEvents)
        {
            while (_pendingEvents.Count > 0)
            {
                var evt = _pendingEvents.Dequeue();
                SpawnIcon(evt);
            }
        }
    }

    private void SpawnIcon(SoundEvent evt)
    {
        var player = PlayerAvatar.instance;
        var cam    = Camera.main;

        if (player == null || cam == null) return;

        var toSound = evt.WorldPosition - player.transform.position;
        
        float maxDistance = (evt.Category == SoundCategory.Weapon || evt.Category == SoundCategory.Explosion)
            ? MajorHearingDistance 
            : MinorHearingDistance;

        if (toSound.magnitude > maxDistance)
        {
            return;
        }

        if (evt.Category == SoundCategory.Monster && !string.IsNullOrEmpty(evt.IconName))
        {
            foreach (var a in _pool)
            {
                if (a.IsActive && a.IconName == evt.IconName)
                {
                    float distance = Vector3.Distance(a.WorldPos, evt.WorldPosition);
                    if (distance <= 3.0f)
                    {
                        a.Activate(evt.WorldPosition, evt.Category, evt.IconName);
                        return;
                    }
                }
            }
        }
        
        SoundIcon? icon = GetFreeIcon();
        if (icon == null)
        {
            return;
        }

        icon.Activate(evt.WorldPosition, evt.Category, evt.IconName);
    }

    private SoundIcon? GetFreeIcon()
    {
        foreach (var a in _pool)
            if (!a.IsActive) return a;
        return null;
    }

    private void BuildCanvas()
    {
        var canvasObj = new GameObject("DeafLink_Canvas");
        canvasObj.transform.SetParent(transform, false);

        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 100;

        _scaler = canvasObj.AddComponent<CanvasScaler>();
        _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _scaler.referenceResolution = new Vector2(1920f, 1080f);
        _scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        _canvasRect = canvasObj.GetComponent<RectTransform>();
    }

    private void BuildPool()
    {
        _pool = new SoundIcon[IconPoolSize];
        for (int i = 0; i < IconPoolSize; i++)
        {
            var IconObj = new GameObject($"DeafLink_Icon_{i}");
            IconObj.transform.SetParent(_canvas.transform, false);
            _pool[i] = IconObj.AddComponent<SoundIcon>();
        }
    }
}
