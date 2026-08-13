using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainWindowEdge : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private const int HANDLE_WIDTH = 6;

    [SerializeField] RectTransform titlebar;

    WindowManager windowManager;
    MainWindow window;

    private enum DragMode
    {
        None,
        Titlebar,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
    }
    private DragMode dragMode = DragMode.None;
    private Vector2Int titlebarStartOffset;
    private enum DragAttempt
    {
        None,
        Candidate,
        Go
    }
    private DragAttempt dragAttempt;

    void Awake()
    {
        windowManager = GameObject.FindWithTag("Main Canvas").GetComponent<WindowManager>();
        window = GetComponent<MainWindow>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Down");

        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.pressPosition, eventData.pressEventCamera, out Vector3 mousePos);

        var windowRect = window.Rect;
        var offsetPos = NormalizeMousePosition(mousePos) - windowRect.position;

        // Atas
        if (offsetPos.y < HANDLE_WIDTH && window.State == MainWindowState.Floating)
        {
            if (offsetPos.x < HANDLE_WIDTH)
            {
                dragMode = DragMode.TopLeft;
            }
            else if (windowRect.width - offsetPos.x < HANDLE_WIDTH)
            {
                dragMode = DragMode.TopRight;
            }
            else
            {
                dragMode = DragMode.Top;
            }
        }
        // Titlebar
        else if (offsetPos.y <= titlebar.sizeDelta.y)
        {
            dragMode = DragMode.Titlebar;
        }
        // Bawah
        else if (windowRect.height - offsetPos.y < HANDLE_WIDTH && window.State == MainWindowState.Floating)
        {
            if (offsetPos.x < HANDLE_WIDTH)
            {
                dragMode = DragMode.BottomLeft;
            }
            else if (windowRect.width - offsetPos.x < HANDLE_WIDTH)
            {
                dragMode = DragMode.BottomRight;
            }
            else
            {
                dragMode = DragMode.Bottom;
            }
        }
        // Tengah
        else if (window.State == MainWindowState.Floating)
        {
            if (offsetPos.x < HANDLE_WIDTH)
            {
                dragMode = DragMode.Left;
            }
            else if (windowRect.width - offsetPos.x < HANDLE_WIDTH)
            {
                dragMode = DragMode.Right;
            }
        }

        Debug.Log($"Down DragMode {dragMode}");

        // Kalau masuk mode dragging.
        if (dragMode != DragMode.None)
        {
            // Coba request ke WindowManager apakah boleh ngedrag window, kalau gaboleh maka batalkan dragging.
            if (!windowManager.RequestStartDragWindow(window))
            {
                dragMode = DragMode.None;
            }
            // Kalau boleh, lanjut ke masa candidate sebelum terkonfirmasi bahwa window benar-benar didrag oleh OnBeginDrag.
            else
            {
                dragAttempt = DragAttempt.Candidate;
                DetermineCursor(windowRect, offsetPos);
            }

             Debug.Log($"Down DragAttempt {dragAttempt}");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"Up DragAttempt {dragAttempt}");
        
        // Batalkan dragging apabila masih masa candidate.
        if (dragAttempt == DragAttempt.Candidate)
        {
            dragMode = DragMode.None;
            dragAttempt = DragAttempt.None;
            windowManager.RequestEndDragWindow(window);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"BeginDrag DragMode {dragMode} DragAttempt {dragAttempt}");

        // Konfirmasi bahwa window benar-benar didrag.
        if (dragAttempt != DragAttempt.Candidate || eventData.pointerEnter == null) return;
        dragAttempt = DragAttempt.Go;

        // Kalau yang didragnya titlebar...
        if (dragMode == DragMode.Titlebar)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.pressPosition, eventData.pressEventCamera, out Vector3 mousePos);

            var nMousePos = NormalizeMousePosition(mousePos);

            // Suruh window unmaximize dulu kalau lagi dimaximize, sekalian ubah posisinya ngikutin posisi mouse.
            if (window.State == MainWindowState.Maximized)
            {
                // Tapi cek dulu apakah diperbolehkan untuk unmaximize atau tidak, apabila tidak maka batalkan dragging.
                if (!window.Unmaximize(true))
                {
                    dragMode = DragMode.None;
                    dragAttempt = DragAttempt.None;
                    return;
                }

                window.Y = nMousePos.y - 20;
                window.X = nMousePos.x - (window.Width / 2);
            }

            var windowRect = window.Rect;
            var offsetPos = nMousePos - windowRect.position;

            // Ambil posisi offset mouse.
            titlebarStartOffset = offsetPos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragAttempt != DragAttempt.Go) return;

        // Debug.Log($"Drag {dragMode}");

        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector3 mousePos);

        var windowRect = window.Rect;
        var nMousePos = NormalizeMousePosition(mousePos);
        var offsetPos = nMousePos - windowRect.position;

        switch (dragMode)
        {
            case DragMode.Titlebar:
                window.X = nMousePos.x - titlebarStartOffset.x;
                window.Y = nMousePos.y - titlebarStartOffset.y;
                break;
            case DragMode.TopLeft:
                window.X += offsetPos.x;
                window.Y += offsetPos.y;
                window.Width -= offsetPos.x;
                window.Height -= offsetPos.y;
                break;
            case DragMode.Top:
                window.Y += offsetPos.y;
                window.Height -= offsetPos.y;
                break;
            case DragMode.TopRight:
                window.Y += offsetPos.y;
                window.Width += offsetPos.x - windowRect.width;
                window.Height -= offsetPos.y;
                break;
            case DragMode.BottomLeft:
                window.X += offsetPos.x;
                window.Width -= offsetPos.x;
                window.Height += offsetPos.y - windowRect.height;
                break;
            case DragMode.Bottom:
                window.Height += offsetPos.y - windowRect.height;
                break;
            case DragMode.BottomRight:
                window.Width += offsetPos.x - windowRect.width;
                window.Height += offsetPos.y - windowRect.height;
                break;
            case DragMode.Left:
                window.X += offsetPos.x;
                window.Width -= offsetPos.x;
                break;
            case DragMode.Right:
                window.Width += offsetPos.x - windowRect.width;
                break;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragMode = DragMode.None;
        dragAttempt = DragAttempt.None;

        windowManager.RequestEndDragWindow(window);
        windowManager.RequestSetCursor(window, Cursors.Normal);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null || dragAttempt != DragAttempt.None || window.State != MainWindowState.Floating) return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector3 mousePos);

        var windowRect = window.Rect;
        var offsetPos = NormalizeMousePosition(mousePos) - windowRect.position;

        DetermineCursor(windowRect, offsetPos);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null || dragAttempt != DragAttempt.None || window.State != MainWindowState.Floating) return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector3 mousePos);

        var windowRect = window.Rect;
        var offsetPos = NormalizeMousePosition(mousePos) - windowRect.position;

        DetermineCursor(windowRect, offsetPos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (dragAttempt != DragAttempt.None) return;

        windowManager.RequestSetCursor(window, Cursors.Normal);
    }

    // PRIVATE FUNCTIONS =====================================

    // private RectInt NormalizeFrameRect()
    // {
    //     return new RectInt(
    //         (int)(rectTransform.anchoredPosition.x - (rectTransform.sizeDelta.x / 2)),
    //         (int)(-rectTransform.anchoredPosition.y - (rectTransform.sizeDelta.y / 2)),
    //         (int)rectTransform.sizeDelta.x,
    //         (int)rectTransform.sizeDelta.y
    //     );
    // }

    private Vector2Int NormalizeMousePosition(Vector3 mousePos)
    {
        return new Vector2Int(
            (int)(mousePos.x * WindowManager.CANVAS_WIDTH / Screen.width),
            (int)(-(mousePos.y * WindowManager.CANVAS_HEIGHT / Screen.height) + WindowManager.CANVAS_HEIGHT)
        );
    }

    private void DetermineCursor(RectInt windowRect, Vector2Int offsetPos)
    {
        if (offsetPos.y < HANDLE_WIDTH)
        {
            if (offsetPos.x < HANDLE_WIDTH)
            {
                windowManager.RequestSetCursor(window, Cursors.ResizeNW);
            }
            else if (windowRect.width - offsetPos.x < HANDLE_WIDTH)
            {
                windowManager.RequestSetCursor(window, Cursors.ResizeNE);
            }
            else
            {
                windowManager.RequestSetCursor(window, Cursors.ResizeVertical);
            }
        }
        // Bawah
        else if (windowRect.height - offsetPos.y < HANDLE_WIDTH)
        {
            if (offsetPos.x < HANDLE_WIDTH)
            {
                windowManager.RequestSetCursor(window, Cursors.ResizeNE);
            }
            else if (windowRect.width - offsetPos.x < HANDLE_WIDTH)
            {
                windowManager.RequestSetCursor(window, Cursors.ResizeNW);
            }
            else
            {
                windowManager.RequestSetCursor(window, Cursors.ResizeVertical);
            }
        }
        // Tengah
        else
        {
            if (offsetPos.x < HANDLE_WIDTH)
            {
                windowManager.RequestSetCursor(window, Cursors.ResizeHorizontal);
            }
            else if (windowRect.width - offsetPos.x < HANDLE_WIDTH)
            {
                windowManager.RequestSetCursor(window, Cursors.ResizeHorizontal);
            }
            else
            {
                windowManager.RequestSetCursor(window, Cursors.Normal);
            }
        }
    }
}
