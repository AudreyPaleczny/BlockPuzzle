using UnityEngine;

public class MrV_Mino : MonoBehaviour
{
	public Vector3 targetPosition;
	Vector3 lastPosition;
	public float shiftTime = 1f/8;
	float timer = 0;
	public Transform graphic;
	bool needsToShift;

	public static Vector3 GetGridPosition(Vector3 v) {
		for (int i = 0; i < 3; ++i) {
			v[i] = Mathf.Round(v[i]);
		}
		return v;
	}

	private void Start() {
		graphic.SetParent(null);
	}

	public void FixedUpdate() {
		Vector3 newPos = GetGridPosition(transform.position);
		Vector3 delta = newPos - targetPosition;
		if (delta != Vector3.zero) {
			needsToShift = true;
			lastPosition = graphic.position;
			targetPosition = newPos;
			timer = 0;
			NonStandard.Lines.Make("move " + name).Arrow(lastPosition, targetPosition, Color.red, 1f/32);
		}
	}

	public void Update() {
		if (needsToShift) {
			timer += Time.deltaTime;
			if (timer > shiftTime) {
				graphic.position = targetPosition;
				needsToShift = false;
			} else {
				graphic.position = Vector3.Lerp(lastPosition, targetPosition, timer / shiftTime);
			}
		}
	}
}