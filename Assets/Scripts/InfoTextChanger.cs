using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InfoTextChanger : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    private Text infoText;
    private CameraMove camMove;

    void Awake() {
        camMove=mainCam.GetComponent<CameraMove>();
        infoText=GetComponent<Text>();
    }

    public void ChangeInfoText()
    {
        infoText.text=camMove.LeftTopCornerSpriteName();
    }
}
