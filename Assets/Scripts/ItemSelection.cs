using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemSelection : MonoBehaviour
{
    public List<Button> itemButtons;
    public List<GameObject> itemSlots;
    Button selectedItemButton;
    int itemSlotIndex;
    Button createdButton;

    public event Action Upgrade1Selected;
    public event Action Upgrade2Selected;
    public event Action Upgrade3Selected;
    public event Action Upgrade4Selected;
    public event Action Upgrade5Selected;
    public event Action Upgrade6Selected;

    private void OnEnable()
    {
        List<Button> secilenButtons = new List<Button>();
        while (secilenButtons.Count < 3)
        {
            int buttonListIndex = Random.Range(0, itemButtons.Count - 1);
            selectedItemButton = itemButtons[buttonListIndex];
            if (!secilenButtons.Contains(selectedItemButton))
            {
                createdButton = Instantiate(selectedItemButton);
                createdButton.transform.position = itemSlots[itemSlotIndex].transform.position;
                createdButton.transform.localScale = itemSlots[itemSlotIndex].transform.localScale;
                createdButton.transform.SetParent(transform);

                createdButton.GetComponent<Button>().onClick.AddListener(() => TaskOnClicked(buttonListIndex + 1));
                secilenButtons.Add(selectedItemButton);
                itemSlots[itemSlotIndex].SetActive(false);
                itemSlotIndex++;

            }
        }
    }

    private void OnDisable()
    {
        itemSlotIndex = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Button>())
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    void TaskOnClicked(int clickIndex)
    {
        switch (clickIndex)
        {
            case 1:
                Upgrade1Selected?.Invoke();
                break;
            case 2:
                Upgrade2Selected?.Invoke();
                break;
            case 3:
                Upgrade3Selected?.Invoke();
                break;
            case 4:
                Upgrade4Selected?.Invoke();
                break;
            case 5:
                Upgrade5Selected?.Invoke();
                break;
            case 6:
                Upgrade6Selected?.Invoke();
                break;
        }

        GameController.Instance.UpgradeLevelCompleted();
    }
}
