using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class BakingAnimationsMachine : EditorWindow
{

    public enum BakingType
    {
        SingleAnimation,
        BlendingAnimation,
        MultipleAnimations
    }

    private GameObject _animatedObjectInstance;
    //private Texture2D _texturedAnimation;
    private Mesh _currentMesh;

 

    public void LaunchBaking(BakingType bakingType, AnimationClip animationClip, GameObject animatedObject, string name, string savePath)
    {
        Debug.Log("Name: " + name);


        _animatedObjectInstance = Instantiate<GameObject>(animatedObject);
        SkinnedMeshRenderer skinnedMeshRenderer = _animatedObjectInstance.GetComponentInChildren<SkinnedMeshRenderer>();
        Animator animator = null;



        if (animationClip != null) Debug.Log("Animation Clip Found !");
        else
        {
            throw new System.Exception("Animation Clip is Null");
        }

        if (_animatedObjectInstance.TryGetComponent<Animator>(out animator)) Debug.Log("Animator Found !");
        else
        {
            throw new System.Exception("No Animator was found");
        }


        if (skinnedMeshRenderer != null) Debug.Log("Skinned Mesh Renderer Found !");
        else
        {
            throw new System.Exception("No Skinned Mesh was found");
        }

        animator.applyRootMotion = false;
        UnBakedTextureAnimations unBakedTexture = new UnBakedTextureAnimations(bakingType, animationClip, skinnedMeshRenderer, animator, name, savePath);
        EditorCoroutineUtility.StartCoroutine(Generate_SingleTexturedAnimation(unBakedTexture), this);
    }



    private IEnumerator Generate_SingleTexturedAnimation(UnBakedTextureAnimations unBakedTexture)
    {
        if (unBakedTexture.VertexCount > 2000)
        {
            throw new System.Exception("The model have too much vertex to be used. (More than 2000) ");
        }

        // Assuming you have a path where you want to save the texture
        string fileName = unBakedTexture.Name;

        // Make sure the directory exists, create it if it doesn't
        System.IO.Directory.CreateDirectory(unBakedTexture.SavePath);


        _currentMesh = new Mesh();
        bool hasFinishedAnimationConvertion = false;


        int frameCount = Mathf.ClosestPowerOfTwo((int)(unBakedTexture.AnimClips[0].length * unBakedTexture.AnimClips[0].frameRate));
        Texture2D _texturedAnimation = new Texture2D(unBakedTexture.MapWidth, frameCount, TextureFormat.RGBAHalf, true);

        _texturedAnimation.name = string.Format($"{unBakedTexture.Name}.animMap");


        int currentAnimationFrame = 0;
        float currentAnimationTime = 0;
        float incrementValue = unBakedTexture.AnimClips[0].length / frameCount;





        // Launch the animation
        unBakedTexture.PlayAnimation();

        // Launch the baking function
        EditorCoroutineUtility.StartCoroutine(BakingTexture(unBakedTexture), this);

        yield return new WaitUntil(() => hasFinishedAnimationConvertion == true);


        _texturedAnimation.Apply();
        Debug.Log("File Name: " + unBakedTexture.Name);

        

        // Save the Texture2D as a PNG file
        byte[] bytes = _texturedAnimation.EncodeToPNG();

        AssetDatabase.CreateAsset(_texturedAnimation, Path.Combine(unBakedTexture.SavePath, fileName + ".asset"));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();
        unBakedTexture.Destroy();
        //DestroyImmediate(_animatedObjectInstance);


        IEnumerator BakingTexture(UnBakedTextureAnimations unBakedTexture)
        {
            unBakedTexture.SkinnedMesh.BakeMesh(_currentMesh);


            for (int i = 0; i < unBakedTexture.VertexCount; i++)
            {
                _texturedAnimation.SetPixel(i, currentAnimationFrame, new Color(_currentMesh.vertices[i].x, _currentMesh.vertices[i].y, _currentMesh.vertices[i].z));
            }


            Debug.Log("Frame: " + currentAnimationFrame + " Done");


            currentAnimationTime += incrementValue;
            currentAnimationFrame++;


            if (currentAnimationTime > unBakedTexture.AnimClips[0].length)
            {
                hasFinishedAnimationConvertion = true;
            }
            else
            {
                unBakedTexture.SampleAnimation(incrementValue);

                yield return new WaitForSeconds(incrementValue);

                EditorCoroutineUtility.StartCoroutine(BakingTexture(unBakedTexture), this);
            }

        }
    }

    
}
