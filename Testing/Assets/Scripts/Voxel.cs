using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public struct Voxel
{
    public enum Material {
        Empty,
        Ground,
        Rock,
        Water
    }

    public Material material;
    public int firstVertIndex;
    public Vector3[] verts;

    // TODO - this needs improving
    public Color32 color {
        get 
        {
            switch(material)
            {
                case Material.Ground:
                    return new Color32(0,255,0,255);
                case Material.Rock:
                    return new Color32(128,128,128,255);
                case Material.Water:
                    return new Color32(0,0,255,128);
                default:
                    break;
            }
            return new Color32(0,0,0,0);
        }
    }

    public bool IsOpaque() 
    {
        switch(material)
        {
            case Material.Ground:
            case Material.Rock:
                return true;
            default:
                break;
        }
        return false;
    }
}
