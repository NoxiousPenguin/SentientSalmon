using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CatchFish : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText;
    [HideInInspector] public int score;

    void Start(){
        score = 0;
    }

    // not really sure why the other method is incompatible with the smart salmon
    void OnCollisionEnter2D(Collision2D collision) {

        if (collision.collider.gameObject.tag == "AI Salmon") {
            score++;
            scoreText.text = score.ToString();
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.tag == "AI Salmon"){
            Destroy(other.gameObject);
            score++;
            scoreText.text = score.ToString();
        }
    }
}
