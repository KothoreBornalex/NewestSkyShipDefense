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

public class TexturedAnimationEditorWindow : EditorWindow
{
    

    private static BakingAnimationsMachine _bakingAnimationsMachine;

    private BakingType _bakingType;
    private GameObject AnimatedObject;
    private AnimationClip SingleAnimation;


    private string _name;

    private const string BASE_PATH = "Assets/";
    private string savePath = "";




    [MenuItem("Tools/Textured Animation Creator")]
    public static void CreateEditorWindow()
    {
        EditorWindow window = GetWindow<TexturedAnimationEditorWindow>();
        window.titleContent = new GUIContent("Textured Animation Editor");

        _bakingAnimationsMachine = new BakingAnimationsMachine();
    }


    private void OnGUI()
    {
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
            //EditorCoroutineUtility.StartCoroutine(GenerateTexturedAnimation(TargetedAnimation, AnimatedObject, DryRun), this);
            switch (_bakingType)
            {
                case BakingType.SingleAnimation:
                    _bakingAnimationsMachine.LaunchBaking(_bakingType, SingleAnimation, AnimatedObject, _name);
                    //EditorCoroutineUtility.StartCoroutine(GenerateTexturedAnimation(SingleAnimation, AnimatedObject), this);
                    break;

                case BakingType.BlendingAnimation:
                    break;

                case BakingType.MultipleAnimations:
                    break;
            }


            Debug.Log("Debug Test: " + BASE_PATH + _name);
        }

        GUI.enabled = true;
        if (GUILayout.Button("Clear progress bar"))
        {
            EditorUtility.ClearProgressBar();
        }

    }

    private string OpenExplorer(string basePath)
    {
        return EditorUtility.OpenFilePanel("Select the Animation to Convert", basePath, "");
    }







    private void HandleSavePathButton()
    {
        EditorGUILayout.Space(20);
        if (GUILayout.Button("Select Save Path"))
        {
            Debug.Log("Is Working");
            savePath = OpenExplorer(BASE_PATH);
        }
        EditorGUILayout.Space(20);
    }

    private void HandleBakingTypeChoice()
    {
        BakingType newBakingType = (BakingType)EditorGUILayout.EnumPopup("Baking Type", _bakingType);
        _bakingType = newBakingType;
    }

    private void HandleSingleAnimation()
    {
        GameObject newAnimatedObject = EditorGUILayout.ObjectField("New Mesh", AnimatedObject, typeof(GameObject), true) as GameObject;
        AnimationClip newAnimationClip = EditorGUILayout.ObjectField("New Animation", SingleAnimation, typeof(AnimationClip), true) as AnimationClip;


        if (newAnimationClip != SingleAnimation && newAnimationClip != null)
        {
            _name = newAnimatedObject.name + "_" + newAnimationClip.name + "_TexturedAnimations";
        }

        AnimatedObject = newAnimatedObject;
        SingleAnimation = newAnimationClip;
        _name = EditorGUILayout.TextField("Name", _name);
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
        string fileName = _name + ".png"; // Change the file name if needed

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
