using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using static BakingAnimationsMachine;

public class UnBakedTextureAnimations
{


    #region FIELDS
    private readonly BakingType _bakingType;
    private readonly int _vertexCount;
    private readonly int _mapWidth;
    private readonly List<AnimationClip> _animClips;
    private PlayableGraph _playableGraph;
    private readonly string _name;
   
    private readonly Animator _animator;
    private readonly SkinnedMeshRenderer _skinnedMesh;


    public List<AnimationClip> AnimClips => _animClips;
    public SkinnedMeshRenderer SkinnedMesh => _skinnedMesh;
    public Animator Animator => _animator;

    public string Name => _name;
    public int MapWidth => _mapWidth;
    public int VertexCount => _vertexCount;
    public PlayableGraph PlayableGraph => _playableGraph;

    #endregion

    /*    public UnBakedTextureAnimations(Animation anim, SkinnedMeshRenderer smr, string goName)
        {
            _vertexCount = smr.sharedMesh.vertexCount;
            _mapWidth = Mathf.NextPowerOfTwo(_vertexCount);
            _animClips = new List<AnimationClip>();
            _animation = anim;
            _skin = smr;
            _name = goName;
        }*/


    public UnBakedTextureAnimations(BakingType bakingType, AnimationClip animClip, SkinnedMeshRenderer skinnedMesh, Animator animator, string name)
    {
        _bakingType = bakingType;
        _vertexCount = skinnedMesh.sharedMesh.vertexCount;
        _mapWidth = Mathf.NextPowerOfTwo(_vertexCount);

        _animClips = new List<AnimationClip>();
        _animClips.Add(animClip);

        _playableGraph = PlayableGraph.Create();
        _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        _animator = animator;
        _skinnedMesh = skinnedMesh;
        _name = name;
    }


    #region METHODS

    public void PlayAnimation()
    {
        AnimationPlayableUtilities.PlayClip(_animator, _animClips[0], out _playableGraph);
        _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    }


    public void SampleAnimation(float incrementValue)
    {
        if (_animator == null)
        {
            Debug.LogError("animator is null!!");
            return;
        }

        _playableGraph.Evaluate(incrementValue);
    }

    public void BakeMesh(ref Mesh m)
    {
        if (_skinnedMesh == null)
        {
            Debug.LogError("skin is null!!");
            return;
        }

        _skinnedMesh.BakeMesh(m);
    }


    public void Destroy()
    {
        _playableGraph.Destroy();
    }

    #endregion
}
