using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField] private float speed; // platform speed
    [SerializeField] private int startingPoint; // starting index/position of the platform
    [SerializeField] private Transform[] points; // Array of the points that the platform moves to
    private int i; // index for the array "points"

    // Start is called before the first frame update
    void Start()
    {
        transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f) // if the platform is very close to its destination point
        {
            // increase the index to make platform move to next point or reset if end of list reached:
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
        }

        // move platform to point position i
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.transform.SetParent(null);
    }

}
