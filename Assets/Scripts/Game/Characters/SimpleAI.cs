using LL.Game.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{

    public GameObject mainCharacter;
    public float speed;
    public float chaseDistance; // chasing starts at this distance
    private float distance;
    public float sneakFactor;
    public CatBehaviour catBehaviour;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 aiPosition = transform.position, mainCharacterPosition = mainCharacter.transform.position;
        distance = Vector2.Distance(aiPosition, mainCharacterPosition);
        Vector2 direction = (mainCharacterPosition - aiPosition);
        direction.Normalize();
        if (distance < chaseDistance - sneakFactor)
        {
            transform.position = Vector2.MoveTowards(aiPosition, mainCharacterPosition, speed * Time.deltaTime);
        }
    }
}
