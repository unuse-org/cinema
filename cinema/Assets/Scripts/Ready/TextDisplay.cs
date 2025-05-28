using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    public string[] texts;
    int textNumber = 0;
    public int EndTextDisplayFlag = 0;

    void Start()
    {
        // テキストの初期化
        if (texts == null || texts.Length == 0)
        {
            Debug.LogError("No texts assigned to TextDisplay script.");
            return;
        }

        // 最初のテキストを表示
        this.GetComponent<TextMeshProUGUI>().text = texts[textNumber];

    }

    void Update()
    {
        this.GetComponent<TextMeshProUGUI>().text = texts[textNumber];
        if (textNumber != texts.Length - 1)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                textNumber = textNumber + 1;
            }
        }
        else
        {
            Invoke("EndTextDisplay", 2f);
        }
    }      
    void EndTextDisplay()
    {
        EndTextDisplayFlag = 1;
    }
}