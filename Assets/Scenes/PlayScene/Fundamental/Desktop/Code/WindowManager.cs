using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#nullable enable


public class WindowManager : MonoBehaviour
{
    public static int CANVAS_WIDTH { get; private set; }
    public static int CANVAS_HEIGHT { get; private set; }

    [SerializeField] GameObject windowsGroup;

    CanvasScaler canvasScaler;
    CursorManager cursorManager;

    private List<MainWindow> _windowList = new();
    public IList<MainWindow> WindowList
    {
        get => _windowList.AsReadOnly();
    }

    private MainWindow? draggedWindow = null;

    void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        cursorManager = GetComponent<CursorManager>();

        CANVAS_WIDTH = (int)canvasScaler.referenceResolution.x;
        CANVAS_HEIGHT = (int)canvasScaler.referenceResolution.y;
    }

    void Start()
    {

    }

    // PUBLIC FUNCTIONS =====================================

    public bool AddWindow(MainWindow window)
    {
        _windowList.Add(window);

        window.transform.SetParent(windowsGroup.transform);
        window.transform.SetAsLastSibling();

        return true;
    }

    public void RemoveWindow(MainWindow window)
    {
        _windowList.Remove(window);
        if (draggedWindow == window) draggedWindow = null;
    }

    public bool RequestChangeWindowState(MainWindow window, MainWindowState state)
    {
        if (!_windowList.Contains(window)) throw new Exception("Window belum ditambahkan ke WindowManager, tidak boleh di-show kan");

        if (state == MainWindowState.Maximized)
        {
            window.transform.SetAsLastSibling();
        }

        return true;
    }

    public bool RequestFocusWindow(MainWindow window)
    {
        window.transform.SetAsLastSibling();

        return true;
    }

    public bool RequestStartDragWindow(MainWindow window)
    {
        if (draggedWindow != null) return false;

        draggedWindow = window;

        return true;
    }

    public bool RequestEndDragWindow(MainWindow window)
    {
        if (draggedWindow != window) return false;

        draggedWindow = null;

        return true;
    }

    public bool RequestSetCursor(MainWindow window, Cursors cursor)
    {
        if (draggedWindow != null) return false;
        
        cursorManager.SetCursor(cursor);

        return true;
    }
}
