using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashRay : MonoBehaviour {

	public Transform cameraRoot;
	public SteamVR_TrackedController controller;
	public LineRenderer visualisation;
	public float dashSpeed;
	public float dashRotationSpeed;
	public Transform teleportGoal;

	bool isDashing = false;

	// Use this for initialization
	void Start () {
		controller.TriggerClicked += DashButton;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(controller){
			transform.position = controller.transform.position;
			transform.rotation = controller.transform.rotation;
		}

		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, 100)){
			if(visualisation != null)
				visualisation.SetPosition(1, Vector3.forward * hit.distance);
			if(teleportGoal != null){
				if(!isDashing){
					teleportGoal.gameObject.SetActive(true);
					teleportGoal.SetPositionAndRotation(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
				} else {
					teleportGoal.gameObject.SetActive(false);
				}
			}
		} else {
			if(visualisation != null)
				visualisation.SetPosition(1, Vector3.forward * 1000);
			if(teleportGoal != null)
				teleportGoal.gameObject.SetActive(false);
		}
	}

	void DashButton(object sender, ClickedEventArgs e){
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, 100)){
			StartCoroutine(DashTo(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)));
		}
	}

	IEnumerator DashTo(Vector3 endPoint, Quaternion endRotation){
		Vector3 startPoint = cameraRoot.position;
		Quaternion startRot = cameraRoot.rotation;
		float distanceDuration = Vector3.Distance(startPoint, endPoint) / dashSpeed;
		float rotationDuration = Quaternion.Angle(startRot, endRotation) / dashRotationSpeed;
		float duration = Mathf.Max(distanceDuration, rotationDuration);
		
		float startTime = Time.time;
		while(Time.time < startTime + duration){
			float fract = (Time.time - startTime) / duration;
			cameraRoot.SetPositionAndRotation(
					Vector3.Lerp(startPoint, endPoint, fract), 
					Quaternion.Slerp(startRot, endRotation, fract));
			yield return null;
		}
		cameraRoot.position = endPoint;
		cameraRoot.rotation = endRotation;
	}
}
