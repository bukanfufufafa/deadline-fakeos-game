using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WallpaperLoader : MonoBehaviour
{
    [SerializeField] private Image targetImage;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, StringBuilder pvParam, uint fWinIni);

    private const uint SPI_GETDESKWALLPAPER = 0x0073;

    private void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        LoadCurrentWallpaper();
    }

    public void LoadCurrentWallpaper()
    {
        string wallpaperPath = GetWindowsWallpaperPath();

        if (string.IsNullOrEmpty(wallpaperPath) || !File.Exists(wallpaperPath))
        {
            Debug.LogError($"[WallpaperLoader] Could not find desktop wallpaper at: {wallpaperPath}");
            return;
        }

        // Read image bytes and load into a Texture2D
        byte[] fileData = File.ReadAllBytes(wallpaperPath);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        
        if (texture.LoadImage(fileData)) // Auto-resizes texture dimensions to match image source
        {
            // Convert Texture2D into Sprite for UI Canvas Image
            Sprite wallpaperSprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            targetImage.sprite = wallpaperSprite;
            Debug.Log($"[WallpaperLoader] Loaded wallpaper successfully from {wallpaperPath}");
        }
        else
        {
            Debug.LogError("[WallpaperLoader] Failed to decode wallpaper image bytes.");
        }
    }

    private string GetWindowsWallpaperPath()
    {
        // 260 is standard MAX_PATH in Windows
        StringBuilder pathBuilder = new StringBuilder(260);

        // Retrieve current desktop wallpaper path via Win32 API
        if (SystemParametersInfo(SPI_GETDESKWALLPAPER, (uint)pathBuilder.Capacity, pathBuilder, 0))
        {
            return pathBuilder.ToString();
        }

        return string.Empty;
    }
}