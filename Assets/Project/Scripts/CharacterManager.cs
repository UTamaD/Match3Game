using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class CharacterManager : MonoBehaviour
{
    
    public static CharacterManager Instance { get; private set; }
    
    public GameObject[] characterList;

    private int currentImageNum = 0;
    private Coroutine imageCoroutine = null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SetImage(int number,float duration)
    {
        if (imageCoroutine == null)
        {
            imageCoroutine = StartCoroutine(SetImageCoroutine(number,duration));
        }
        
    }
    

    private IEnumerator SetImageCoroutine(int number,float duration)
    {
        
        if (number < characterList.Length)
        {
            characterList[currentImageNum].SetActive(false);
            currentImageNum = number;
            characterList[currentImageNum].SetActive(true);
        }
        yield return new WaitForSeconds(duration);
        imageCoroutine = null;
    }
}
