using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool isPlaying = false;
    public static bool isSliceMode = true;
    public static bool isGameOver = false;
    public static int chance = 3;
    public static int collectedBomb = 0;
    private static int score = 0;
    private AudioSource failAudio;

    public static TMP_Text scoreObj;
    public TMP_Text mode;
    public GameObject explosion;
    public GameObject GameOverModal;
    public RawImage[] failIcons;

    private FetchHandData handDataScript;
    private int curHandType;

    private void Awake()
    {
        Debug.Log("Awake");
        score = 0;
        scoreObj = GameObject.Find("Score").GetComponent<TMP_Text>();
        chance = 3;
        collectedBomb = 0;
        explosion.SetActive(false);
        GameOverModal.SetActive(false);
        isPlaying = true;
        isGameOver = false;
        isSliceMode = true;
        handDataScript = this.gameObject.GetComponent<FetchHandData>();
    }

    void Start()
    {
        Time.timeScale = 1;
        failAudio = failIcons[0].transform.parent.gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        // 남은 기회를 확인하고 X 표시 UI 를 업데이트함
        CheckChance();
        
        // isGameOver 가 true 일 때 게임오버 로직을 실행함
        if (isGameOver)
        {
            GameOver();
            return;
        }

        // hand data 를 가져와 모드를 지속적으로 전환해줌 
        curHandType = handDataScript.handType;

        if (curHandType == 0) {
            isSliceMode = true;
            mode.text = "Slice!";
        } else {
            isSliceMode = false;
            mode.text = "Grab";
        }
    }

    public static void MoveScene(int sceneIdx)
    {
        SceneManager.LoadScene(sceneIdx);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("fruit"))
        {
            chance--;
            failAudio.Play();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("bomb"))
        {
            Destroy(other.gameObject);
            isGameOver = true;
        }
    }

    // isGameOver 값이 true 일 때 발동됨
    // 폭탄을 베었을 때 발동
    public void GameOver()
    {
        // 폭발 효과를 재생한 뒤
        explosion.SetActive(true);

        // 1초 뒤에 게임을 멈춤
        StartCoroutine(DelayTimeAndEndGame(1));
        Debug.Log("Game Over");
    }

    IEnumerator DelayTimeAndEndGame(int sec)
    {
        yield return new WaitForSeconds(sec);
        StopGameWithGameOverModal();
    }


    // 기회가 모두 차감되었을 때 게임을 멈추고 게임오버 모달을 띄움 (기회 없을 시 게임 오버 로직)
    public void StopGameWithGameOverModal()
    {
        MuteBomb();
        isPlaying = false;
        GameOverModal.SetActive(true);
        Time.timeScale = 0;
    }

    private void CheckChance()
    {
        switch (chance) {
            case 2:
                failIcons[0].material = null;
                break;
            case 1:
                failIcons[1].material = null;
                break;
            case 0:
                failIcons[2].material = null;
                StopGameWithGameOverModal();
                break;
            default:
                break;
                
        }
    }

    private void MuteBomb()
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("bomb");
        foreach (GameObject bomb in bombs)
        {
            bomb.GetComponent<AudioSource>().Stop();
        }
    }

    public static void IncreaseScore()
    {
        score += 10;
        scoreObj.text = score.ToString();
    }
}
