using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class ThirdPersonCamera : MonoBehaviour 
	{
        //Angle threshold at which the camera cant go above/below ->(prevent unwanted behaviour); like turning your head all the way back)
        private const float CAMERA_ANGLE_LOW_LIMIT = -90f;
        private const float CAMERA_ANGLE_HIGH_LIMIT = 90f; 

		//Camera publics i.e what "users" call editable settings
        public GameObject theCamera; //read as "GameObject theCamera"
        public float followDistance = 5.0f;
        public float mouseSensitivityX = 4.0f;
        public float mouseSensitivityY = 2.0f;
        public float heightOffset = 0.5f;
		
		//Camera positioning
		Vector2 cameraMovement; //Sort of like cursor position but takes the "camera" position set by the cursor; "cameraPosition = cursorPos"
		float cameraRotationX;
		float cameraRotationY;
		
		//Scroll 
		float maxScrollDistance = 3f;
        float scrollDistance;
		
		bool firstClickFlag; //when you zoom out for the first time there is a wierd skip when scrolling
		Vector2 mouseUpPosition; //in third person whehn right click is released we store this position to use for the camera to be in to retain consistent camera angle
        void Start () {
            //place the camera and set the forward vector to match player
            theCamera.transform.forward = gameObject.transform.forward;
			cameraMovement = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y"));
			
			cameraRotationX = theCamera.transform.eulerAngles.x; //Set the base rotation
			cameraRotationY = theCamera.transform.eulerAngles.y; //Set the base rotation
			
            //hide the cursor and lock the cursor to center
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Locked;
			
			firstClickFlag = true;
			mouseUpPosition = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y"));
        }
        void SetScrollDistance()
        {
            scrollDistance += Input.GetAxisRaw("Mouse ScrollWheel"); //0.1->1
		    scrollDistance =  Mathf.Clamp(scrollDistance, 0, maxScrollDistance); //'0' so we dont inverse scroll
        }
		void HandleThirdPersonView()
		{
			Cursor.lockState = CursorLockMode.None;
            if(Input.GetKey(KeyCode.Mouse1))
            {
				if(Input.GetKeyDown(KeyCode.Mouse1))
				{
					cameraRotationX = mouseUpPosition.x;
					cameraRotationY = mouseUpPosition.y;
				}
				theCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(cameraRotationX, CAMERA_ANGLE_LOW_LIMIT, CAMERA_ANGLE_HIGH_LIMIT), cameraRotationY, 0f);
            }
			if(Input.GetKeyUp(KeyCode.Mouse1))
			{
				mouseUpPosition = new Vector2(cameraRotationX, cameraRotationY);
			}
            theCamera.transform.position -= theCamera.transform.forward * scrollDistance * followDistance; //This moves the camera based on the "scroll" amount; -> see "void SetScrollDistance()"
		}
		void HandleFirstPersonView()
		{
			Cursor.lockState = CursorLockMode.Locked;
            theCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(cameraRotationX, CAMERA_ANGLE_LOW_LIMIT, CAMERA_ANGLE_HIGH_LIMIT),cameraRotationY, 0);
		}
		void HandleViewPoint()
		{
			cameraRotationX += -cameraMovement.y * mouseSensitivityY; //calculate the rotation
			cameraRotationY +=  cameraMovement.x * mouseSensitivityX; //calculate the rotation
            if(scrollDistance > .05f) //Min scroll
            {
				if(firstClickFlag)
				{
					mouseUpPosition = new Vector2(cameraRotationX,cameraRotationY);
				}
				HandleThirdPersonView();
				firstClickFlag = false;
            }
            else
            {
				firstClickFlag = true;
				HandleFirstPersonView();
            }
		}
		
        void Update () 
		{	
            cameraMovement = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y"));

            //first we place the camera at the position of the player + height offset
            theCamera.transform.position = gameObject.transform.position + new Vector3(0,heightOffset,0);
            SetScrollDistance();
			HandleViewPoint();
		}
	}