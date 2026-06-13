using DeafLink.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DeafLink.UI;

public class SoundIcon : MonoBehaviour
{
    private const float DisplayDuration  = 1.5f;
    private const float FadeStartAt      = 0.8f;

    private float _timer;
    private bool _active;

    public Vector3 WorldPos => _worldPos;
    public string IconName { get; private set; } = string.Empty;

    private Vector3 _worldPos;
    private RectTransform _rectTransform = null!;
    private Image _iconImage = null!;
    private CanvasGroup _canvasGroup = null!;

    private static readonly Color[] CategoryColors = {
        Color.red,         // Monster
        Color.white,       // ItemBreak
        new Color(1f, 0.6f, 0f), // Weapon
        Color.yellow       // Explosion
    };

    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
        if (_rectTransform == null) _rectTransform = gameObject.AddComponent<RectTransform>();
        
        _rectTransform.sizeDelta = new Vector2(64f, 64f);
        _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        _rectTransform.pivot = new Vector2(0.5f, 0.5f);

        _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        var imgObj = new GameObject("IconImage");
        imgObj.transform.SetParent(transform, false);

        _iconImage = imgObj.AddComponent<Image>();
        _iconImage.preserveAspect = true;

        var imgRect = imgObj.GetComponent<RectTransform>();
        imgRect.sizeDelta = new Vector2(64f, 64f);

        gameObject.SetActive(false);
    }

    public void Activate(Vector3 soundWorldPos, SoundCategory category, string iconName)
    {
        _worldPos = soundWorldPos;
        IconName = iconName ?? string.Empty;
        _timer  = DisplayDuration;
        _active = true;

        if (!string.IsNullOrEmpty(iconName))
        {
            var sprite = IconManager.GetIcon(iconName);
            if (sprite != null)
            {
                _iconImage.sprite = sprite;
                _iconImage.color = Color.white; 
            }
            else
            {
                _iconImage.sprite = null;
                _iconImage.color = CategoryColors[(int)category];
            }
        }
        else
        {
            _iconImage.sprite = null;
            _iconImage.color = CategoryColors[(int)category];
        }

        _canvasGroup.alpha = 1f;
        UpdateScreenPosition();
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        _active = false;
        gameObject.SetActive(false);
    }

    public bool IsActive => _active;

    private void Update()
    {
        if (!_active) return;

        UpdateScreenPosition();

        _timer -= Time.deltaTime;

        if (_timer <= FadeStartAt)
        {
            _canvasGroup.alpha = Mathf.Clamp01(_timer / FadeStartAt);
        }

        if (_timer <= 0f)
        {
            Deactivate();
        }
    }

    private void UpdateScreenPosition()
    {
        var cam = Camera.main;
        var player = PlayerAvatar.instance;
        if (cam == null || player == null) return;

        var canvasRect = _rectTransform.parent as RectTransform;
        if (canvasRect == null) return;

        Vector3 toSound = _worldPos - player.transform.position;
        Vector3 toSoundFlat = Vector3.ProjectOnPlane(toSound, Vector3.up).normalized;
        Vector3 forwardFlat = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 rightFlat = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;

        float x = Vector3.Dot(toSoundFlat, rightFlat);  // Positive if right, Negative if left
        float y = Vector3.Dot(toSoundFlat, forwardFlat); // Positive if front, Negative if back
        
        Vector2 dir = new Vector2(x, y).normalized;
        if (dir.sqrMagnitude < 0.001f) dir = Vector2.up;

        float hw = canvasRect.rect.width / 2f;
        float hh = canvasRect.rect.height / 2f;
        float pad = 48f;
        float minX = -hw + pad;
        float maxX = hw - pad;
        float minY = -hh + pad;
        float maxY = hh - pad;

        float tX = float.MaxValue;
        if (dir.x > 0) tX = maxX / dir.x;
        else if (dir.x < 0) tX = minX / dir.x;

        float tY = float.MaxValue;
        if (dir.y > 0) tY = maxY / dir.y;
        else if (dir.y < 0) tY = minY / dir.y;

        float t = Mathf.Min(tX, tY);
        Vector2 clampedPos = dir * t;

        _rectTransform.anchoredPosition = clampedPos;
    }
}
