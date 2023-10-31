using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Profiling;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public GameObject card;
    public GameObject firstCard;
    public GameObject secondCard;

    public Text timeTxt;
    public GameObject endTxt;
    public Text addTxt;
    public Text maxScoreText;
    public Text thisScoreText;

    public GameObject endPanel;

    private RectTransform transaddtxt;
    byte c;
    public int check;

    float time;
    public float maxTime;
    public float warningTime;
    bool isRunning = true;

    public AudioManager audioManager;

    public AudioClip match;
    public AudioClip failed;
    public AudioClip bestscore;
    public AudioClip lowscore;
    public AudioSource audioSource;

    public Sprite[] sprites;    // sprite�� Inspectorâ���� �ޱ� ���� ����
    List<GameObject> cardList;  // card���� ���� cardList, ����� card�� ���µ� ���
    public List<GameObject> namelist;

    public GameObject warningBackground;

    private void Awake()
    {
        I = this;
    }

    void Start()
    {
        transaddtxt = addTxt.GetComponent<RectTransform>();
        Time.timeScale = 1.0f;

        cardList = new List<GameObject>();
        //namelist = new List<GameObject>();
        Sprite tempSprite = sprites[0];
        int tempSpriteNum = 0;

        // 12���� ī�� ����
        // ī�� sprite�� ���������� �־���
        for (int i = 0; i < 12; ++i)
        {
            // ī��� 12�� �����Ǿ�� �ϴµ� sprite�� 6��
            // 2���� ī��� ���� ī�忩�� �ϹǷ�
            if (i % 2 == 0)     // 0, 2, 4, 6, 8, 10 �϶��� sprite�� �ٲ�
            {
                tempSprite = sprites[i / 2];
                tempSpriteNum = i / 2;
            }

            GameObject newCard = Instantiate(card);
            newCard.transform.parent = GameObject.Find("Cards").transform;

            float x = (i / 4) * 1.4f - 2.1f;
            float y = (i % 4) * 1.4f - 3.0f;
            newCard.transform.position = new Vector3(x, y, 0);


            newCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite = tempSprite;
            newCard.GetComponent<Card>().spriteNum = tempSpriteNum; // card�� spriteNum �־��ֱ�
            cardList.Add(newCard);  // List�� ������ ī�� �־��ֱ�
        }

        // ī�� ����
        for(int i = cardList.Count- 1; i> 0; --i)
        {
            int randomNum = Random.Range(0, i);

            // swap
            Vector3 tempPosition = cardList[i].transform.position;
            cardList[i].transform.position = cardList[randomNum].transform.position;
            cardList[randomNum].transform.position = tempPosition;
        }
    }

    void Update()
    {
        float addy = transaddtxt.anchoredPosition.y;         // addtxt ��ġ
        addy += 0.5f;                                        // addtxt y�� ���
        transaddtxt.anchoredPosition = new Vector2(0, addy);  // addtxt y�� ���

        c -= 1;                                              // ���� ���� �����ϰ�
        addTxt.color = new Color32(255, 0, 0, c);            // ���� ���� �����ϰ�


        time += Time.deltaTime;

        if (time > warningTime)
        {
            warningBackground.gameObject.SetActive(true);
            audioManager.GetComponent<AudioSource>().pitch = 1.5f;
        }
        if (time > maxTime)
            Invoke("GameEnd", 0.5f);

        timeTxt.text = time.ToString("N2");
    }

    public void IsMatched()
    {
        int firstCardSpriteNum = firstCard.GetComponent<Card>().spriteNum;
        int secondCardSpriteNum = secondCard.GetComponent<Card>().spriteNum;

        if(firstCardSpriteNum == secondCardSpriteNum)
        {
            audioSource.PlayOneShot(match);

            string info = firstCard.GetComponentInChildren<SpriteRenderer>().sprite.name;   // sprite�� �̸� rtanx info�� ����
            check = int.Parse(info.Substring(info.Length - 1)) -1;  // rtanx �� x�κ� �ڸ���, int �� ����
            // �迭�� 0���� �����ϹǷ� -1

            //check = firstCard.GetComponent<Card>().spriteNum;

            namelist[check].SetActive(true);            // Active True
            Invoke("nActiveFalse", 1.0f);               // 1�� �� false

            firstCard.GetComponent<Card>().DestrotyCard();
            secondCard.GetComponent<Card>().DestrotyCard();

            int cardsLeft = GameObject.Find("Cards").transform.childCount;
            if (cardsLeft == 2)
                Invoke("GameEnd", 0.5f);
        }
        else
        {
            audioSource.PlayOneShot(failed);

            firstCard.GetComponent<Card>().CloseCard();
            secondCard.GetComponent<Card>().CloseCard();

            // �ð� �߰� ���
            time += 5;
            addTxt.color = new Color32(255, 0, 0, 255);             // ���ڻ� RED
            c = 0;                                                  // ���� �ʱ�ȭ
            transaddtxt.anchoredPosition = new Vector2(0, 450);     // ���� ��ġ �ʱ�ȭ (����ð� ��)
            addTxt.gameObject.SetActive(true);                      // addTXT Ȱ��ȭ
            Invoke("ActiveFalse", 1.0f);                            // 1�� �� ActiveFalse ����
        }

        firstCard = null;
        secondCard = null;
    }

    void ActiveFalse()
    {
        addTxt.gameObject.SetActive(false);                          // addtxt ��Ȱ��ȭ �ϱ�
    }
    void nActiveFalse()
    {
        namelist[check].SetActive(false);
    }

    void GameEnd()
    {
        warningBackground.gameObject.SetActive(false);

        isRunning = false;
        endPanel.SetActive(true);
        Time.timeScale = 0f;
        thisScoreText.text = time.ToString("N2");

        //endTxt.SetActive(true);

        if (PlayerPrefs.HasKey("bestscore") == false)
        {
            // ��������� ����Ʈ ���ھ�� ������ �뷡
            audioSource.PlayOneShot(bestscore);
            PlayerPrefs.SetFloat("bestscore", time);

        }
        else if (time < PlayerPrefs.GetFloat("bestscore"))
        {
            // ��������� ����Ʈ ���ھ�� ������ ������ �뷡
            audioSource.PlayOneShot(bestscore);
            PlayerPrefs.SetFloat("bestscore", time);

        }
        else
        {
            // ��������� ����Ʈ ���ھ�� ������ ������ �뷡
            audioSource.PlayOneShot(lowscore);
        }

        float maxScore = PlayerPrefs.GetFloat("bestscore");
        maxScoreText.text = maxScore.ToString("N2");
        EndGameBgmStop();
    }

    void EndGameBgmStop()
    {
        if (audioManager != null && audioManager.audioSource != null)
        {
            audioManager.audioSource.Stop();
        }
    }

    public void RetryGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoHomeBtn()
    {
        SceneManager.LoadScene("StartScene");
    }
}
