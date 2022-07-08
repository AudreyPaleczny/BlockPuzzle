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
			v[i] = Mathf.Floor(v[i]);
		}
		return v;
	}

	private void Start() {
		Loosen();
	}

	public void Loosen() {
		if (graphic == null) { graphic = transform.GetChild(0); }
		graphic.SetParent(null);
	}

	public void Tighten() {
		if (graphic == null) { graphic = transform.GetChild(0); }
		graphic.SetParent(transform);
		graphic.localPosition = Vector3.zero;
	}

	public static void Loosen(Transform t) {
		for (int i = 0; i < t.childCount; ++i) {
			MrV_Mino mm = t.GetChild(i).GetComponent<MrV_Mino>();
			if (mm != null) mm.Loosen();
		}
	}
	public static void Tighten(Transform t) {
		for (int i = 0; i < t.childCount; ++i) {
			MrV_Mino mm = t.GetChild(i).GetComponent<MrV_Mino>();
			if (mm != null) mm.Tighten();
		}
	}

	public void FixedUpdate() {
		Vector3 newPos = GetGridPosition(transform.position) + new Vector3(0.5f, 0.5f, 0);
		Vector3 delta = newPos - targetPosition;
		if (delta != Vector3.zero) {
			needsToShift = true;
			lastPosition = graphic.position;
			targetPosition = newPos;
			timer = 0;
			//NonStandard.Lines.Make("move " + name).Arrow(lastPosition, targetPosition, Color.red, 1f/32);
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
