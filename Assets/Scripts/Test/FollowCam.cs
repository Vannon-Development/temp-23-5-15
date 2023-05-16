using System;
using UnityEngine;

namespace Test
{
    public class FollowCam : MonoBehaviour
    {
        [SerializeField] private GameObject target;

        private void Update()
        {
            if(target) transform.position = target.transform.position - new Vector3(0, 0, 10);
        }
    }
}
