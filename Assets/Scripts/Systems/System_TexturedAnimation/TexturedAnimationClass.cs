using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturedAnimationClass
{
    public string Name;
    public Texture2D texturedAnimation;

    public void InitializedTexturedAnimation(int vertexCount, int frameCount)
    {
        // ! \\
        // VertexCount is the width of the texture.
        // FrameCount is the height of the texture.
        // ! \\

        texturedAnimation = new Texture2D(vertexCount, frameCount, TextureFormat.RGBAHalf, false);
    }
}
