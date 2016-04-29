using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HelpMessage : MonoBehaviour
{
    public float TimeOut = 5;

    private Text _text;
    private Image _panel;
    private float _lastSet;

    public static HelpMessage Instance { get; private set; }

    // Use this for initialization
    void Awake ()
    {
        Instance = this;
        _text = GetComponentInChildren<Text>();
        _panel = GetComponentInChildren<Image>();
    }

    void Update()
    {
        if (_lastSet + TimeOut < Time.time)
        {
            Color color = _text.color;
            color.a -= .1f;
            _text.color = color;

            color = _panel.color;
            color.a -= 0.06f;
            _panel.color = color;
        }
    }

    public void SetMessage(string msg)
    {
        if (msg == null)
        {
            _lastSet = float.MinValue;
            return;
        } 
    
        _text.text = msg;

        Color color = _text.color;
        color.a = 1.0f;
        _text.color = color;

        color = _panel.color;
        color.a = 0.6f;
        _panel.color = color;

	    _lastSet = Time.time;
	}
}
