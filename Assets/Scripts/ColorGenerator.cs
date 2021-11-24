using UnityEngine;
/*****************************************************************************
* Project: TerrainEditor
* File   : ColorGenerator.cs
* Date   : 20.11.2021
* Author : Franz Mörike (FM)
*
* These coded instructions, statements, and computer programs contain
* proprietary information of the author and are protected by Federal
* copyright law. They may not be disclosed to third parties or copied
* or duplicated in any form, in whole or in part, without the prior
* written consent of the author.
*
* ChangeLog
* ----------------------------:
*	20.11.21	FM	created
*	21.11.21    FM  added comments
******************************************************************************/
public class ColorGenerator : MonoBehaviour
{
    [SerializeField]
    TerrainGenerator terrainGenerator;
    [SerializeField]
    private float waterLevel;
    [SerializeField]
    private float grassLevel;
    [SerializeField]
    private float mountainLevel;

    /// <summary>
    /// Creates a colormap according to heights of the different levels
    /// -> Texture will be drawn depending on the height at an index
    /// </summary>
    /// <param name="_width">width of the desired colormap</param>
    /// <param name="_height">height  of the desired colormap</param>
    /// <returns>new colormap</returns>
    public Color[] UpdateColor(int _width, int _height)
    {
        Color[] colormap = new Color[_width * _height];
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                if (terrainGenerator.AllNoise[j, i] <= waterLevel)
                {
                    //Set water
                    Debug.Log("Setting color blue");
                    colormap[i * _height + j] = Color.blue;
                }
                else if (terrainGenerator.AllNoise[j, i] <= grassLevel)
                {
                    //Set grass
                    Debug.Log("Setting color green");
                    colormap[i * _height + j] = Color.green;
                }
                else if (terrainGenerator.AllNoise[j, i] <= mountainLevel)
                {
                    //Set mountain
                    Debug.Log("Setting color gray");
                    colormap[i * _height + j] = Color.grey;
                }
                else
                {
                    //Set mountainTop
                    Debug.Log("Setting color white");
                    colormap[i * _height + j] = Color.white;
                }
                //Debug.Log($"Terrain Noise: {terrainGenerator.AllNoise[j, i]}");
                //Debug.Log($"Colormap colors {colormap[i * _height + j]}: {colormap[i * _height + j]}");
            }
        }
        return colormap;
    }

    /// <summary>
    /// Creates a new texture of a given colormap
    /// </summary>
    /// <param name="_colormap">the colormap, calculated in UpdateColor()</param>
    /// <param name="_width">width of colormap</param>
    /// <param name="_height">height of colormap</param>
    /// <returns>texture to apply on plane</returns>
    public Texture2D UpdateTexture(Color[] _colormap, int _width, int _height)
    {
        Texture2D newTexture = new Texture2D(_width, _height);
        newTexture.filterMode = FilterMode.Bilinear;
        newTexture.wrapMode = TextureWrapMode.MirrorOnce;
        newTexture.SetPixels(_colormap);
        newTexture.Apply();
        return newTexture;
    }
}
