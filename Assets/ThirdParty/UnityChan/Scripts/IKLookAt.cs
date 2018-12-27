//
// Mecanim.IkLookAt
// 使用時には、AnimatorのBase LayerのIK PassをONにすること.
//
using UnityEngine;
using System.Collections;


namespace UnityChan
{

//	[RequireComponent(typeof(Animator))]  

	public class IKLookAt : MonoBehaviour {

        private Animator avator;
        private MeshRenderer target = null;
		
		public bool ikActive = false;
		public Transform lookAtObj = null;
		
		public float lookAtWeight = 1.0f;
		public float bodyWeight = 0.3f;
		public float headWeight = 0.8f;
		public float eyesWeight = 1.0f;
		public float clampWeight = 0.5f;
        public bool isGUI = true;





		// Use this for initialization
		void Start () {
            avator = GetComponent<Animator>();
            if (lookAtObj != null)
            {
                target = lookAtObj.GetComponentInParent<MeshRenderer>();
                target.enabled = false;
            }
            
		}

		void OnGUI()
		{

            if (isGUI)
            {
                Rect rect1 = new Rect(Screen.width - 120, Screen.height - 40, 100, 30);
                //GUILayout.Label("Activate Look at IK");
                ikActive = GUI.Toggle(rect1, ikActive, "Look at Target");
            }
		}


		void OnAnimatorIK(int layorIndex)
		{		
			if(avator)
			{
				if(ikActive)
				{
                    avator.SetLookAtWeight(lookAtWeight,bodyWeight,headWeight,eyesWeight,clampWeight);
                    if (lookAtObj != null)
                    {
                        target.enabled = true;
                        avator.SetLookAtPosition(lookAtObj.position);
                    }
                    else
                    {
                        avator.SetLookAtWeight(0.0f);
                    }
				}
				else
				{
					avator.SetLookAtWeight(0.0f);
				}
			}
		}
		
		void Update () 
		{
			if(avator)
			{
				if(!ikActive)
				{
                    if (lookAtObj != null)
					{
                        target.enabled = false;
                        //Targetを消した位置に再び出したい場合には、下の行をコメントアウトする.
                        //lookAtObj.position = avator.bodyPosition + avator.bodyRotation * new Vector3(0,0.5f,1);
					}				
				}
			}		
		}   		  
	}
}
