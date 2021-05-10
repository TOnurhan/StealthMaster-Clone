using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    [SerializeField] List<LevelScript> levelList = default;
    [SerializeField] GameObject upgradeLevel = default;
    [SerializeField] Image inputImage = default;
    [SerializeField] Image fadeImage = default;
    [SerializeField] PlayerBehaviour player = default;
    [SerializeField] GameObject itemSelectionTab = default;
    [SerializeField] MoneyMagnet moneyMagnet = default;
    [SerializeField] GameObject gameOverMenu = default;
    [SerializeField] TMP_Text moneyText = default;
    [SerializeField] HealthBarManager healthBar = default;
    int totalMoneyCount;
    int levelIndex = 0;
    LevelScript currentLevel;

    private void Start()
    {
        Instance = this;
        GetLevel();
    }

    void SaveProgress()
    {

    }

    public void LevelCompleted()
    {
        inputImage.gameObject.SetActive(false);
        player.variableJoystick.Reset();

        player.transform.LookAt(levelList[levelIndex].lastPosition.position);
        LeanTween.move(player.gameObject, levelList[levelIndex].lastPosition.position, 1.5f).setOnComplete(() =>
        {
            player.transform.LookAt(Vector3.forward);
            LeanTween.alpha(fadeImage.rectTransform, 1, 1.5f).setOnComplete(() =>
            {
                player.transform.position = Vector3.zero;
                totalMoneyCount += moneyMagnet.moneyCount;
                moneyMagnet.moneyCount = 0;
                currentLevel.DisableAllEnemies();
                player.transform.LookAt(Vector3.forward);
                upgradeLevel.SetActive(true);
                LeanTween.alpha(fadeImage.rectTransform, 0, 1f).setOnComplete(() =>
                {
                    LeanTween.delayedCall(1f, () =>
                    {
                        fadeImage.raycastTarget = false;
                        itemSelectionTab.gameObject.SetActive(true);
                    });
                });        
                
            });
        });
    }

    public void UpgradeLevelCompleted()
    {
        fadeImage.raycastTarget = true;
        itemSelectionTab.SetActive(false);
        LeanTween.alpha(fadeImage.rectTransform, 1, 1f).setEase(LeanTweenType.easeInCirc).setOnComplete(() =>
        {
            upgradeLevel.SetActive(false);
            //if, liste bittiğinde hata vermemesi adına son bölümü tekrarlaması için.
            if (levelIndex < levelList.Count - 1)
            {
                levelIndex++;
            }
            GetLevel();
            LeanTween.alpha(fadeImage.rectTransform, 0, 1f).setEase(LeanTweenType.easeInCirc).setOnComplete(() => 
            {
                inputImage.gameObject.SetActive(true);
            });
        });
    }

    void GetLevel()
    {
        currentLevel = Instantiate(levelList[levelIndex], levelList[levelIndex].transform.position, Quaternion.identity);
    }

    public void GameOver()
    {
        LeanTween.alpha(fadeImage.rectTransform, 0.5f, 1f).setOnComplete(() => 
        {
            gameOverMenu.SetActive(true);
            fadeImage.gameObject.SetActive(true);
            moneyText.text = moneyMagnet.moneyCount + "";
        });
    }

    public void Restart()
    {
        LeanTween.alpha(fadeImage.rectTransform, 1f, 1f).setOnComplete(() => 
        {
            player.gameObject.SetActive(true);
            healthBar.HealthBarChanged(100);
            moneyMagnet.moneyCount = 0;
            currentLevel.DisableAllEnemies();
            Destroy(currentLevel);
            GetLevel();
            gameOverMenu.SetActive(false);
            LeanTween.alpha(fadeImage.rectTransform, 0f, 1f);
        });
    }
}
