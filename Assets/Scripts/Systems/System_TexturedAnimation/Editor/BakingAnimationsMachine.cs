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

 

    public void LaunchBaking(BakingType bakingType, AnimationClip animationClip, GameObject animatedObject, string name)
    {
        Debug.Log("Name: " + name);


        _animatedObjectInstance = Instantiate<GameObject>(animatedObject);
        Animator animator = null;
        SkinnedMeshRenderer skinnedMeshRenderer = null;


        if (_animatedObjectInstance.TryGetComponent<Animator>(out animator)) Debug.Log("Animator Found !");
        else
        {
            throw new System.Exception("No Animator was found");
        }


        skinnedMeshRenderer = _animatedObjectInstance.GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedMeshRenderer != null) Debug.Log("Skinned Mesh Renderer Found !");
        else
        {
            throw new System.Exception("No Skinned Mesh was found");
        }

        UnBakedTextureAnimations unBakedTexture = new UnBakedTextureAnimations(bakingType, animationClip, skinnedMeshRenderer, animator, name);
        EditorCoroutineUtility.StartCoroutine(Generate_SingleTexturedAnimation(unBakedTexture), this);
    }



    private IEnumerator Generate_SingleTexturedAnimation(UnBakedTextureAnimations unBakedTexture)
    {
        if (unBakedTexture.VertexCount > 2000)
        {
            throw new System.Exception("The model have too much vertex to be used. (More than 2000) ");
        }



        _currentMesh = new Mesh();
        bool hasFinishedAnimationConvertion = false;


        //int frameCount = (int)(unBakedTexture.AnimClips[0].length * unBakedTexture.AnimClips[0].frameRate);
        //Texture2D _texturedAnimation = new Texture2D(unBakedTexture.VertexCount, 64, TextureFormat.RGBAHalf, true);

        int frameCount = Mathf.ClosestPowerOfTwo((int)(unBakedTexture.AnimClips[0].length * unBakedTexture.AnimClips[0].frameRate));
        Texture2D _texturedAnimation = new Texture2D(unBakedTexture.MapWidth, frameCount, TextureFormat.RGBAHalf, true);

        _texturedAnimation.name = string.Format($"{unBakedTexture.Name}.animMap");


        int currentAnimationFrame = 0;
        float currentAnimationTime = 0;
        //float incrementValue = 1f / unBakedTexture.AnimClips[0].frameRate;
        //float incrementValue = unBakedTexture.AnimClips[0].length / unBakedTexture.AnimClips[0].frameRate;
        float incrementValue = unBakedTexture.AnimClips[0].length / frameCount;





        // Launch the animation
        unBakedTexture.PlayAnimation();


        // Launch the baking function
        EditorCoroutineUtility.StartCoroutine(BakingTexture(unBakedTexture), this);

        yield return new WaitUntil(() => hasFinishedAnimationConvertion == true);


        _texturedAnimation.Apply();




        Debug.Log("File Name: " + unBakedTexture.Name);

        // Assuming you have a path where you want to save the texture
        string savePath = "Assets/Resources/Textures/"; // Change this path to your desired location
        string fileName = unBakedTexture.Name;




        // Make sure the directory exists, create it if it doesn't
        System.IO.Directory.CreateDirectory(savePath);

        // Save the Texture2D as a PNG file
        byte[] bytes = _texturedAnimation.EncodeToPNG();

        //System.IO.File.WriteAllBytes(System.IO.Path.Combine(savePath, fileName), bytes);

        AssetDatabase.CreateAsset(_texturedAnimation, Path.Combine(savePath, fileName + ".asset"));



        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();
        unBakedTexture.Destroy();
        Destroy(_animatedObjectInstance);




        IEnumerator BakingTexture(UnBakedTextureAnimations unBakedTexture)
        {
            unBakedTexture.BakeMesh(ref _currentMesh);

            for (int i = 0; i < unBakedTexture.VertexCount; i++)
            {
                Vector3 currentVertex = _currentMesh.vertices[i];
                _texturedAnimation.SetPixel(i, currentAnimationFrame, new Color(currentVertex.x, currentVertex.y, currentVertex.z));
            }


            Debug.Log("Frame: " + currentAnimationFrame + " Done");


            currentAnimationTime += incrementValue;
            currentAnimationFrame++;


            /*if (currentAnimationFrame > frameCount)
            {
                hasFinishedAnimationConvertion = true;
            }
            else
            {
                unBakedTexture.SampleAnimation(incrementValue);

                yield return new WaitForSeconds(incrementValue);

                EditorCoroutineUtility.StartCoroutine(BakingTexture(unBakedTexture), this);
            }*/


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
