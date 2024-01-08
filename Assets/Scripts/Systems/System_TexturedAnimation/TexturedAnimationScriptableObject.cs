using System;
using System.Collections.Generic;
using UnityEngine;

public class TexturedAnimationScriptableObject : ScriptableObject
{
    public int AnimationFPS;
    public List<TexturedAnimationClass> Animations = new();

}
