using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DeafLink.Core;

public static class IconManager
{
    private static readonly Dictionary<string, Sprite> _iconCache = new();
    private static readonly string[] _manifestResourceNames;
    private static readonly Assembly _assembly;

    static IconManager()
    {
        _assembly = Assembly.GetExecutingAssembly();
        _manifestResourceNames = _assembly.GetManifestResourceNames();
    }

    public static Sprite? GetIcon(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return null;

        string cacheKey = fileName.ToLowerInvariant();
        if (_iconCache.TryGetValue(cacheKey, out var cachedSprite))
        {
            return cachedSprite;
        }

        string? resourceName = _manifestResourceNames.FirstOrDefault(r => 
            r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

        if (resourceName == null)
        {
            DeafLinkPlugin.Log.LogWarning($"[DeafLink] IconManager: Failed to find embedded resource for {fileName}");
            return null;
        }

        try
        {
            using Stream stream = _assembly.GetManifestResourceStream(resourceName);
            if (stream == null) return null;

            byte[] imageData = new byte[stream.Length];
            stream.Read(imageData, 0, (int)stream.Length);

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(imageData))
            {
                Sprite sprite = Sprite.Create(
                    texture, 
                    new Rect(0, 0, texture.width, texture.height), 
                    new Vector2(0.5f, 0.5f), 
                    100f, 
                    0, 
                    SpriteMeshType.FullRect
                );
                
                _iconCache[cacheKey] = sprite;
                return sprite;
            }
        }
        catch (Exception ex)
        {
            DeafLinkPlugin.Log.LogError($"[DeafLink] Failed to load icon {fileName}: {ex}");
        }

        return null;
    }
}
