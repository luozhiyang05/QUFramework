using UnityEngine;

namespace Tool.CameraController
{
    public class FreeCamera : MonoBehaviour
    {
        [Header("人物移动")]
        public bool useWalk = true;
        public float walkSpeed;
        public float rotateSpeed;

        [Header("角色信息")]
        public Transform playerTrans;
        public GameObject model;
        public Transform rotateLookAtTrans;
        public Transform walkLookAtTrans;

        [Header("相机旋转")]
        public float horizontalRotateSpeed = 250f;
        public float verticalRotateSpeed = 200f;
        public float maxAngle = 60;
        public float minAngle = -10;
        [Range(0, 1f)] public float rotateLerpTime = 0.075f;

        [Header("焦距")]
        public float minDistance;
        public float maxDistance;
        public float scrollSpeed = 30f;
        [Range(0, 1f)] public float scrollLerpTime = 0.15f;

        [Header("景深")] 
        public bool useFieldOfView = true;
        public float minFieldOfView = 60;
        public float maxFieldOfView = 65;
        [Range(0, 1f)] public float fieldLerpTime = 0.1f;

        [Header("按键")] public KeyCode unlockCursorKeyCode = KeyCode.LeftAlt;

        [Header("状态")]
        public bool canUseCam = true;
        public bool isVisibleCursor;


        private Camera _mainCamera;
        private Transform _mainCameraTrans;
        private Vector3 _mainCameraPos;
        private Vector3 _lookAtPos;
        private float _inputX, _inputY;
        private float _mouseInputX, _mouseInputY, _scrollInput;
        private float _nowYAngle;
        private float _nowXAngle;
        private float _tempScrollInput;

        private bool _canUseScroll = true;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera != null) _mainCameraTrans = _mainCamera.transform;
            _tempScrollInput = _mainCameraTrans.localPosition.z;
            
            var localEulerAngles = rotateLookAtTrans.localEulerAngles;
            _nowYAngle = localEulerAngles.x;
            _nowXAngle = localEulerAngles.y;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        private void Update()
        {
            if (!canUseCam) return;

            //是否运用行走
            if (useWalk) Walk();

            CameraControl();
        }
        


        private void Walk()
        {
            //input
            _inputX = Input.GetAxisRaw("Horizontal");
            _inputY = Input.GetAxisRaw("Vertical");

            if (_inputX != 0 || _inputY != 0)
            {
                //as WalkLookAtTrans input
                Vector3 inputDir = walkLookAtTrans.forward * _inputY + walkLookAtTrans.right * _inputX;

                //Face to inputDir
                Quaternion lookRotation = Quaternion.LookRotation(inputDir, Vector3.up);
                model.transform.rotation =
                    Quaternion.Slerp(model.transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

                //Player Walk
                transform.Translate(inputDir * (walkSpeed * Time.deltaTime), Space.World);
            }
        }

        private void CameraControl()
        {
            //缓存人物坐标
            Vector3 playPos = playerTrans.position;

            //Camera Horizontal
            _mouseInputX = Input.GetAxisRaw("Mouse X");
            _mouseInputY = Input.GetAxisRaw("Mouse Y");

            //Field Of View
            if (useFieldOfView)
                _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView,
                    (_inputX != 0 || _inputY != 0) ? maxFieldOfView : minFieldOfView, Time.deltaTime / fieldLerpTime);

            //Scroll
            Vector3 mainCameraPos = _mainCameraTrans.localPosition;
            _scrollInput = Input.GetAxisRaw("Mouse ScrollWheel");
            if (_scrollInput != 0 && _canUseScroll)
            {
                _tempScrollInput = _scrollInput * scrollSpeed;
                _tempScrollInput = Mathf.Clamp(_tempScrollInput + mainCameraPos.z, minDistance, maxDistance);
            }

            mainCameraPos.z = Mathf.Lerp(mainCameraPos.z, _tempScrollInput, Time.deltaTime / scrollLerpTime);
            _mainCameraTrans.localPosition = mainCameraPos;

            //Restrict angle
            if (_mouseInputX != 0 || _mouseInputY != 0)
            {
                _nowXAngle += _mouseInputX * Time.deltaTime * horizontalRotateSpeed;
                _nowYAngle += -_mouseInputY * Time.deltaTime * verticalRotateSpeed;
                _nowYAngle = Mathf.Clamp(_nowYAngle, minAngle, maxAngle);
            }

            //Rotation Camera
            Quaternion targetQuat = Quaternion.Euler(_nowYAngle, _nowXAngle, 0);
            rotateLookAtTrans.rotation =
                Quaternion.Slerp(rotateLookAtTrans.rotation, targetQuat, Time.deltaTime / rotateLerpTime);

            //Restore WalkLookAtTrans
            walkLookAtTrans.RotateAround(playPos, Vector3.up, Time.deltaTime * horizontalRotateSpeed * _mouseInputX);
        }
    }
}