using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleLightMover : MonoBehaviour
{
    Light candle;
    public Vector3 startPos;
    const float endmove = 1;            //def:
    readonly float max = 2.45f;         //2.2
    readonly float start = 0.3f;        //0.3
    readonly float min = 2.35f;         //1.8
    bool increasing = true;

    float speed = 1.9f;

    public bool dirRight = true;
    public float swayOffset = 15.7f;
    public float swaySpeed;
    float posX1, posX2;

    // Start is called before the first frame update
    void Start()
    {
        candle = GetComponent<Light>();
        startPos = transform.position;
        posX1 = startPos.x + swayOffset;
        posX2 = startPos.x - swayOffset;
        swaySpeed = Random.Range(0.105f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (increasing && candle.intensity < max)
        {
            candle.intensity += Time.deltaTime * speed;
        }
        if (candle.intensity >= max || (candle.intensity > min && !increasing))
        {
            increasing = false;
            candle.intensity -= Time.deltaTime * speed;
        }
        if (candle.intensity <= min)
        {
            increasing = true;
        }
        
        if (dirRight)
        {
            transform.Translate(Vector2.right * swaySpeed * Time.deltaTime);
            transform.Translate(Vector2.down * swaySpeed / 2 * Time.deltaTime);
        }
        else
        {
            transform.Translate(-Vector2.right * swaySpeed * Time.deltaTime);
            transform.Translate(-Vector2.down * swaySpeed / 2 * Time.deltaTime);
        }

        if (transform.position.x >= posX1)
        {
            dirRight = false;
        }

        if (transform.position.x <= posX2)
        {
            dirRight = true;
        }
    }
}
