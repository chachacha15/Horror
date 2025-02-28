using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightManager : MonoBehaviour
{

    #region Variables
    // 他クラス
    private Inventory inventory;

    // 手持ちライト用
    [SerializeField] GameObject flashLightSystem;
    [SerializeField] GameObject flashlightTutorial;
    private TutorialManager tutorialManager;
    private bool hasTutorialShown = false; // チュートリアルがもう表示されたか

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

    #endregion
}
