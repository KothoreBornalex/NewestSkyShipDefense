using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Playables;
using static BakingAnimationsMachine;
using System;

public class TexturedAnimationEditorWindow : EditorWindow
{
    

    private static BakingAnimationsMachine _bakingAnimationsMachine;

    private string _basePath = "";
    private string _folderName = "";
    private string _savePath = "";

    private BakingType _bakingType;
    private GameObject AnimatedObject;
    private AnimationClip SingleAnimation;
    private string _animationName;

    




    [MenuItem("Tools/Textured Animation Creator")]
    public static void CreateEditorWindow()
    {
        EditorWindow window = GetWindow<TexturedAnimationEditorWindow>();
        window.titleContent = new GUIContent("Textured Animation Editor");

        _bakingAnimationsMachine = new BakingAnimationsMachine();
    }


    private void OnGUI()
    {
        HandleRegenerateWindowButton();
        HandleSavePathButton();

        HandleBakingTypeChoice();


        switch (_bakingType)
        {
            case BakingType.SingleAnimation: 
                HandleSingleAnimation(); 
                break;

            case BakingType.BlendingAnimation:

                break;

            case BakingType.MultipleAnimations:

                break;
        }

        EditorGUILayout.Space(20);




        EditorGUILayout.Space(20);
        GUI.enabled = SingleAnimation != null && AnimatedObject != null;
        if (GUILayout.Button("Generate Textured Animation"))
        {
            switch (_bakingType)
            {
                case BakingType.SingleAnimation:
                    _bakingAnimationsMachine.LaunchBaking(_bakingType, SingleAnimation, AnimatedObject, _animationName, _savePath);
                    break;

                case BakingType.BlendingAnimation:
                    break;

                case BakingType.MultipleAnimations:
                    break;
            }


            Debug.Log("Debug Test: " + _basePath + _animationName);
        }

        GUI.enabled = true;
        if (GUILayout.Button("Clear progress bar"))
        {
            EditorUtility.ClearProgressBar();
        }

    }

    private void HandleRegenerateWindowButton()
    {
        EditorGUILayout.Space(20);
        if (GUILayout.Button("Regenerate Window"))
        {
            _bakingAnimationsMachine = new BakingAnimationsMachine();
        }
        EditorGUILayout.Space(20);
    }

    private string OpenExplorer(string basePath)
    {
        string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", basePath, "");
        string result = "";

        // Find the index of "Assets"
        int assetsIndex = selectedPath.IndexOf("Assets");

        // Check if "Assets" is found in the string
        if (assetsIndex != -1)
        {
            // Extract the substring starting from "Assets"
            result = selectedPath.Substring(assetsIndex);
        }
        else
        {
            // Handle the case where "Assets" is not found
            Debug.Log("Error: 'Assets' not found in the input string.");
        }

        return result;
    }







    private void HandleSavePathButton()
    {
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Select Save Path"))
        {
            Debug.Log("Is Working");
            _savePath = OpenExplorer(_basePath);
        }

        /*
        _folderName = EditorGUILayout.TextField("Save Folder Name:", _folderName);

        if(string.IsNullOrEmpty(_folderName))
        {
            _savePath = EditorGUILayout.TextField("Save Path:", _savePath);
        }
        else
        {
            _savePath = EditorGUILayout.TextField("Save Path:", _basePath + "/" + _folderName);
        }
        */

        _savePath = EditorGUILayout.TextField("Save Path:", _savePath);

        EditorGUILayout.Space(20);
    }

    private void HandleBakingTypeChoice()
    {
        BakingType newBakingType = (BakingType)EditorGUILayout.EnumPopup("Baking Type", _bakingType);
        _bakingType = newBakingType;
    }

