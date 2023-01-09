using UnityEngine;

public class KeycardLogic : MonoBehaviour
{
    private GameObject[] keyCards;
    private LevelSettings _levelSettings;
    public bool keyCardsCompleted;
    private int collectedKeycards;
    private Quaternion keyCardRotation;

    void Start()
    {
        _levelSettings = FindObjectOfType<LevelSettings>();
        if (_levelSettings.KeycardsExistInLevel())
        {
            keyCards = new GameObject[_levelSettings.GetNumberOfKeycardsInLevel()];
            keyCards = GameObject.FindGameObjectsWithTag("Keycard");
        }
        else
        {
            keyCardsCompleted = true;
        }
    }

    public void AddCollectedKeyCard(int instanceId)
    {
        collectedKeycards++;
        foreach (var key in keyCards)
        {
            if (key.activeSelf)
            {
                if (key.GetInstanceID() == instanceId)
                {
                    key.SetActive(false);
                }
            }
        }

        if (collectedKeycards == keyCards.Length)
        {
            keyCardsCompleted = true;
        }
    }
}