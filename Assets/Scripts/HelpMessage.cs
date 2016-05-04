using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HelpMessage : MonoBehaviour
{
    public float TimeOut = 5;

    private Text _text;
    private Image _panel;
    private CanvasGroup _canvasGroup;
    private float _lastSet;
    private bool setMsg = false;

    public static HelpMessage Instance { get; private set; }

    Color textColor;
    Color panelColor;

    // Use this for initialization
    void Awake ()
    {
        Instance = this;
        _text = GetComponentInChildren<Text>();
        _panel = GetComponentInChildren<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();   

        textColor = _text.color;
        panelColor = _panel.color;
    }

    void Update()
    {
        if(setMsg)
        {
            //textColor.a += 0.1f;
            //_text.color = textColor;

            //panelColor.a += 0.1f;
            //_panel.color = panelColor;
            _canvasGroup.alpha += 0.1f;
        }

        if (_lastSet + TimeOut < Time.time)
        {
            //textColor.a -= 0.06f;
            //_text.color = textColor;

            //panelColor.a -= 0.06f;
            //_panel.color = panelColor;
            _canvasGroup.alpha -= 0.06f;

            setMsg = false; 
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
        //if(!setMsg)
        //{
        //    textColor.a = 0;
        //    panelColor.a = 0;
        //}
        setMsg = true;
	    _lastSet = Time.time;
	}
}