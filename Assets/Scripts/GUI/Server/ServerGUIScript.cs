using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;

public class ServerGUIScript : GenericGUI {
    
    [SerializeField]
    private GameObject panelGo;
    [SerializeField]
    private Transform lineTxtPrefab;

    private Text[] _logTexts;
    private int _lineIdx;

    private readonly byte MAXLINES = 14;

    protected override void OnEnable()
    {
        base.OnEnable();
        _logTexts = new Text[MAXLINES];
        _lineIdx = 0;

        Transform textTransform;

        for (byte idx = 0; idx < MAXLINES; idx++)
        {
            textTransform = Instantiate(lineTxtPrefab);
            textTransform.SetParent(panelGo.transform, false);
            _logTexts[idx] = textTransform.GetComponent<Text>();
        }
    }

    public void Log(string msg)
    {
        _lineIdx = (_lineIdx + 1) % (MAXLINES - 1);
        _logTexts[_lineIdx].text = msg;
        _logTexts[_lineIdx + 1].text = "";
    }

}
