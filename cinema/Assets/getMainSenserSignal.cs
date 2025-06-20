using UnityEngine;
using TMPro;

public class getMainSenserSignal : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI signalText;

    private int sensorSignal = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            sensorSignal = -1;
            UpdateUI();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            sensorSignal = 1;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        switch (sensorSignal)
        {
            case -1:
                signalText.text = "Signal: THROW (-1)";
                break;
            case 1:
                signalText.text = "Signal: FAST (1)";
                break;
            default:
                signalText.text = "Signal: NONE (0)";
                break;
        }
    }
}
