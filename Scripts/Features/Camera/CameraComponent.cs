using UnityEngine;

namespace Client
{
    struct CameraComponent
    {
        public GameObject CameraObject;
        public GameObject HolderObject;
        public GameObject FollowObject;
        public Transform CameraTransform;
        public Transform HolderTransform;
        public Transform FollowTransform;
        //public Animator Animator;
        //public CameraMB CameraMB;
    }
}