using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIControler : MonoBehaviour
{
    [SerializeField] private TMP_Text HPOutput;
    [SerializeField] private TMP_Text DeathNotification;
    [SerializeField] private TMP_Text Sight;
    [SerializeField] private TMP_Text ElevatorHint;

    private bool TargetIsSelected = false;
    private Collider2D _Target;

    private void Update() 
    {
        if(TargetIsSelected)
        {
            RedrawningSight();
        }
    }
    
    public void SetHP(int HP)
    {
        HPOutput.text = $"Здоровье: {HP}";
    }

    public void ActivateDeathNotification()
    {
        DeathNotification.gameObject.SetActive(true);
    }

    public void DrawSight(Collider2D Target)
    {
        if(Target != null)Debug.Log("_Target != Null");
        else Debug.Log("_Target = null");
        Vector3 ScreenPosition = Camera.main.WorldToScreenPoint(Target.transform.position);
        Sight.rectTransform.position = ScreenPosition;
        _Target = Target;
        TargetIsSelected = true;

        Sight.gameObject.SetActive(true);
    }

    public void StopDrawningSight()
    {
        Sight.gameObject.SetActive(false);
        TargetIsSelected = false;
    }

    public void ShowHint()
    {
        ElevatorHint.gameObject.SetActive(true);
    }

    public void HideHint()
    {
        ElevatorHint.gameObject.SetActive(false);
    }

    private void RedrawningSight()
    {
        Vector3 ScreenPosition = Camera.main.WorldToScreenPoint(_Target.transform.position);
        Sight.rectTransform.position = ScreenPosition;
    }

}
