using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public GameObject effectPrefab;
    public Transform effectGroup;

    public int score;
    public int maxLevel;
    public bool isOver;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        NextDongle();
    }

    Dongle GetDongle()
    {
        //effect created
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();

        //dongle created
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup);
        Dongle instantDongle = instantDongleObj.GetComponent<Dongle>();
        instantDongle.effect = instantEffect;

        return instantDongle;
    }

    void NextDongle()
    {
        if (isOver)
        {
            return;
        }

        Dongle newDongle = GetDongle();
        lastDongle = newDongle;
        lastDongle.manager = this;
        lastDongle.level = Random.Range(0, 3);
        lastDongle.gameObject.SetActive(true);

        StartCoroutine("WaitNext");
    }

    IEnumerator WaitNext()
    {
        while (lastDongle != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        NextDongle();
    }

    public void TouchDown()
    {
        if(lastDongle == null)
        {
            return;
        }

        lastDongle.Drag();
    }

    public void TouchUP()
    {
        if (lastDongle == null)
        {
            return;
        }

        lastDongle.Drop();
        lastDongle = null;
    }

    public void GameOver()
    {
        if (isOver)
        {
            return;
        }

        isOver = true;

        StartCoroutine("GameOverRoutine");
    }

    IEnumerator GameOverRoutine()
    {
        Dongle[] dongles = GameObject.FindObjectsOfType<Dongle>();

        for (int index = 0; index < dongles.Length; index++)
        {
            dongles[index].rigid.simulated = false;
        }

        for (int index = 0; index < dongles.Length; index++)
        {
            dongles[index].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
