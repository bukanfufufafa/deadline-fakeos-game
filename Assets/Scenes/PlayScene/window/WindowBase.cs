using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WindowBase : MonoBehaviour
{
    public const int SCREEN_WIDTH = 1600;
    public const int SCREEN_HEIGHT = 900;
    public readonly Vector4 OverlayMargin = new(45, 28, 45, 62);

    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            titlebarText.text = value;
        }
    }

    [SerializeField] TextMeshProUGUI titlebarText;
    [SerializeField] Button closeButton;
    [SerializeField] Button maximizeButton;
    [SerializeField] Button minimizeButton;
    [SerializeField] Image windowOverlay;

    WindowManager windowManager;
    RectTransform rectTransform;
    CanvasGroup canvasGroup;


    private bool _isOpened;
    public bool IsOpened
    {
        get => _isOpened;
    }

    private WindowBaseState _state;
    public WindowBaseState State
    {
        get => _state;
    }

    // Posisi X dan Y dari kiri-atas, bukan dari anchor (tengah).
    private int _x;
    public int X
    {
        get => _x;
        set
        {
            _x = value;
            if (_state == WindowBaseState.Floating) Reposition();
        }
    }
    private int _y;
    public int Y
    {
        get => _y;
        set
        {
            _y = value;
            if (_state == WindowBaseState.Floating) Reposition();
        }
    }

    private int _width;
    public int Width
    {
        get => _width;
        set
        {
            _width = value;
            if (_state == WindowBaseState.Floating) Reposition();
        }
    }
    private int _height;
    public int Height
    {
        get => _height;
        set
        {
            _height = value;
            if (_state == WindowBaseState.Floating) Reposition();
        }
    }

    // PUBLIC FUNCTIONS =====================================

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        var size = rectTransform.sizeDelta;
        _width = (int)size.x; _height = (int)size.y;

        // Ambil dan ubah posisi dari anchor (tengah) ke kiri-atas.
        var position = rectTransform.anchoredPosition;
        (float x, float y) = NormalizePosition(position.x, position.y);
        _x = (int)x; _y = (int)y;

        Debug.Log($"Position: x {_x} y {_y}");

        maximizeButton.onClick.AddListener(() =>
        {
            if (_state == WindowBaseState.Floating)
                Maximize();
            else if (_state == WindowBaseState.Maximized)
                Unmaximize();
        });
        closeButton.onClick.AddListener(() =>
        {
            Close();
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        windowManager = GameObject.Find("Main Canvas").GetComponent<WindowManager>();

        Open();
    }

    public void Open()
    {
        if (_isOpened) return;

        windowManager.AddWindow(this);

        LSequence.Create()
            .Append(LMotion.Create(new Vector3(0.85f, 0.85f), Vector3.one, 0.6f)
                .WithEase(Ease.OutCubic)
                .BindToLocalScale(transform))
            .Join(LMotion.Create(0f, 1f, 0.6f)
                .WithEase(Ease.Linear)
                .BindToAlpha(canvasGroup))
            .Run();

        _isOpened = true;
    }

    public void Close()
    {
        if (!_isOpened) return;

        LSequence.Create()
            .Append(LMotion.Create(Vector3.one, new Vector3(0.85f, 0.85f), 0.4f)
                .WithEase(Ease.OutCubic)
                .BindToLocalScale(transform))
            .Join(LMotion.Create(1f, 0f, 0.4f)
                .WithEase(Ease.OutCubic)
                .BindToAlpha(canvasGroup))
            .Run();

        _isOpened = false;
    }

    public bool Maximize()
    {
        if (_state == WindowBaseState.Maximized || !windowManager.RequestChangeWindowState(this, WindowBaseState.Maximized))
            return false;

        _state = WindowBaseState.Maximized;

        LMotion.Create(
                new Rect(_x, _y, _width, _height),
                new Rect(
                    0 - OverlayMargin.x,
                    0 - OverlayMargin.y,
                    SCREEN_WIDTH + OverlayMargin.x + OverlayMargin.z,
                    SCREEN_HEIGHT + OverlayMargin.y + OverlayMargin.w),
                0.6f)
            .WithEase(Ease.OutExpo)
            .Bind(rect =>
            {
                Debug.Log($"Maximize iterate: {rect}");
                Reposition(rect.x, rect.y, rect.width, rect.height);
            })
            .AddTo(this);

        windowOverlay.color = new Color(255, 255, 255, 0);

        return true;
    }

    public bool Unmaximize()
    {
        if (_state == WindowBaseState.Floating || !windowManager.RequestChangeWindowState(this, WindowBaseState.Floating))
            return false;

        _state = WindowBaseState.Floating;

        LMotion.Create(
                new Rect(
                    0 - OverlayMargin.x,
                    0 - OverlayMargin.y,
                    SCREEN_WIDTH + OverlayMargin.x + OverlayMargin.z,
                    SCREEN_HEIGHT + OverlayMargin.y + OverlayMargin.w),
                new Rect(_x, _y, _width, _height),
            0.6f)
            .WithEase(Ease.OutExpo)
            .Bind(rect =>
            {
                Reposition(rect.x, rect.y, rect.width, rect.height);
            })
            .AddTo(this);

        windowOverlay.color = new Color(255, 255, 255, 255);

        return true;
    }

    // PRIVATE FUNCTIONS =====================================

    private (float x, float y) NormalizePosition()
    {
        return NormalizePosition(_x, _y);
    }
    private (float x, float y) NormalizePosition(float x, float y)
    {   
        return (x - (_width / 2), -y - (_height / 2));
    }
    private (float x, float y) UnormalizePosition(float x, float y, float width, float height)
    {
        return (x + (width / 2), -(y + (height / 2)));
    }

    private void Reposition()
    {
        rectTransform.sizeDelta = new Vector2(_width, _height);
        rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, _x);
        rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, _y);
    }
    private void Reposition(float x, float y, float width, float height)
    {
        rectTransform.sizeDelta = new Vector2(width, height);
        // rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x, width);
        // rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, x, height);

        (float nx, float ny) = UnormalizePosition(x, y, width, height);
        rectTransform.anchoredPosition = new Vector2(nx, ny);
    }
}
