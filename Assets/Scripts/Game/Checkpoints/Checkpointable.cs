using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LL.Game.Checkpoints
{
    public class Checkpointable : MonoBehaviour
    {
        public Vector2 Checkpoint { get; private set; }

        void OnEnable()
        {
            Checkpoint = transform.position;
        }

        public void SetCheckpoint()
        {
            Checkpoint = transform.position;
        }

        public void SetCheckpoint(Vector2 checkpoint)
        {
            this.Checkpoint = checkpoint;
        }

        public void ResetToCheckpoint()
        {
            transform.position = Checkpoint;
        }
    }
}

