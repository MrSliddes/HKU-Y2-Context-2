using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayScore : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;
    public Sprite spriteGood;
    public Sprite spriteNeutral;
    public Sprite spirteBad;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "Score: " + Points.score.ToString();
        if(Points.score >= 6) image.sprite = spriteGood;
        else if(Points.score > 0) image.sprite = spriteNeutral;
        else image.sprite = spirteBad;
    }
}
