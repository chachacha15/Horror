using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightManager : MonoBehaviour
{
    // ���N���X
    private Inventory inventory;

    // �莝�����C�g�p
    [SerializeField] GameObject flashLightSystem;
    [SerializeField] GameObject flashlightTutorial;
    private TutorialManager tutorialManager;
    private bool hasTutorialShown = false; // �`���[�g���A���������\�����ꂽ��


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