    private void HandleSingleAnimation()
    {
        GameObject newAnimatedObject = EditorGUILayout.ObjectField("Mesh Rig", AnimatedObject, typeof(GameObject), true) as GameObject;
        AnimationClip newAnimationClip = EditorGUILayout.ObjectField("Animation To Convert", SingleAnimation, typeof(AnimationClip), true) as AnimationClip;


        if (newAnimationClip != SingleAnimation && newAnimationClip != null)
        {
            //_animationName = newAnimatedObject.name + "_" + newAnimationClip.name + "_TexturedAnimations";
            _animationName = newAnimatedObject.name + "_TexAnim";

        }

        AnimatedObject = newAnimatedObject;
        SingleAnimation = newAnimationClip;
        _animationName = EditorGUILayout.TextField("Animation Name:", _animationName);
    }

    private IEnumerator GenerateTexturedAnimation(AnimationClip animationClip, GameObject animatedObject)
    {
        yield return new WaitForSeconds(0.5f);


        PlayableGraph playableGraph;
        Animator animator = null;
        SkinnedMeshRenderer skinnedMesh = null;
        Mesh currentMesh = new Mesh();
        GameObject animatedObjectInstance = Instantiate<GameObject>(animatedObject);
        bool hasFinishedAnimationConvertion = false;

        playableGraph = PlayableGraph.Create();


        if (animatedObjectInstance.TryGetComponent<Animator>(out animator))
        {
            

        }
        else
        {
            animator = animatedObjectInstance.AddComponent<Animator>();
        }

       

        skinnedMesh = animatedObjectInstance.GetComponentInChildren<SkinnedMeshRenderer>();

        int vertexCount = skinnedMesh.sharedMesh.vertices.Length;
        int frameCount = (int)(animationClip.length * 30);
        Texture2D texturedAnimation = new Texture2D(vertexCount, 64, TextureFormat.RGBAHalf, false);

        int currentAnimationFrame = 0;
        float currentAnimationTime = 0;
        float incrementValue = 1f / 30;



        if(vertexCount > 2000)
        {
            throw new System.Exception("The model have too much vertex to be used. (More than 2000) ");
        }


        // Launch the animation and set the update mode
        AnimationPlayableUtilities.PlayClip(animator, animationClip, out playableGraph);
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        // Launch the baking function
        EditorCoroutineUtility.StartCoroutine(CyclingThroughAnimation(), this);



        yield return new WaitUntil(() => hasFinishedAnimationConvertion == true);
        
        texturedAnimation.Apply();
        playableGraph.Destroy();
        
        
        // Assuming you have a path where you want to save the texture
        string savePath = "Assets/Resources/Textures/"; // Change this path to your desired location
        string fileName = _animationName + ".png"; // Change the file name if needed

        // Make sure the directory exists, create it if it doesn't
        System.IO.Directory.CreateDirectory(savePath);

        // Save the Texture2D as a PNG file
        byte[] bytes = texturedAnimation.EncodeToPNG();
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(savePath, fileName), bytes);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();



        EditorUtility.ClearProgressBar();





        IEnumerator CyclingThroughAnimation()
        {
            skinnedMesh.BakeMesh(currentMesh);

            for (int i = 0; i < vertexCount; i++)
            {
                //Color vertexPosition = new Color(sharedMesh.vertices[i].x, sharedMesh.vertices[i].y, sharedMesh.vertices[i].z);
                Color vertexPosition = new Color(currentMesh.vertices[i].x, currentMesh.vertices[i].y, currentMesh.vertices[i].z, 1);
                texturedAnimation.SetPixel(i, currentAnimationFrame, vertexPosition);
            }


            Debug.Log("Frame: " + currentAnimationFrame + " Done");

            
            currentAnimationTime += incrementValue;
            currentAnimationFrame++;

            if(currentAnimationTime >= animationClip.length)
            {
                hasFinishedAnimationConvertion = true;
            }
            else
            {
                playableGraph.Evaluate(incrementValue);
                yield return new WaitForSeconds(incrementValue);

                EditorCoroutineUtility.StartCoroutine(CyclingThroughAnimation(), this);
            }

        }
    }

}
