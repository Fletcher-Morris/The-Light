using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Terain_Grass_Extract : MonoBehaviour
{
    public Texture2D m_terrainDataTex;
    public Texture2D m_newTex;
    public Terrain m_terrain;
    public Material m_material;

    public TextureChannel m_extractChannel;

    public int index = 0;

    public bool refresh = false;
    private void Update()
    {
        if (refresh) Extract();
    }

    private void Start()
    {
        Extract();
    }

    private void Extract()
    {
        if (m_terrain == null) TryGetComponent<Terrain>(out m_terrain);

        if (m_terrain == null) return;
        m_terrainDataTex = m_terrain.terrainData.GetAlphamapTexture(index);
        m_material = m_terrain.materialTemplate;

        m_newTex = new Texture2D(m_terrainDataTex.width, m_terrainDataTex.height);

        for(int y = 0; y < m_terrainDataTex.height; y++)
        {
            for (int x = 0; x < m_terrainDataTex.width; x++)
            {
                Color p = m_terrainDataTex.GetPixel(x, y);
                switch (m_extractChannel)
                {
                    case TextureChannel.r:
                        m_newTex.SetPixel(x, y, new Color(p.r, p.r, p.r, 1));
                        break;
                    case TextureChannel.g:
                        m_newTex.SetPixel(x, y, new Color(p.g, p.g, p.g, 1));
                        break;
                    case TextureChannel.b:
                        m_newTex.SetPixel(x, y, new Color(p.b, p.b, p.b, 1));
                        break;
                    case TextureChannel.a:
                        m_newTex.SetPixel(x, y, new Color(p.a, p.a, p.a, 1));
                        break;
                    default:
                        break;
                }
            }
        }

        m_newTex.Apply();

        m_material.SetTexture("_PlacementTexture", m_newTex);

        refresh = false;
    }
}

public enum TextureChannel
{
    r,g,b,a
}
