using System;
using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainWindow : MonoBehaviour, IPointerDownHandler
{
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

    private MainWindowState _state;
    public MainWindowState State
    {
        get => _state;
    }

    // Posisi X dan Y dari kiri-atas, bukan dari anchor (tengah).
    private int _x = int.MinValue;
    public int X
    {
        get => _x;
        set
        {
            _x = value;
            if (_state == MainWindowState.Floating) Reposition();
        }
    }
    private int _y = int.MinValue;
    public int Y
    {
        get => _y;
        set
        {
            _y = value;
            if (_state == MainWindowState.Floating) Reposition();
        }
    }

    private int _width = int.MinValue;
    public int Width
    {
        get => _width;
        set
        {
            _width = value;
            if (_state == MainWindowState.Floating) Reposition();
        }
    }
    private int _height = int.MinValue;
    public int Height
    {
        get => _height;
        set
        {
            _height = value;
            if (_state == MainWindowState.Floating) Reposition();
        }
    }

    public RectInt Rect
    {
        get => new RectInt(_x, _y, _width, _height);
    }

    void Awake()
    {
        windowManager = GameObject.FindWithTag("Main Canvas").GetComponent<WindowManager>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        var size = rectTransform.sizeDelta;
        if (_width == int.MinValue) _width = (int)size.x;
        if (_height == int.MinValue) _height = (int)size.y;

        var position = NormalizePosition();
        if (_x == int.MinValue) _x = (int)position.x;
        if (_y == int.MinValue) _y = (int)position.y;

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        maximizeButton.onClick.AddListener(() =>
        {
            if (_state == MainWindowState.Floating)
                Maximize();
            else if (_state == MainWindowState.Maximized)
                Unmaximize();
        });
        closeButton.onClick.AddListener(() =>
        {
            Close();
        });
    }

    void Start()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isOpened) return;

        windowManager.RequestFocusWindow(this);
    }

    // PUBLIC FUNCTIONS =====================================

    public void Open()
    {
        if (_isOpened) return;

        windowManager.AddWindow(this);

        Reposition();

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

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

        windowManager.RemoveWindow(this);

        LSequence.Create()
            .Append(LMotion.Create(Vector3.one, new Vector3(0.85f, 0.85f), 0.5f)
                .WithEase(Ease.OutCubic)
                .WithOnComplete(() =>
                {
                    Destroy(gameObject);
                })
                .BindToLocalScale(transform))
            .Join(LMotion.Create(1f, 0f, 0.4f)
                .WithEase(Ease.OutCubic)
                .BindToAlpha(canvasGroup))
            .Run();

        _isOpened = false;
    }

    public bool Maximize()
    {
        if (_state == MainWindowState.Maximized || !windowManager.RequestChangeWindowState(this, MainWindowState.Maximized))
            return false;

        _state = MainWindowState.Maximized;

        LMotion.Create(
                new Rect(_x, _y, _width, _height),
                new Rect(
                    0,
                    0,
                    WindowManager.CANVAS_WIDTH,
                    WindowManager.CANVAS_HEIGHT),
                0.6f)
            .WithEase(Ease.OutExpo)
            .Bind(rect =>
            {
                Reposition(rect.x, rect.y, rect.width, rect.height);
            })
            .AddTo(this);

        windowOverlay.color = new Color(255, 255, 255, 0);

        return true;
    }

    public bool Unmaximize(bool onlyState = false)
    {
        if (_state == MainWindowState.Floating || !windowManager.RequestChangeWindowState(this, MainWindowState.Floating))
            return false;

        _state = MainWindowState.Floating;

        if (!onlyState)
        {
            LMotion.Create(
                    new Rect(
                        0,
                        0,
                        WindowManager.CANVAS_WIDTH,
                        WindowManager.CANVAS_HEIGHT),
                    new Rect(_x, _y, _width, _height),
                0.6f)
                .WithEase(Ease.OutExpo)
                .Bind(rect =>
                {
                    Reposition(rect.x, rect.y, rect.width, rect.height);
                })
                .AddTo(this);
        }

        windowOverlay.color = new Color(255, 255, 255, 255);

        return true;
    }

    // PRIVATE FUNCTIONS =====================================

    private (float x, float y) NormalizePosition()
    {
        Vector2 position = rectTransform.anchoredPosition;
        return NormalizePosition(position.x, position.y);
    }
    private (float x, float y) NormalizePosition(float x, float y)
    {
        return (x - (_width / 2), -y - (_height / 2));
    }
    private (float x, float y) UnnormalizePosition(float x, float y, float width, float height)
    {
        return (x + (width / 2), -(y + (height / 2)));
    }

    private void Reposition()
    {
        Reposition(_x, _y, _width, _height);
    }
    private void Reposition(float x, float y, float width, float height)
    {
        rectTransform.sizeDelta = new Vector2(
            width,
            height
        );

        (float nx, float ny) = UnnormalizePosition(
            x,
            y,
            width,
            height
        );
        rectTransform.anchoredPosition = new Vector2(nx, ny);
    }
}
