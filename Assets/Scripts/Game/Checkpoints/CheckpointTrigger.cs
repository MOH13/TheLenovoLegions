using System.Collections;
using System.Collections.Generic;
using LL.Game.Checkpoints;
using UnityEngine;

namespace LL.Game.Checkpoints
{
    public class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField]
        LayerMask layerMask;

        void OnTriggerEnter2D(Collider2D col)
        {
            if (((1 << col.gameObject.layer) & layerMask) != 0)
            {
                var checkpointee = col.gameObject.GetComponent<Checkpointable>();
                checkpointee.SetCheckpoint();
                SaveManager.Save();
            }
        }
    }
}