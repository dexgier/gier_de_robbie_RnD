using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VoronoiDiagram : MonoBehaviour
{
    public Vector2Int imageDimensions;
    public int regionAmount;
    public bool drawByDistance = false;

    Color[] colorTest;

    public void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Sprite.Create((drawByDistance ? GetDiagramByDistance() : GetDiagram()), new Rect(0,0, imageDimensions.x, imageDimensions.y), Vector2.one * 0.5f);
        colorTest = new Color[imageDimensions.x * imageDimensions.y];
    }

    
    
    Texture2D GetDiagram()
    {
        Vector2Int[] centroids = new Vector2Int[regionAmount];
        Color[] regions = new Color[regionAmount];

        for (int i = 0; i < regionAmount; i++)
        {
            centroids[i] = new Vector2Int(UnityEngine.Random.Range(0, imageDimensions.x), UnityEngine.Random.Range(0, imageDimensions.y));
            regions[i] = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f,1f));

        }
        Color[] pixelColors = new Color[imageDimensions.x * imageDimensions.y];
        for (int x = 0; x < imageDimensions.x; x++)
        {
            for (int y = 0; y < imageDimensions.y; y++)
            {
                int index = x * imageDimensions.x + y;
                pixelColors[index] = regions[GetClosestGetCentroidIndex(new Vector2Int(x, y), centroids)];
            }
        }

        return GetImageFromColorArray(pixelColors);
    }

    Texture2D GetDiagramByDistance()
    {
        Vector2Int[] centroids = new Vector2Int[regionAmount];
        

        for (int i = 0; i < regionAmount; i++)
        {
            centroids[i] = new Vector2Int(UnityEngine.Random.Range(0, imageDimensions.x), UnityEngine.Random.Range(0, imageDimensions.y));
        }
        Color[] pixelColors = new Color[imageDimensions.x * imageDimensions.y];
        float[] distances = new float[imageDimensions.x * imageDimensions.y];

        for (int x = 0; x < imageDimensions.x; x++)
        {
            for (int y = 0; y < imageDimensions.y; y++)
            {
                int index = x * imageDimensions.x + y;
                distances[index] = Vector2.Distance(new Vector2(x,y), centroids[GetClosestGetCentroidIndex(new Vector2Int(x,y), centroids)]);
            }
        }
        float maxDist = GetMaxDistance(distances);

        for (int i = 0; i < distances.Length; i++)
        {
            float colorValue = distances[i] / maxDist;
            pixelColors[i] = new Color(colorValue, colorValue, colorValue, 1f);
        }

        Color lowestValue = getLowestColorValue(pixelColors);
        pixelColors = FillLowestValueWithColor(lowestValue, pixelColors);

        //Color[] tempColArray = ColorArrayLowToHigh(pixelColors);

        return GetImageFromColorArray(pixelColors);
    }



    float GetMaxDistance(float[] distances)
    {
        float maxDist = float.MinValue;
        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] > maxDist)
            {
                maxDist = distances[i];
            }
        }
        return maxDist;
    }

    Color getLowestColorValue(Color[] colorArray)
    {
        Color lowestColor = new Color(0, 0, 0, 1);

        for (int i = 0; i < colorArray.Length; i++)
        {
            if (colorArray[i].r > lowestColor.r)
            {
                lowestColor = colorArray[i];
            }
        }
        return lowestColor;
    }

    Color[] FillLowestValueWithColor(Color test, Color[] pixelColors)
    {
        for (int i = 0; i < pixelColors.Length; i++)
        {
            if(pixelColors[i].r == test.r)
            {
                pixelColors[i] = new Color(1f, 0, 0, 1f);
            }
        }
        return pixelColors;
    }

    int GetClosestGetCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
    {
        float smallestDist = float.MaxValue;
        int index = 0;
        for (int i = 0; i < centroids.Length; i++)
        {
            if(Vector2.Distance(pixelPos, centroids[i]) < smallestDist)
            {
                smallestDist = Vector2.Distance(pixelPos, centroids[i]);
                index = i;
            }
        }
        return index;
    }

    Texture2D GetImageFromColorArray(Color[] pixelColors)
    {
        Texture2D tex = new Texture2D(imageDimensions.x, imageDimensions.y);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(pixelColors);
        tex.Apply();
        return tex;
    } 
}
