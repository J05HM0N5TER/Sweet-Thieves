using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cedits : MonoBehaviour
{
    [SerializeField] Canvas designers = null;
    [SerializeField] Canvas Artists_1 = null;
    [SerializeField] Canvas Artists_2 = null;
    [SerializeField] Canvas Programmers = null;
    [SerializeField] Canvas specialThanks = null;
    [SerializeField] Canvas teachers = null;
    [SerializeField] Canvas CodenameLemon = null;
    [SerializeField] float delay = 5.0f;


    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
            
    }
    // Update is called once per frame
    void Update()
    {
        StartCoroutine(itterateThroughCavases());
    }
    private IEnumerator itterateThroughCavases()
    {
        designers.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        designers.gameObject.SetActive(false);
        Artists_1.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        Artists_1.gameObject.SetActive(false);
        Artists_2.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        Artists_2.gameObject.SetActive(false);
        Programmers.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        Programmers.gameObject.SetActive(false);
        specialThanks.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        specialThanks.gameObject.SetActive(false);
        teachers.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        teachers.gameObject.SetActive(false);
        CodenameLemon.gameObject.SetActive(true);
    }
}
