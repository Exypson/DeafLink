using UnityEngine;

namespace DeafLink.Core;

public enum SoundCategory
{
    Monster,   
    ItemBreak, 
    Weapon,    
    Explosion  
}

public struct SoundEvent
{

    public Vector3 WorldPosition;

    public SoundCategory Category;

    public string IconName;

    public SoundEvent(Vector3 worldPosition, SoundCategory category, string iconName = "")
    {
        WorldPosition = worldPosition;
        Category = category;
        IconName = iconName;
    }
}
