using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEngine;

#nullable enable


public class WindowManager : MonoBehaviour
{
    private List<WindowBase> _windowList = new();
    public IList<WindowBase> WindowList
    {
        get => _windowList.AsReadOnly();
    }

    public WindowBase? FocusedWindow { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool AddWindow(WindowBase window)
    {
        _windowList.Add(window);

        return true;
    }

    public bool RequestChangeWindowState(WindowBase window, WindowBaseState state)
    {
        if (!_windowList.Contains(window)) throw new Exception("Window belum ditambahkan ke WindowManager, tidak boleh di-show kan");

        return true;
    }

    public bool RequestFocusWindow(WindowBase window)
    {
        window.transform.SetSiblingIndex(0);
        FocusedWindow = window;

        return true;
    }

    public bool ReserveMaximizeWindow()
    {
        return true;
    }
}
