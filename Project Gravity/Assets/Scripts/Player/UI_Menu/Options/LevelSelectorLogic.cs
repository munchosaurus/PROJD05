using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorLogic : MonoBehaviour
{
    public void SelectLevel()
    {
        FindObjectOfType<LevelSelector>().SelectLevel(GetComponent<Button>());
    }
}
