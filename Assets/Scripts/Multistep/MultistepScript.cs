using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultistepScript : MonoBehaviour
{
    
    [SerializeField]
    bool useSprite = true;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float smoothingFactor = 0.0f;
    [SerializeField]
    private Texture2D sprite;

    private SpriteRenderer spriteRenderer;
    private Color[] colors;

    [SerializeField]
    private int beginSize = 4;

    [SerializeField]
    private int steps = 4;

    private int currentSize = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (useSprite)
        {
            //Get colors and size from the sprite we selected
            colors = sprite.GetPixels();
            currentSize = sprite.width;
        }
        else
        {
            //Create color array and fill with random colors
            colors = new Color[beginSize * beginSize];
            for (int y = 0; y < beginSize; y++)
            {
                for (int x = 0; x < beginSize; x++)
                {
                    colors[x + y * beginSize] = GetRandomColor();
                }
            }
            currentSize = beginSize;
        }

        //Execute the algorithm for # times
        for (int i = 0; i < steps; i++)
        {
            colors = GenerateStep(colors, currentSize, currentSize);
            currentSize *= 2;

            colors = SmoothStep(colors, currentSize, currentSize, smoothingFactor);
        }

        //colors = GenerateRoads(colors, currentSize, currentSize);
        //colors = SmoothRoads(colors, currentSize, currentSize);
        SetImageFromColor(colors, currentSize, currentSize);
    }

    public static bool RandomBool()
    {
        return UnityEngine.Random.value >= 0.5f;
    }


    public static Color[] GenerateStep(Color[] inputColors, int width, int height)
    {
        //Create an array that's twice as big
        Color[] outputColors = new Color[(height * 2) * (width * 2)];

        int newWidth = width * 2;
        //Loop through old image width and height
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //copy color from old array to new array
                Color color = inputColors[x + y * width];
                outputColors[(x * 2) + ((y * 2) * newWidth)] = color;

                //set neighbouring pixels in new array since the array is twice as big
                SetColor((x * 2) + 1, (y * 2), newWidth, outputColors, color);
                SetColor((x * 2), (y * 2) + 1, newWidth, outputColors, color);
                SetColor((x * 2) + 1, (y * 2) + 1, newWidth, outputColors, color);

                //Set random colors around the position to color
                SetColorAtArrayPositionIfRandomBool((x * 2) - 1 + (y * 2) * newWidth, outputColors, color);
                SetColorAtArrayPositionIfRandomBool((x * 2) + 1 + (y * 2) * newWidth, outputColors, color);
                SetColorAtArrayPositionIfRandomBool((x * 2) - 1 + ((y * 2) + 1) * newWidth, outputColors, color);
                SetColorAtArrayPositionIfRandomBool((x * 2) - 1 + ((y * 2) - 1) * newWidth, outputColors, color);
                SetColorAtArrayPositionIfRandomBool((x * 2) + ((y * 2) - 1) * newWidth, outputColors, color);
                SetColorAtArrayPositionIfRandomBool((x * 2) + ((y * 2) + 1) * newWidth, outputColors, color);
                SetColorAtArrayPositionIfRandomBool((x * 2) + 1 + ((y * 2) - 1) * newWidth, outputColors, color);
                SetColorAtArrayPositionIfRandomBool((x * 2) + 1 + ((y * 2) + 1) * newWidth, outputColors, color);
            }
        }

        return outputColors;
    }

    
    void SetImageFromColor(Color[] inputColors, int xSize, int ySize)
    {
        Texture2D tex = new Texture2D(xSize, ySize);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(inputColors);
        tex.Apply();

        spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, xSize, ySize), Vector2.one);
    }

    static void SetColorAtArrayPositionIfRandomBool(int position, Color[] array, Color color)
    {
        if (RandomBool() && position > 0 && position < array.Length) array[position] = color;
    }

    static void SetColor(int x, int y, int width, Color[] colorIn, Color color)
    {
        colorIn[x + y * width] = color;
    }

    Color GetRandomColor()
    {
        float rr = UnityEngine.Random.Range(0.0f, 1.0f);
        float gg = UnityEngine.Random.Range(0.0f, 1.0f);
        float bb = UnityEngine.Random.Range(0.0f, 1.0f);
        Color color = new Color(rr, gg, bb);
        return color == Color.black ? GetRandomColor() : color;
    }

    private static Color[] SmoothStep(Color[] inputColors, int width, int height, float smoothingFactor)
    {
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                try
                {
                    if (UnityEngine.Random.value > (1.0f - smoothingFactor))
                    {
                        //If the 2 pixels above and below are the same and the 2 pixels on the left and right are the same
                        if (inputColors[x - 1 + y * width] == inputColors[x + 1 + y * width] && inputColors[x + (y - 1) * width] == inputColors[x + (y + 1) * width])
                        {
                            if (RandomBool())
                            {
                                inputColors[x + y * width] = inputColors[x + 1 + y * width];
                            }
                            else
                            {
                                inputColors[x + y * width] = inputColors[x + (y + 1) * width];
                            }
                        }
                        else if (inputColors[x + (y + 1) * width] == inputColors[x + (y - 1) * width])
                        {
                            inputColors[x + y * width] = inputColors[x + (y + 1) * width];
                        }
                        else if (inputColors[x - 1 + y * width] == inputColors[x + 1 + y * width])
                        {
                            inputColors[x + y * width] = inputColors[x + 1 + y * width];
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
        return inputColors;
    }
    static Color[] SmoothRoads(Color[] inputColors, int width, int height)
    {
        Color[] outputColors = new Color[width * height];

        Array.Copy(inputColors, outputColors, width * height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                try
                {
                    Color color = inputColors[x + y * width];

                    if (inputColors[x - 1 + y * width] == inputColors[x + 1 + y * width] && inputColors[x + (y - 1) * width] == inputColors[x + (y + 1) * width])
                    {
                        if (RandomBool())
                        {
                            if (outputColors[x + 1 + y * width] == Color.black)
                                outputColors[x + y * width] = outputColors[x + 1 + y * width];
                        }
                        else
                        {
                            if (outputColors[x + (y + 1) * width] == Color.black)
                                outputColors[x + y * width] = outputColors[x + (y + 1) * width];
                        }
                    }
                    else if (inputColors[x + (y + 1) * width] == inputColors[x + (y - 1) * width])
                    {
                        if (outputColors[x + (y + 1) * width] == Color.black)
                            outputColors[x + y * width] = outputColors[x + (y + 1) * width];
                    }
                    else if (inputColors[x - 1 + y * width] == inputColors[x + 1 + y * width])
                    {
                        if (outputColors[x + 1 + y * width] == Color.black)
                            outputColors[x + y * width] = outputColors[x + 1 + y * width];
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        return outputColors;
    }
    static Color[] GenerateRoads(Color[] inputColors, int width, int height)
    {
        Color[] outputColors = new Color[width * height];

        Array.Copy(inputColors, outputColors, width * height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                try
                {
                    Color color = inputColors[x + y * width];
                    if (inputColors[x - 1 + y * width] != color) outputColors[x + y * width] = Color.black;
                    if (inputColors[x + 1 + y * width] != color) outputColors[x + y * width] = Color.black;
                    if (inputColors[x + (y - 1) * width] != color) outputColors[x + y * width] = Color.black;
                    if (inputColors[x + (y + 1) * width] != color) outputColors[x + y * width] = Color.black;
                    if (inputColors[x - 1 + (y - 1) * width] != color) outputColors[x + y * width] = Color.black;
                    if (inputColors[x + 1 + (y + 1) * width] != color) outputColors[x + y * width] = Color.black;
                    if (inputColors[x - 1 + (y + 1) * width] != color) outputColors[x + y * width] = Color.black;
                    if (inputColors[x + 1 + (y - 1) * width] != color) outputColors[x + y * width] = Color.black;
                }
                catch (Exception e)
                {

                }
            }
        }

        return outputColors;
    }
}
