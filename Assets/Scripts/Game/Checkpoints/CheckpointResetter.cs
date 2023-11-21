using System.Collections;
using System.Collections.Generic;
using LL.Game.Checkpoints;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LL.Game.Checkpoints
{
    public class CheckpointResetter : MonoBehaviour
    {
        [SerializeField]
        LayerMask layerMask;

        void OnTriggerEnter2D(Collider2D col)
        {
            if (((1 << col.gameObject.layer) & layerMask) != 0)
            {
                SceneController.Instance.NextLevel(SceneManager.GetActiveScene().name);
                // var checkpointee = col.gameObject.GetComponent<Checkpointable>();
                // checkpointee.ResetToCheckpoint();
            }
        }
    }
}