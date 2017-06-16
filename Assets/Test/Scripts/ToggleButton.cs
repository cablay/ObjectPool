using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    Button button;
    Text buttonText;
    ColorBlock onColorBlock;
    bool useObjectPool;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
        onColorBlock = button.colors;
        useObjectPool = true;
    }

    public void ToggleObjectPoolUsage()
    {
        useObjectPool = !useObjectPool;
        button.colors = useObjectPool ? onColorBlock : ColorBlock.defaultColorBlock;
        buttonText.text = useObjectPool ? "Using ObjectPool" : "Not Using ObjectPool";
    }
}
