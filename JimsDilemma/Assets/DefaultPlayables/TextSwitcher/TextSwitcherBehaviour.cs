using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[Serializable]
public class TextSwitcherBehaviour : PlayableBehaviour
{
    public Color color = Color.white;
    public int fontSize = 14;
	//[TextArea(0,2)]
    public string text;
}
