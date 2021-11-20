using System.Collections;
using System.Collections.Generic;
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
* --Changelog:
*	20.11.21	FM	created
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color[] UpdateColor(int width, int height)
    {
        Color[] colormap = new Color[width * height];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (terrainGenerator.AllNoise[width, height] >= waterLevel)
                {
                    //Set water
                    colormap[i * height + j] = Color.blue;
                }
                else if (terrainGenerator.AllNoise[width, height] >= grassLevel)
                {
                    //Set grass
                    colormap[i * height + j] = Color.green;
                }
                else if (terrainGenerator.AllNoise[width, height] >= mountainLevel)
                {
                    //Set mountain
                    colormap[i * height + j] = Color.grey;
                }
                else
                    //Set mountainTop
                    colormap[i * height + j] = Color.white;
            }
        }
        return colormap;
    }

    public Texture2D UpdateTexture(Color[] _colormap, int _width, int _height)
    {
        Texture2D newTexture = new Texture2D(_width, _height);
        newTexture.filterMode = FilterMode.Trilinear;
        newTexture.wrapMode = TextureWrapMode.Clamp;
        return newTexture;
    }
}
