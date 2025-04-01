using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlashlightManager : MonoBehaviour
{

    #region Variables

    // 手持ちライト用
    [SerializeField] GameObject flashLightSystem;
    [SerializeField] GameObject flashlightTutorial;
    private bool hasTutorialShown = false; // チュートリアルがもう表示されたか

    // 他クラス
    private Inventory inventory;
    private TutorialManager tutorialManager;

    #endregion

    #region Methods

 
    // Start is called before the first frame update
    void Start()
    {
        // 他クラスを取得
        tutorialManager = flashlightTutorial.GetComponent<TutorialManager>();
        inventory = FindObjectOfType<Inventory>();

    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.items.Count > 0)
        {
            // 手にフラッシュライトを持っているとき
            if (inventory.selectedItem != null && inventory.selectedItem.item.name == "Flashlight")
            {
                // フラッシュライトシステムがあるか
                if (flashLightSystem != null)
                {
                    // フラッシュライトを持つとフラッシュライトシステムがONになる
                    flashLightSystem.SetActive(true);

                    if (tutorialManager != null && !hasTutorialShown)
                    {
                        hasTutorialShown = true; // チュートリアル画面が見えているというフラグを立てる
                        StartCoroutine(tutorialManager.ShowTutorial()); // チュートリアル表示
                        flashlightTutorial.SetActive(true); // フラッシュライトチュートリアル表示
                        flashlightTutorial.transform.GetChild(0).gameObject.SetActive(true); // フラッシュライトオブジェクトをONにする
                    }
                }
            }
            else
            {
                if (flashLightSystem != null)
                {
                    flashLightSystem.SetActive(false); // フラッシュライトオブジェクトをONにする

                }
            }
        }
        
    }

    #endregion
}
