using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightManager : MonoBehaviour
{
    // 他クラス
    private Inventory inventory;

    // 手持ちライト用
    [SerializeField] GameObject flashLightSystem;
    [SerializeField] GameObject flashlightTutorial;
    private TutorialManager tutorialManager;
    private bool hasTutorialShown = false; // チュートリアルがもう表示されたか


    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = flashlightTutorial.GetComponent<TutorialManager>();
        inventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.selectedItem != null && inventory.selectedItem.item.name == "Flashlight")
        {
            if (flashLightSystem != null)
            {
                flashLightSystem.SetActive(true);

                if (tutorialManager != null && !hasTutorialShown)
                {
                    hasTutorialShown = true;
                    StartCoroutine(tutorialManager.ShowTutorial());
                    flashlightTutorial.SetActive(true);
                    flashlightTutorial.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (flashLightSystem != null)
            {
                flashLightSystem.SetActive(false);

            }
        }
    }
}
