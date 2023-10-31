using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public AudioClip flip;  // ���� ���� ��ü
    public AudioSource audioSource; // ���� ������ �÷��� �� ������

    public Animator anim;
    
    public int spriteNum = 0;
    public void OpenCard()
    {
        audioSource.PlayOneShot(flip);

        anim.SetBool("isOpen", true);

        transform.Find("Front").gameObject.SetActive(true);
        transform.Find("Back").gameObject.SetActive(false);
        string swtime = GameManager.I.timeTxt.text;
 
        //closeCard 5���� ����
        //Invoke("CloseCard", 5.0f);

        if (GameManager.I.firstCard == null)
        {
            GameManager.I.firstCard = gameObject;
        }
        else
        {
            GameManager.I.secondCard = gameObject;
            GameManager.I.IsMatched();
        }
    }

    public void DestrotyCard()
    {
        Invoke("DestroyCardInvoke", 0.5f);
    }

    void DestroyCardInvoke()
    {
        Destroy(gameObject);
    }

    public void CloseCard()
    {
        Invoke("CloseCardInvoke", 0.5f);
    }

    void CloseCardInvoke()
    {
        anim.SetBool("isOpen", false);
        transform.Find("Back").gameObject.SetActive(true);
        transform.Find("Front").gameObject.SetActive(false);

        if (gameObject == GameManager.I.firstCard)
            GameManager.I.firstCard = null;
    }

}
