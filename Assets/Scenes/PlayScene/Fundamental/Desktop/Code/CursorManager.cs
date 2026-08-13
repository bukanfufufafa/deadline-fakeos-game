using System.Collections.Generic;
using UnityEngine;

public enum Cursors
{
    Normal,
    ResizeHorizontal,
    ResizeVertical,
    ResizeNE,
    ResizeNW
}

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D resizeHorizontal;
    [SerializeField] Texture2D resizeVertical;
    [SerializeField] Texture2D resizeNE;
    [SerializeField] Texture2D resizeNW;

    private Dictionary<Cursors, CursorData> cursorsData;

    void Awake()
    {
        cursorsData = new()
        {
            { Cursors.Normal, new(null, new Vector2(0, 0)) },
            { Cursors.ResizeHorizontal, new(resizeHorizontal, new Vector2(10, 10)) },
            { Cursors.ResizeVertical, new(resizeVertical, new Vector2(10, 10)) },
            { Cursors.ResizeNE, new(resizeNE, new Vector2(10, 10)) },
            { Cursors.ResizeNW, new(resizeNW, new Vector2(10, 10)) },
        };
    }

    public void SetCursor(Cursors cursor)
    {
        CursorData data = cursorsData[cursor];
        Cursor.SetCursor(data.Texture, data.Hotspot, CursorMode.Auto);
    }

    private class CursorData
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Hotspot { get; private set; }

        public CursorData(Texture2D texture, Vector2 hotspot)
        {
            Texture = texture;
            Hotspot = hotspot;
        }
    }
}