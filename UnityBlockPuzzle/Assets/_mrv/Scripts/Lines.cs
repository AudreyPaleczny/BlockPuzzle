#define PULSE_COLOR
using System;
using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using NonStandard.NumCs;
#if UNITY_EDITOR
using System.Diagnostics;
#endif

// author: mvaganov@hotmail.com
// license: Copyfree, public domain. This is free code! Great artists, steal this code!
// latest version at: https://pastebin.com/raw/8m69iTut -- last updated (2021/11/19)
namespace NonStandard {
	/// <summary>static functions for Unity's LineRenderer. Creates visualizations for 3D Vector math.
	/// This library isn't optimized for performance, it's built to make math less invisible, even at compiled runtime.
	/// </summary>
	public class Lines : MonoBehaviour {
		/// <summary>
		/// the ends of a line.
		/// Normal is a simple rectangular end
		/// Arrow ends in an arrow head
		/// ArrowBothEnds starts and ends with an arrow head
		/// </summary>
		public enum End { Normal, Arrow, ArrowBothEnds };

		public bool autoParentLinesToGlobalObject = true;

		/// <summary>the dictionary of named lines. This structure allows Lines to create new lines without needing explicit variables</summary>
		private static readonly Dictionary<string, GameObject> NamedObject = new Dictionary<string, GameObject>();
		/// <summary>The singleton instance.</summary>
		private static Lines _instance;

		public const float ARROW_SIZE = 3, LINE_SIZE = 1f / 8, SAME_AS_START_SIZE = -1;

		public static Material _defaultMaterial;
		public static Material DefaultMaterial {
			get {
				if (_defaultMaterial != null) return _defaultMaterial;
				GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
				_defaultMaterial = primitive.GetComponent<MeshRenderer>().sharedMaterial;
				DestroyImmediate(primitive);
				return _defaultMaterial;
			}
		}

		[Tooltip("Used to draw lines. Ideally a white Sprites/Default shader."), SerializeField]
		private Material lineMaterial;
		public static Material LineMaterial {
			get {
				Lines lines = Instance;
				if (lines.lineMaterial != null) return lines.lineMaterial;
				const string colorShaderName = "Sprites/Default";//"Unlit/Color";
				lines.lineMaterial = FindShaderMaterial(colorShaderName);
				return lines.lineMaterial;
			}
		}

		public static Lines Instance {
			get {
				if (_instance) return _instance;
				return _instance = FindComponentInstance<Lines>();
			}
		}

		public static T FindComponentInstance<T>() where T : Component {
			T instance;
			if ((instance = FindObjectOfType(typeof(T)) as T) != null) return instance;
			GameObject g = new GameObject($"<{typeof(T).Name}>");
			instance = g.AddComponent<T>();
			return instance;
		}

		private void Start() {
			if (_instance == null || _instance == this) return;
			Debug.LogWarning("<Lines> should be a singleton. Deleting extra");
			Destroy(this);
		}

		/// <param name="name"></param>
		/// <param name="createIfNotFound">if true, this function will not return null</param>
		/// <returns>a line object with the given name. can return null if no such object has been made yet with this function</returns>
		public static GameObject Get(string name, bool createIfNotFound = false) {
			if ((NamedObject.TryGetValue(name, out GameObject go) && go) || !createIfNotFound) return go;
			go = NamedObject[name] = MakeLineRenderer(ref go).gameObject;
			go.name = name;
			return go;
		}

		/// <summary></summary>
		/// <returns>an unnamed, unmanaged Line object</returns>
		public static Wire MakeWire(string wirename = null) {
			GameObject go = null;
			MakeLineRenderer(ref go);
			if (!string.IsNullOrEmpty(wirename)) { go.name = wirename; }
			Wire line = go.GetComponent<Wire>();
			if (!line) { line = go.AddComponent<Wire>(); line.RefreshSource(); }
			go.layer = LayerMask.NameToLayer("UI");
			return line;
		}

		/// <summary>looks for a line object with the given name and returns it</summary>
		/// <param name="name"></param>
		/// <param name="createIfNotFound"></param>
		/// <returns>a line object with the given name, or null if createIfNotFound is false and the object doesn't exist</returns>
		public static Wire Make(string name, bool createIfNotFound = true) {
			GameObject go = Get(name, createIfNotFound);
			if (go == null) return null;
			Wire line = go.GetComponent<Wire>();
			if (!line) { line = go.AddComponent<Wire>(); line.RefreshSource(); }
			return line;
		}

		/// <summary>
		/// Make the specified Line.
		/// example usage:
		/// <para><code>
		/// /* GameObject forwardLine should be a member variable */
		/// Lines.Make (ref forwardLine, transform.position,
		///             transform.position + transform.forward, Color.blue, 0.1f, 0);
		/// //This makes a long thin triangle, pointing forward.
		/// </code></para>
		/// </summary>
		/// <param name="lineObject">GameObject host of the LineRenderer</param>
		/// <param name="start">Start, an absolute world-space coordinate</param>
		/// <param name="end">End, an absolute world-space coordinate</param>
		/// <param name="color"></param>
		/// <param name="startSize">How wide the line is at the start</param>
		/// <param name="endSize">How wide the line is at the end</param>
		public static LineRenderer Make(ref GameObject lineObject, Vector3 start, Vector3 end,
			Color color = default, float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE) {
			LineRenderer lr = MakeLineRenderer(ref lineObject);
			SetLine(lr, color, startSize, endSize);
			lr.positionCount = 2;
			lr.SetPosition(0, start); lr.SetPosition(1, end);
			return lr;
		}

		/// <summary>Make the specified Line from a list of points</summary>
		/// <returns>The LineRenderer hosting the line</returns>
		/// <param name="lineObject">GameObject host of the LineRenderer</param>
		/// <param name="color">Color of the line</param>
		/// <param name="points">List of absolute world-space coordinates</param>
		/// <param name="pointCount">Number of the points used points list</param>
		/// <param name="startSize">How wide the line is at the start</param>
		/// <param name="endSize">How wide the line is at the end</param>
		public static LineRenderer Make(ref GameObject lineObject, IList<Vector3> points, int pointCount,
			Color color = default, float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE) {
			LineRenderer lr = MakeLineRenderer(ref lineObject);
			return Make(lr, points, pointCount, color, startSize, endSize);
		}

		public static LineRenderer Make(LineRenderer lr, IList<Vector3> points, int pointCount,
			Color color = default, float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE) {
			SetLine(lr, color, startSize, endSize);
			return MakeLine(lr, points, pointCount);
		}

		public static LineRenderer MakeLine(LineRenderer lr, IList<Vector3> points, int pointCount) {
			lr.positionCount = pointCount;
			for (int i = 0; i < pointCount; ++i) { lr.SetPosition(i, points[i]); }
			return lr;
		}

		public static LineRenderer MakeLine(LineRenderer lr, IList<Vector3> points, Color color, float startSize, float endSize, End lineEnds) {
			if (Math3d.EQ2(endSize, SAME_AS_START_SIZE)) { endSize = startSize; }
			if (points == null) { lr = Make(lr, null, 0, color, startSize, endSize); return lr; }
			if (lineEnds == End.Arrow || lineEnds == End.ArrowBothEnds) {
				Keyframe[] keyframes = CalculateArrowKeyframes(points, points.Count, out var line, startSize, endSize);
				lr = MakeArrow(lr, line, line.Length, color, startSize, endSize);
				lr.widthCurve = new AnimationCurve(keyframes);
				if (lineEnds == End.ArrowBothEnds) {
					ReverseLineInternal(ref lr);
					Vector3[] p = new Vector3[lr.positionCount];
					lr.GetPositions(p);
					lr = MakeArrow(lr, p, p.Length, color, endSize, startSize, ARROW_SIZE, lr.widthCurve.keys);
					ReverseLineInternal(ref lr);
				}
			} else {
				lr = Make(lr, points, points.Count, color, startSize, endSize);
				FlattenKeyFrame(lr);
			}
			if (lr.loop && lineEnds != End.Normal) { lr.loop = false; }
			return lr;
		}

		public static void FlattenKeyFrame(LineRenderer lr) {
			AnimationCurve widthCurve = lr.widthCurve;
			Keyframe[] keys = widthCurve.keys;
			if (keys != null && keys.Length > 2) {
				lr.widthCurve = new AnimationCurve(new Keyframe[] { keys[0], keys[keys.Length - 1] });
			}
		}

		public static LineRenderer MakeLineRenderer(ref GameObject lineObject) {
			if (!lineObject) {
				lineObject = new GameObject();
				if (Instance.autoParentLinesToGlobalObject) {
					lineObject.transform.SetParent(_instance.transform);
				}
			}
			return MakeLineRenderer(lineObject);
		}

		public static LineRenderer MakeLineRenderer(GameObject lineObject) {
			LineRenderer lr = lineObject.GetComponent<LineRenderer>();
			if (!lr) { lr = lineObject.AddComponent<LineRenderer>(); }
			return lr;
		}

		public static LineRenderer SetLine(LineRenderer lr, Color color, float startSize, float endSize) {
			lr.startWidth = startSize;
			if (Math3d.EQ2(endSize, SAME_AS_START_SIZE)) { endSize = startSize; }
			lr.endWidth = endSize;
			SetColor(lr, color);
			return lr;
		}

		public static Material FindShaderMaterial(string shaderName) {
			Shader s = Shader.Find(shaderName);
			if (!s) {
				throw new Exception("Missing shader: " + shaderName
					+ ". Please make sure it is in the \"Resources\" folder, "
					+ "or used by at least one other object in the scene. Or, "
					+ " manually assign the line material to a Lines GameObject.");
			}
			return new Material(s);
		}

		public static void SetColor(LineRenderer lr, Color color) {
			bool needsMaterial = lr.material == null || lr.material.name.StartsWith("Default-Material");
			if (needsMaterial) { lr.material = LineMaterial; }
			if (color == default) { color = Color.magenta; }
#if PULSE_COLOR
			long t = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			long duration = 500;
			long secComponent = t % duration;
			float a = Mathf.Abs((2f * secComponent - duration) / duration);
			Color.RGBToHSV(color, out float h, out float s, out float v);
			color = Color.HSVToRGB(h, s + (a * .25f), v + (a * .25f));
#endif
			lr.material.color = color;
		}

		/// <summary>Makes a circle with a 3D line</summary>
		/// <returns>The LineRenderer hosting the line</returns>
		/// <param name="lineObj">GameObject host of the LineRenderer</param>
		/// <param name="color">Color of the line</param>
		/// <param name="center">Absolute world-space 3D coordinate</param>
		/// <param name="normal">Which way the circle is facing</param>
		/// <param name="radius"></param>
		/// <param name="pointCount">How many points to use for the circle. If zero, will do 24*PI*r</param>
		/// <param name="lineSize">The width of the line</param>
		public static LineRenderer MakeCircle(ref GameObject lineObj, Vector3 center, Vector3 normal,
			Color color = default, float radius = 1, int pointCount = 0, float lineSize = LINE_SIZE) {
			Vector3[] points = null;
			Math3d.WriteCircle(ref points, center, normal, radius, pointCount);
			LineRenderer lr = Lines.Make(ref lineObj, points, points.Length, color, lineSize, lineSize);
			lr.loop = true;
			return lr;
		}

		public static LineRenderer MakeSphere(string name, float radius = 1,
			Vector3 center = default, Color color = default, float lineSize = LINE_SIZE) {
			GameObject go = Get(name, true);
			return MakeSphere(ref go, radius, center, color, lineSize);
		}
		/// <returns>a line renderer in the shape of a sphere made of 3 circles, for the x.y.z axis</returns>
		/// <param name="lineObj">Line object.</param>
		/// <param name="radius">Radius.</param>
		/// <param name="center">Center.</param>
		/// <param name="color">Color.</param>
		/// <param name="lineSize">Line size.</param>
		public static LineRenderer MakeSphere(ref GameObject lineObj, float radius = 1,
			Vector3 center = default, Color color = default, float lineSize = LINE_SIZE) {
			Vector3[] circles = new Vector3[24 * 3];
			Math3d.WriteArc(ref circles, 24, Vector3.forward, Vector3.up, 360, center, 24 * 0);
			Math3d.WriteArc(ref circles, 24, Vector3.right, Vector3.up, 360, center, 24 * 1);
			Math3d.WriteArc(ref circles, 24, Vector3.up, Vector3.forward, 360, center, 24 * 2);
			if (Math3d.EQ2(radius, 1)) { for (int i = 0; i < circles.Length; ++i) { circles[i] *= radius; } }
			return Lines.Make(ref lineObj, circles, circles.Length, color, lineSize, lineSize);
		}

		public static LineRenderer MakeBox(ref GameObject lineObj, Vector3 center,
			Vector3 size, Quaternion rotation, Color color = default, float lineSize = LINE_SIZE) {
			Vector3 y = Vector3.up / 2 * size.y;
			Vector3 x = Vector3.right / 2 * size.x;
			Vector3 z = Vector3.forward / 2 * size.z;
			Vector3[] line = new Vector3[] {
				 z+y-x, -z+y-x, -z-y-x, -z-y+x, -z+y+x,  z+y+x,  z-y+x,  z-y-x,
				 z+y-x,  z+y+x,  z-y+x, -z-y+x, -z+y+x, -z+y-x, -z-y-x,  z-y-x
			};
			for (int i = 0; i < line.Length; ++i) { line[i] = rotation * line[i] + center; }
			LineRenderer lr = Make(ref lineObj, line, line.Length, color, lineSize, lineSize);
			lr.numCornerVertices = 4;
			return lr;
		}

		public static LineRenderer MakeMapPin(string name, Color c = default, float size = 1, float lineSize = LINE_SIZE) {
			GameObject go = Get(name, true);
			return MakeMapPin(ref go, c, size, lineSize);
		}
		private static Vector3[] _mapPinPointsBase;
		/// <summary>Draws a "map pin", which shows a visualization for direction and orientation</summary>
		/// <returns>The LineRenderer hosting the map pin line. The LineRenderer's transform can be adjusted!</returns>
		/// <param name="lineObj">Line object.</param>
		/// <param name="c">C: color</param>
		/// <param name="size">Size: radius of the map pin</param>
		/// <param name="lineSize">Line width.</param>
		public static LineRenderer MakeMapPin(ref GameObject lineObj, Color c = default, float size = 1, float lineSize = LINE_SIZE) {
			const float epsilon = 1 / 1024.0f;
			if (_mapPinPointsBase == null) {
				Vector3 pos = Vector3.zero, forward = Vector3.forward * size, right = Vector3.right * size, up = Vector3.up;
				const float startAngle = (360.0f / 4) - (360.0f / 32);
				Vector3 v = Quaternion.AngleAxis(startAngle, up) * forward;
				Math3d.WriteArc(ref _mapPinPointsBase, 32, up, v, 360, pos);
				Vector3 tip = pos + forward * Mathf.Sqrt(2);
				_mapPinPointsBase[0] = _mapPinPointsBase[_mapPinPointsBase.Length - 1];
				int m = (32 * 5 / 8);
				_mapPinPointsBase[++m] = _mapPinPointsBase[m] + (tip - _mapPinPointsBase[m]) * (1 - epsilon);
				_mapPinPointsBase[++m] = tip;
				int n = (32 * 7 / 8) + 1;
				while (n < 32) { _mapPinPointsBase[++m] = _mapPinPointsBase[n++]; }
				Vector3 side = pos + right;
				_mapPinPointsBase[++m] = _mapPinPointsBase[m] + (side - _mapPinPointsBase[m]) * (1 - epsilon);
				_mapPinPointsBase[++m] = pos + right;
				_mapPinPointsBase[++m] = pos + right * epsilon;
				_mapPinPointsBase[++m] = pos;
				_mapPinPointsBase[++m] = pos + up * (size * (1 - epsilon));
				_mapPinPointsBase[++m] = pos + up * size;
			}
			LineRenderer lr = Lines.Make(ref lineObj, _mapPinPointsBase, _mapPinPointsBase.Length, c, lineSize, lineSize);
			lr.useWorldSpace = false;
			return lr;
		}

		public static LineRenderer SetMapPin(string name, Transform t, Color c = default, float size = 1, float lineWidth = LINE_SIZE) {
			GameObject go = Get(name, true);
			return SetMapPin(ref go, t, c, size, lineWidth);
		}
		/// <summary>Draws a "map pin", which shows a visualization for direction and orientation</summary>
		/// <returns>The LineRenderer hosting the map pin line</returns>
		/// <param name="lineObj">Line object.</param>
		/// <param name="t">t: the transform to attach the map pin visualisation to</param>
		/// <param name="c">C: color</param>
		/// <param name="size">Size: radius of the map pin</param>
		/// <param name="lineWidth">Line width.</param>
		public static LineRenderer SetMapPin(ref GameObject lineObj, Transform t, Color c = default, float size = 1, float lineWidth = LINE_SIZE) {
			LineRenderer line_ = MakeMapPin(ref lineObj, c, size, lineWidth);
			Transform transform = line_.transform;
			transform.SetParent(t);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			return line_;
		}


		/// <returns>a line renderer in the shape of a spiraling sphere, spiraling about the Vector3.up axis</returns>
		/// <param name="lineObj">Line object.</param>
		/// <param name="radius">Radius.</param>
		/// <param name="center">Center.</param>
		/// <param name="rotation"></param>
		/// <param name="color">Color.</param>
		/// <param name="lineSize">LineSize.</param>
		public static LineRenderer MakeSpiralSphere(ref GameObject lineObj, float radius = 1,
			Vector3 center = default, Quaternion rotation = default, Color color = default, float lineSize = LINE_SIZE) {
			Vector3[] vertices = Math3d.CreateSpiralSphere(center, radius, rotation, 24, 3);
			return Make(ref lineObj, vertices, vertices.Length, color, lineSize, lineSize);
		}

		public static LineRenderer MakeArrow(ref GameObject lineObject, Vector3 start, Vector3 end,
			Color color = default, float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE, float arrowHeadSize = ARROW_SIZE) {
			return MakeArrow(ref lineObject, new Vector3[] { start, end }, 2, color, startSize, endSize, arrowHeadSize);
		}

		public static LineRenderer MakeArrow(ref GameObject lineObject, IList<Vector3> points, int pointCount,
			Color color = default, float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE,
			float arrowHeadSize = ARROW_SIZE, Keyframe[] lineKeyFrames = null) {
			LineRenderer lr = MakeLineRenderer(ref lineObject);
			return MakeArrow(lr, points, pointCount, color, startSize, endSize, arrowHeadSize, lineKeyFrames);
		}

		public static LineRenderer MakeArrow(LineRenderer lr, IList<Vector3> points, int pointCount,
				Color color = default, float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE,
				float arrowHeadSize = ARROW_SIZE, Keyframe[] lineKeyFrames = null) {
			Keyframe[] keyframes = CalculateArrowKeyframes(points, pointCount, out Vector3[] line, startSize, endSize, arrowHeadSize, lineKeyFrames);
			Make(lr, line, line.Length, color, startSize, endSize);
			lr.widthCurve = new AnimationCurve(keyframes);
			return lr;
		}

		public static Keyframe[] CalculateArrowKeyframes(IList<Vector3> points, int pointCount, out Vector3[] line,
		float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE, float arrowHeadSize = ARROW_SIZE, Keyframe[] lineKeyFrames = null) {
			float arrowSize = endSize * arrowHeadSize;
			int lastGoodIndex = 0;
			Vector3 arrowheadBase = Vector3.zero;
			const float distanceBetweenArrowBaseAndWidePoint = 1.0f / 512;
			Vector3 delta, dir = Vector3.zero;
			// find where, in the list of points, to place the arrowhead
			float dist = 0;
			int lastPoint = pointCount - 1;
			for (int i = lastPoint; i > 0; --i) { // go backwards (from the pointy end)
				float d = Vector3.Distance(points[i], points[i - 1]);
				dist += d;
				// if the arrow direction hasn't been calculated and sufficient distance for the arrowhead has been passed
				if (dir == Vector3.zero && dist >= arrowSize) {
					// calculate w,here the arrowheadBase should be (requires 2 points) based on the direction of this segment
					lastGoodIndex = i - 1;
					delta = points[i] - points[i - 1];
					dir = delta.normalized;
					float extraFromLastGoodIndex = dist - arrowSize;
					arrowheadBase = points[lastGoodIndex] + dir * extraFromLastGoodIndex;
				}
			}
			// if the line is not long enough for an arrow head, make the whole thing an arrowhead
			if (dist <= arrowSize) {
				line = new Vector3[] { points[0], points[lastPoint] };
				return new Keyframe[] { new Keyframe(0, arrowSize), new Keyframe(1, 0) };
			}
			delta = points[lastPoint] - arrowheadBase;
			dir = delta.normalized;
			Vector3 arrowheadWidest = arrowheadBase + dir * (dist * distanceBetweenArrowBaseAndWidePoint);
			line = new Vector3[lastGoodIndex + 4];
			for (int i = 0; i <= lastGoodIndex; i++) {
				line[i] = points[i];
			}
			line[lastGoodIndex + 3] = points[lastPoint];
			line[lastGoodIndex + 2] = arrowheadWidest;
			line[lastGoodIndex + 1] = arrowheadBase;
			Keyframe[] keyframes;
			float arrowHeadBaseStart = 1 - arrowSize / dist;
			float arrowHeadBaseWidest = 1 - (arrowSize / dist - distanceBetweenArrowBaseAndWidePoint);
			if (lineKeyFrames == null) {
				keyframes = new Keyframe[] {
					new Keyframe(0, startSize), new Keyframe(arrowHeadBaseStart, endSize),
					new Keyframe(arrowHeadBaseWidest, arrowSize), new Keyframe(1, 0)
				};
			} else {
				// count how many there are after arrowHeadBaseStart.
				int validCount = lineKeyFrames.Length;
				for (int i = 0; i < lineKeyFrames.Length; ++i) {
					float t = lineKeyFrames[i].time;
					if (t > arrowHeadBaseStart) { validCount = i; break; }
				}
				// those are irrelevant now. they'll be replaced by the 3 extra points
				keyframes = new Keyframe[validCount + 3];
				for (int i = 0; i < validCount; ++i) { keyframes[i] = lineKeyFrames[i]; }
				keyframes[validCount + 0] = new Keyframe(arrowHeadBaseStart, endSize);
				keyframes[validCount + 1] = new Keyframe(arrowHeadBaseWidest, arrowSize);
				keyframes[validCount + 2] = new Keyframe(1, 0);
			}
			return keyframes;
		}

		public static LineRenderer MakeArrowBothEnds(ref GameObject lineObject, Vector3 start, Vector3 end,
			Color color = default, float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE, float arrowHeadSize = ARROW_SIZE) {
			return MakeArrowBothEnds(ref lineObject, new Vector3[] { end, start }, 2, color, startSize, endSize, arrowHeadSize);
		}
		public static LineRenderer MakeArrowBothEnds(ref GameObject lineObject, IList<Vector3> points, int pointCount,
			Color color = default, float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE, float arrowHeadSize = ARROW_SIZE) {
			LineRenderer lr = MakeArrow(ref lineObject, points, pointCount, color, startSize, endSize, arrowHeadSize, null);
			ReverseLineInternal(ref lr);
			Vector3[] p = new Vector3[lr.positionCount];
			lr.GetPositions(p);
			lr = MakeArrow(ref lineObject, p, p.Length, color, endSize, startSize, arrowHeadSize, lr.widthCurve.keys);
			ReverseLineInternal(ref lr);
			return lr;
		}
		public static LineRenderer ReverseLineInternal(ref LineRenderer lr) {
			Vector3[] p = new Vector3[lr.positionCount];
			lr.GetPositions(p);
			Array.Reverse(p);
			lr.SetPositions(p);
			AnimationCurve widthCurve = lr.widthCurve;
			if (widthCurve != null && widthCurve.length > 1) {
				Keyframe[] kf = new Keyframe[widthCurve.keys.Length];
				Keyframe[] okf = widthCurve.keys;
				Array.Copy(okf, kf, okf.Length); //for(int i = 0; i<kf.Length; ++i) { kf[i]=okf[i]; }
				Array.Reverse(kf);
				for (int i = 0; i < kf.Length; ++i) { kf[i].time = 1 - kf[i].time; }
				lr.widthCurve = new AnimationCurve(kf);
			}
			return lr;
		}

		public static LineRenderer MakeArcArrow(ref GameObject lineObj,
			float angle, int pointCount, Vector3 arcPlaneNormal = default, Vector3 firstPoint = default,
			Vector3 center = default, Color color = default, float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE, float arrowHeadSize = ARROW_SIZE) {
			if (arcPlaneNormal == default) { arcPlaneNormal = Vector3.up; }
			if (center == default && firstPoint == default) { firstPoint = Vector3.right; }
			Vector3[] points = null;
			Math3d.WriteArc(ref points, pointCount, arcPlaneNormal, firstPoint, angle, center);
			return MakeArrow(ref lineObj, points, pointCount, color, startSize, endSize, arrowHeadSize);
		}

		public static LineRenderer MakeArcArrowBothEnds(ref GameObject lineObj,
			float angle, int pointCount, Vector3 arcPlaneNormal = default, Vector3 firstPoint = default,
			Vector3 center = default, Color color = default, float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE, float arrowHeadSize = ARROW_SIZE) {
			LineRenderer lr = MakeArcArrow(ref lineObj, angle, pointCount, arcPlaneNormal, firstPoint, center, color, startSize, endSize, arrowHeadSize);
			ReverseLineInternal(ref lr);
			Vector3[] p = new Vector3[lr.positionCount];
			lr.GetPositions(p);
			lr = MakeArrow(ref lineObj, p, p.Length, color, endSize, startSize, arrowHeadSize, lr.widthCurve.keys);
			ReverseLineInternal(ref lr);
			return lr;
		}

		public static LineRenderer MakeArcArrow(ref GameObject lineObject, Vector3 start, Vector3 end, Color color = default, float angle = 90, Vector3 upNormal = default,
			float startSize = LINE_SIZE, float endSize = SAME_AS_START_SIZE, float arrowHeadSize = ARROW_SIZE, int pointCount = 0) {
			Vector3[] arc;
			if (end == start || Mathf.Abs(angle) >= 360) {
				arc = new Vector3[] { start, end };
			} else {
				if (upNormal == default) { upNormal = Vector3.up; }
				if (pointCount == 0) { pointCount = Mathf.Max((int)(angle * 24 / 180) + 1, 2); }
				arc = new Vector3[pointCount];
				Vector3 delta = end - start;
				float dist = delta.magnitude;
				Vector3 dir = delta / dist;
				Vector3 right = Vector3.Cross(upNormal, dir).normalized;
				Math3d.WriteArc(ref arc, pointCount, right, -upNormal, angle);
				Vector3 arcDelta = arc[arc.Length - 1] - arc[0];
				float arcDist = arcDelta.magnitude;
				float angleDiff = Vector3.Angle(arcDelta / arcDist, delta / dist);
				Quaternion turn = Quaternion.AngleAxis(angleDiff, right);
				float ratio = dist / arcDist;
				for (int i = 0; i < arc.Length; ++i) { arc[i] = (turn * arc[i]) * ratio; }
				Vector3 offset = start - arc[0];
				for (int i = 0; i < arc.Length; ++i) { arc[i] += offset; }
			}
			return MakeArrow(ref lineObject, arc, arc.Length, color, startSize, endSize, arrowHeadSize);
		}

		public static void MakeQuaternion(ref GameObject axisObj, Wire[] childWire, Vector3 axis, float angle,
			Vector3 position = default, Color color = default, Quaternion orientation = default,
			int arcPoints = -1, float lineSize = LINE_SIZE, float arrowHeadSize = ARROW_SIZE, Vector3[] startPoint = null) {
			if (childWire.Length != startPoint.Length) { throw new Exception("childWire and startPoint should be parallel arrays"); }
			while (angle >= 180) { angle -= 360; }
			while (angle < -180) { angle += 360; }
			Vector3 axisRotated = orientation * axis;
			MakeArrow(ref axisObj, position - axisRotated, position + axisRotated, color, lineSize, lineSize, arrowHeadSize);
			for (int i = 0; i < childWire.Length; ++i) {
				Wire aObj = childWire[i];
				aObj.Arc(angle, axisRotated, startPoint[i], position, color, Lines.End.Arrow, arcPoints, lineSize);
				//MakeArcArrow(ref aObj, angle, arcPoints, axisRotated, startPoint[i], position, color, lineSize, lineSize, arrowHeadSize);
				childWire[i] = aObj;
			}
		}

		internal static int _CartesianPlaneChildCount(float extents, float increment, out int linesPerDomainHalf) {
			linesPerDomainHalf = (int)(extents / increment);
			if (Mathf.Abs(linesPerDomainHalf - (extents / increment)) < Math3d.TOLERANCE) --linesPerDomainHalf;
			return 2 + linesPerDomainHalf * 4;
		}
		public static void MakeCartesianPlane(Vector3 center, Vector3 up, Vector3 right, Wire[] wires, Color color = default, float lineWidth = LINE_SIZE,
			float size = .5f, float increments = 0.125f, Vector3 offset = default) {
			// prep data structures
			int wireCount = _CartesianPlaneChildCount(size, increments, out int thinLines);
			while (wires.Length < wireCount) { throw new Exception($"can't make {wireCount} wires with {wires.Length} slots"); }
			Vector3[] endPoints = new Vector3[2];
			// prep math
			Vector3 minX = right * -size, maxX = right * size;
			Vector3 minY = up * -size, maxY = up * size;
			Vector3 p = center + offset;
			int index = 1;
			float thinLineWidth = lineWidth / 4;
			// draw the X and Y axis
			endPoints[0] = p + minX; endPoints[1] = p + maxX; wires[0].Line(endPoints, color, End.Arrow, lineWidth);
			endPoints[0] = p + minY; endPoints[1] = p + maxY; wires[1].Line(endPoints, color, End.Arrow, lineWidth);
			// positiveY
			for (int i = 0; i < thinLines; ++i) {
				Vector3 delta = up * (increments * (i + 1));
				endPoints[0] = p + minX + delta; endPoints[1] = p + maxX + delta;
				wires[++index].Line(endPoints, color, End.Normal, thinLineWidth);
			}
			// negativeY
			for (int i = 0; i < thinLines; ++i) {
				Vector3 delta = -up * (increments * (i + 1));
				endPoints[0] = p + minX + delta; endPoints[1] = p + maxX + delta;
				wires[++index].Line(endPoints, color, End.Normal, thinLineWidth);
			}
			// positiveX
			for (int i = 0; i < thinLines; ++i) {
				Vector3 delta = right * (increments * (i + 1));
				endPoints[0] = p + minY + delta; endPoints[1] = p + maxY + delta;
				wires[++index].Line(endPoints, color, End.Normal, thinLineWidth);
			}
			// negativeX
			for (int i = 0; i < thinLines; ++i) {
				Vector3 delta = -right * (increments * (i + 1));
				endPoints[0] = p + minY + delta; endPoints[1] = p + maxY + delta;
				wires[++index].Line(endPoints, color, End.Normal, thinLineWidth);
			}
		}

		public static void WriteRectangle(Vector3[] out_corner, Vector3 origin, Quaternion rotation, Vector3 halfSize, Vector2 position2D) {
			out_corner[0] = (position2D + new Vector2(-halfSize.x, halfSize.y));
			out_corner[1] = (position2D + new Vector2(halfSize.x, halfSize.y));
			out_corner[2] = (position2D + new Vector2(halfSize.x, -halfSize.y));
			out_corner[3] = (position2D + new Vector2(-halfSize.x, -halfSize.y));
			for (int i = 0; i < 4; ++i) { out_corner[i] = rotation * out_corner[i] + origin; }
		}
		public static void MakeRectangle(Wire[] wires, Vector3 origin, Vector2 position2D, Vector2 halfSize, float lineSize, Color a_color, Quaternion rotation) {
			Vector3[] corners = new Vector3[4];
			WriteRectangle(corners, origin, rotation, halfSize, position2D);
			for (int i = 0; i < corners.Length; ++i) {
				Vector3 a = corners[i];
				Vector3 b = corners[(i + 1) % corners.Length];
				wires[i].Line(a, b, a_color, lineSize).NumCapVertices = 4;
			}
		}

		/// <param name="rectTransform">rectangle to draw on. should have RawImage (or no Renderer at all)</param>
		public static Texture2D GetRawImageTexture(RectTransform rectTransform) {
			UnityEngine.UI.RawImage rImg = rectTransform.GetComponent<UnityEngine.UI.RawImage>();
			if (rImg == null) { rImg = rectTransform.gameObject.AddComponent<UnityEngine.UI.RawImage>(); }
			if (rImg == null) { throw new System.Exception("unable to create a RawImage on " + rectTransform.name + ", does it already have another renderer?"); }
			Texture2D img = rImg.texture as Texture2D;
			if (img == null) {
				Rect r = rectTransform.rect;
				img = new Texture2D((int)r.width, (int)r.height);
				img.SetPixels32(0, 0, (int)r.width, (int)r.height, new Color32[(int)(r.width * r.height)]); // set pixels to the default color, which is clear
				rImg.texture = img;
			}
			return img;
		}

		/// <param name="rectTransform">rectangle to draw on. should have RawImage (or no Renderer at all)</param>
		/// <param name="start">(0,0) is lower left</param>
		/// <param name="end"></param>
		/// <param name="color"></param>
		public static void DrawLine(RectTransform rectTransform, Vector2 start, Vector2 end, Color color, bool apply = true) {
			DrawLine(rectTransform, (int)start.x, (int)start.y, (int)end.x, (int)end.y, color, apply);
		}

		/// <param name="rectTransform">rectangle to draw on. should have RawImage (or no Renderer at all)</param>
		/// <param name="x0">0 is left</param>
		/// <param name="y0">0 is bottom</param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="col"></param>
		public static void DrawLine(RectTransform rectTransform, int x0, int y0, int x1, int y1, Color col, bool apply = true) {
			Texture2D img = GetRawImageTexture(rectTransform);
			DrawLine(img, x0, y0, x1, y1, col);
			if (apply) img.Apply();
		}
		public static void DrawAABB(RectTransform rectTransform, Vector2 p0, Vector2 p1, Color col, bool apply = true) {
			DrawAABB(rectTransform, (int)p0.x, (int)p0.y, (int)p1.x, (int)p1.y, col, apply);
		}
		public static void DrawAABB(RectTransform rectTransform, int x0, int y0, int x1, int y1, Color col, bool apply = true) {
			Texture2D img = GetRawImageTexture(rectTransform);
			DrawLine(img, x0, y0, x0, y1, col);
			DrawLine(img, x0, y1, x1, y1, col);
			DrawLine(img, x1, y0, x1, y1, col);
			DrawLine(img, x0, y0, x1, y0, col);
			if (apply) img.Apply();
		}

		/// <summary>draws an un-aliased single-pixel line on the given texture with the given color</summary>ne
		/// <param name="texture"></param>
		/// <param name="x0">0 is left</param>
		/// <param name="y0">0 is bottom</param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="color"></param>
		public static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, Color color) {
			int dy = y1 - y0;
			int dx = x1 - x0;
			int stepY, stepX;
			if (dy < 0) { dy = -dy; stepY = -1; } else { stepY = 1; }
			if (dx < 0) { dx = -dx; stepX = -1; } else { stepX = 1; }
			dy <<= 1;
			dx <<= 1;
			float fraction;
			texture.SetPixel(x0, y0, color);
			if (dx > dy) {
				fraction = dy - (dx >> 1);
				while (Mathf.Abs(x0 - x1) > 1) {
					if (fraction >= 0) {
						y0 += stepY;
						fraction -= dx;
					}
					x0 += stepX;
					fraction += dy;
					texture.SetPixel(x0, y0, color);
				}
			} else {
				fraction = dx - (dy >> 1);
				while (Mathf.Abs(y0 - y1) > 1) {
					if (fraction >= 0) {
						x0 += stepX;
						fraction -= dy;
					}
					y0 += stepY;
					fraction += dx;
					texture.SetPixel(x0, y0, color);
				}
			}
		}
	}
}

namespace NonStandard.NumCs {
	public static class Math3d {

		/// <summary>
		/// how close two floating point values need to be before they are considered equal in this library
		/// </summary>
		public const float TOLERANCE = 1f / (1 << 23); // one sixteen-millionth

		/// <summary>
		/// used to check equality of two floats that are not expected to be assigned as powers of 2
		/// </summary>
		/// <param name="delta">the difference between two floats</param>
		/// <returns></returns>
		public static bool EQ(float delta) { return Mathf.Abs(delta) < Math3d.TOLERANCE; }

		/// <summary>
		/// intended for use when comparing whole numbers or fractional powers of 2
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool EQ2(float a, float b) {
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			return a == b;
		}

		public static Vector3 GetForwardVector(Quaternion q) {
			return new Vector3(2 * (q.x * q.z + q.w * q.y), 2 * (q.y * q.z + q.w * q.x), 1 - 2 * (q.x * q.x + q.y * q.y));
		}
		public static Vector3 GetUpVector(Quaternion q) {
			return new Vector3(2 * (q.x * q.y + q.w * q.z), 1 - 2 * (q.x * q.x + q.z * q.z), 2 * (q.y * q.z + q.w * q.x));
		}
		public static Vector3 GetRightVector(Quaternion q) {
			return new Vector3(1 - 2 * (q.y * q.y + q.z * q.z), 2 * (q.x * q.y + q.w * q.z), 2 * (q.x * q.z + q.w * q.y));
		}

		/// <summary>Write 2D arc in 3D space, into given Vector3 array</summary>
		/// <param name="points">Will host the list of coordinates</param>
		/// <param name="pointCount">How many vertices to make &gt; 1</param>
		/// <param name="normal">The surface-normal of the arc's plane</param>
		/// <param name="firstPoint">Arc start, rotate about Vector3.zero</param>
		/// <param name="angle">2D angle. Tip: Vector3.Angle(v1, v2)</param>
		/// <param name="offset">How to translate the arc</param>
		/// <param name="startIndex"></param>
		public static void WriteArc(ref Vector3[] points, int pointCount,
			Vector3 normal, Vector3 firstPoint, float angle = 360, Vector3 offset = default, int startIndex = 0) {
			if (pointCount < 0) {
				pointCount = (int)Mathf.Abs(24 * angle / 180f) + 1;
			}
			if (pointCount < 0 || pointCount >= 32767) { throw new Exception($"bad point count value: {pointCount}"); }
			if (points == null) { points = new Vector3[pointCount]; }
			if (startIndex >= points.Length) return;
			points[startIndex] = firstPoint;
			Quaternion q = Quaternion.AngleAxis(angle / (pointCount - 1), normal);
			for (int i = startIndex + 1; i < startIndex + pointCount; ++i) { points[i] = q * points[i - 1]; }
			if (offset != Vector3.zero)
				for (int i = startIndex; i < startIndex + pointCount; ++i) { points[i] += offset; }
		}

		public static void WriteBezier(IList<Vector3> points, Vector3 start, Vector3 startControl, Vector3 endControl, Vector3 end, int startIndex = 0, int count = -1) {
			if (count < 0) { count = points.Count - startIndex; }
			float num = count - 1;
			for (int i = 0; i < count; ++i) {
				points[i + startIndex] = GetBezierPoint(start, startControl, endControl, end, i / num);
			}
		}

		public static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			t = Mathf.Clamp01(t); float o = 1 - t, tt = t * t, oo = o * o;
			return oo * o * p0 + 3 * oo * t * p1 + 3 * o * tt * p2 + t * tt * p3;
		}

		public static void WriteArcOnSphere(ref Vector3[] points, int pointCount, Vector3 sphereCenter, Vector3 start, Vector3 end) {
			Vector3 axis;
			if (start == -end) {
				axis = (start != Vector3.up && end != Vector3.up) ? Vector3.up : Vector3.right;
			} else {
				axis = Vector3.Cross(start, end).normalized;
			}
			Vector3 a = start - sphereCenter, b = end - sphereCenter;
			float aRad = a.magnitude, bRad = b.magnitude, angle = 0;
			if (EQ2(aRad, 0) && EQ2(bRad, 0)) {
				a /= aRad; b /= bRad;
				angle = Vector3.Angle(a, b);
				if (float.IsNaN(angle)) { angle = 0; }
			}
			WriteArc(ref points, pointCount, axis, a, angle, Vector3.zero);
			float radDelta = bRad - aRad;
			for (int i = 0; i < points.Length; ++i) {
				points[i] = points[i] * ((i * radDelta / points.Length) + aRad);
				points[i] += sphereCenter;
			}
		}

		public static int WriteCircle(ref Vector3[] points, Vector3 center, Vector3 normal, float radius = 1, int pointCount = 0) {
			if (pointCount == 0) {
				pointCount = (int)Mathf.Round(24 * 3.14159f * radius + 0.5f);
				if (points != null) {
					pointCount = Mathf.Min(points.Length, pointCount);
				}
			}
			Vector3 crossDir = (normal == Vector3.up || normal == Vector3.down) ? Vector3.forward : Vector3.up;
			Vector3 r = Vector3.Cross(normal, crossDir).normalized;
			WriteArc(ref points, pointCount, normal, r * radius, 360, center);
			return pointCount;
		}

		/// <example>CreateSpiralSphere(transform.position, 0.5f, transform.up, transform.forward, 16, 8);</example>
		/// <summary>creates a line spiraled onto a sphere</summary>
		/// <param name="center"></param>
		/// <param name="radius"></param>
		/// <param name="rotation"></param>
		/// <param name="sides"></param>
		/// <param name="rotations"></param>
		/// <returns></returns>
		public static Vector3[] CreateSpiralSphere(Vector3 center = default, float radius = 1,
			Quaternion rotation = default, float sides = 12, float rotations = 6) {
			List<Vector3> points = new List<Vector3>(); // List instead of Array because sides and rotations are floats!
			Vector3 axis = Vector3.up;
			Vector3 axisFace = Vector3.right;
			if (EQ2(sides, 0) && EQ2(rotations, 0)) {
				float iter = 0;
				float increment = 1f / (rotations * sides);
				points.Add(center + axis * radius);
				do {
					iter += increment;
					Quaternion faceTurn = Quaternion.AngleAxis(iter * 360 * rotations, axis);
					Vector3 newFace = faceTurn * axisFace;
					Quaternion q = Quaternion.LookRotation(newFace);
					Vector3 right = GetUpVector(q);
					Vector3 r = right * radius;
					q = Quaternion.AngleAxis(iter * 180, newFace);
					r = q * r;
					r = rotation * r;
					Vector3 newPoint = center + r;
					points.Add(newPoint);
				}
				while (iter < 1);
			}
			return points.ToArray();
		}
	}
}

namespace NonStandard {
	/// <summary>cached calculations. used to validate if a line needs to be re-calculated</summary>
	public class Wire : MonoBehaviour {
		public enum Kind { None, Line, Arc, Orbital, SpiralSphere, Box, Quaternion, CartesianPlane, Rectangle, Disabled }
		private Kind _kind;
		private Vector3[] _points;
		private Vector3 _normal;
		private Quaternion _rotation;
		private int _count;
		private float _startSize, _endSize, _angle;
		private Lines.End _lineEnds;
		public LineRenderer lr;
#if UNITY_EDITOR
		/// <summary>
		/// Where the code is that created this <see cref="Wire"/>. Not present in deployed builds.
		/// </summary>
		// ReSharper disable once NotAccessedField.Global
		public string sourceCode;
#endif
		public int NumCapVertices {
			get => lr.numCapVertices;
			set => lr.numCapVertices = value;
		}

		public void RefreshSource() {
#if UNITY_EDITOR
			StackTrace stackTrace = new StackTrace(true);
			StackFrame f = stackTrace.GetFrame(2);
			string path = f.GetFileName();
			if (path == null) return;
			int fileIndex = path.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
			sourceCode = $"{path.Substring(fileIndex + 1)}:{f.GetFileLineNumber().ToString()}";
#endif
		}

		public Kind kind {
			get => _kind;
			set {
				// special cleanup for Quaternions
				if (_kind == Kind.Quaternion && value != Kind.Quaternion) {
					DisposeOfChildWires();
				}
				// special cleanup for CartesianPlanes
				if (_kind == Kind.CartesianPlane && value != Kind.CartesianPlane) {
					DisposeOfChildWires();
				}
				// special cleanup for Rectangles
				if (_kind == Kind.Rectangle && value != Kind.Rectangle) {
					DisposeOfChildWires();
				}
				_kind = value;
			}
		}

		private void DisposeOfChildWires() {
			Wire[] obj = ChildWires(_count, false);
			if (obj != null) {
				Array.ForEach(obj, w => { w.transform.SetParent(null); Destroy(w.gameObject); });
			}
		}

		private static bool SameArrayOfVectors(IList<Vector3> a, IList<Vector3> b) {
			if (ReferenceEquals(a, b)) { return true; }
			if (a == null || b == null || a.Count != b.Count) { return false; }
			for (int i = 0; i < a.Count; ++i) { if (a[i] != b[i]) return false; }
			return true;
		}
		private bool IsLine(IList<Vector3> points, float startSize, float endSize, Lines.End lineEnds) {
			return kind == Kind.Line && SameArrayOfVectors(_points, points)
				&& Math3d.EQ(startSize - _startSize) && Math3d.EQ(endSize - _endSize) && _lineEnds == lineEnds;
		}
		private void SetLine(IList<Vector3> points, float startSize, float endSize, Lines.End lineEnds) {
			kind = Kind.Line;
			if (points != null) {
				_points = new Vector3[points.Count];
				for (int i = 0; i < _points.Length; ++i) { _points[i] = points[i]; }
			}
			//_points = null; // commented this out. was it here for a reason?
			_startSize = startSize; _endSize = endSize; _lineEnds = lineEnds;
		}
		private bool IsArc(Vector3 start, Vector3 normal, Vector3 center, float angle, float startSize, float endSize, Lines.End lineEnds, int pointCount) {
			return kind == Kind.Arc && _points != null && _points.Length == 1 && _points[0] == start && _count == pointCount
				&& _normal == normal && Math3d.EQ(startSize - _startSize) && Math3d.EQ(endSize - _endSize) && _lineEnds == lineEnds
				&& transform.position == center && _normal == normal && Math3d.EQ(_angle - angle);
		}
		private void SetArc(Vector3 start, Vector3 normal, Vector3 center, float angle, float startSize, float endSize, Lines.End lineEnds, int pointCount) {
			kind = Kind.Arc;
			_points = new Vector3[] { start }; _count = pointCount;
			_startSize = startSize; _endSize = endSize; _lineEnds = lineEnds;
			transform.position = center; _normal = normal; _angle = angle;
		}
		private bool IsOrbital(Vector3 start, Vector3 end, Vector3 center, float startSize, float endSize, Lines.End lineEnds, int pointCount) {
			return kind == Kind.Orbital && _points != null && _points.Length == 2 && _count == pointCount
				&& _points[0] == start && _points[1] == end
				&& Math3d.EQ(startSize - _startSize) && Math3d.EQ(endSize - _endSize) && _lineEnds == lineEnds
				&& transform.position == center;
		}
		private void SetOrbital(Vector3 start, Vector3 end, Vector3 center = default, float startSize = Lines.LINE_SIZE, float endSize = Lines.SAME_AS_START_SIZE,
			Lines.End lineEnds = default, int pointCount = -1) {
			kind = Kind.Orbital;
			_points = new Vector3[] { start, end }; _count = pointCount;
			_startSize = startSize; _endSize = endSize; _lineEnds = lineEnds;
			transform.position = center;
		}
		private bool IsSpiralSphere(Vector3 center, float radius, float lineSize, Quaternion rotation) {
			return kind == Kind.SpiralSphere
				&& Math3d.EQ(_startSize - lineSize) && Math3d.EQ(_endSize - lineSize)
				&& transform.position == center && Math3d.EQ(_angle - radius)
				&& (_rotation.Equals(rotation) || _rotation == rotation);
		}
		private void SetSpiralSphere(Vector3 center, float radius, float lineSize, Quaternion rotation) {
			kind = Kind.SpiralSphere;
			_startSize = _endSize = lineSize;
			transform.position = center; _angle = radius; _rotation = rotation;
		}
		private bool IsBox(Vector3 center, Vector3 size, Quaternion rotation, float lineSize) {
			Transform t = transform;
			return kind == Kind.Box
				&& Math3d.EQ(_startSize - lineSize) && Math3d.EQ(_endSize - lineSize)
				&& t.position == center
				&& t.localScale == size && t.rotation == rotation;
		}
		private void SetBox(Vector3 center, Vector3 size, Quaternion rotation, float lineSize) {
			Transform t = transform;
			kind = Kind.Box;
			_startSize = _endSize = lineSize;
			t.position = center;
			t.localScale = size;
			t.rotation = rotation;
		}
		private bool IsQuaternion(float an, Vector3 ax, Vector3 position, Vector3[] startPoints, Quaternion orientation, float lineSize) {
			return kind == Kind.Quaternion && SameArrayOfVectors(_points, startPoints)
				&& Math3d.EQ(_startSize - lineSize) && Math3d.EQ(_endSize - lineSize)
				&& transform.position == position && _normal == ax && Math3d.EQ(_angle - an) && _count == startPoints.Length
				&& (_rotation.Equals(orientation) || _rotation == orientation); // quaternions can't easily be tested for equality because of floating point errors
		}
		private void SetQuaternion(float an, Vector3 ax, Vector3 position, Vector3[] startPoints, Quaternion orientation, float lineSize) {
			kind = Kind.Quaternion;
			if (ReferenceEquals(startPoints, DefaultQuaternionVisualizationPoints)) {
				_points = DefaultQuaternionVisualizationPoints;
			} else {
				_points = new Vector3[startPoints.Length]; Array.Copy(startPoints, _points, startPoints.Length);
			}
			_startSize = _endSize = lineSize;
			transform.position = position; _normal = ax; _angle = an; _count = startPoints.Length;
			_rotation = orientation;
		}
		private bool IsCartesianPlane(Vector3 center, Quaternion rotation, float lineSize, float extents, float increment) {
			return kind == Kind.CartesianPlane && Math3d.EQ(_startSize - extents) && Math3d.EQ(_endSize - lineSize) && Math3d.EQ(_angle - increment) && (_rotation.Equals(rotation) || _rotation == rotation) && transform.position == center;
		}
		private void SetCartesianPlane(Vector3 center, Quaternion rotation, float lineSize, float extents, float increment) {
			kind = Kind.CartesianPlane; _startSize = extents; _endSize = lineSize; _angle = increment;
			_rotation = rotation; transform.position = center;
			_count = Lines._CartesianPlaneChildCount(extents, increment, out _);
		}
		private bool IsRectangle(Vector3 origin, Vector2 offset2d, Vector2 halfSize, float lineSize, Quaternion rotation) {
			return kind == Kind.Rectangle && origin == transform.position && Math3d.EQ(_startSize - lineSize) && (_rotation.Equals(rotation) || _rotation == rotation) && Math3d.EQ(_normal.x - offset2d.x) && Math3d.EQ(_normal.y - offset2d.y) && Math3d.EQ(_normal.z - halfSize.x) && Math3d.EQ(_endSize - halfSize.y);
		}
		private void SetRectangle(Vector3 origin, Vector2 offset2d, Vector2 halfSize, float lineSize, Quaternion rotation) {
			kind = Kind.Rectangle; transform.position = origin; _startSize = lineSize; _rotation = rotation;
			_normal.x = offset2d.x; _normal.y = offset2d.y; _normal.z = halfSize.x; _endSize = halfSize.y; _count = 4;
		}

		public Wire Line(Vector3 start, Vector3 end, Color color = default, float startSize = Lines.LINE_SIZE, float endSize = Lines.SAME_AS_START_SIZE) {
			return Line(new Vector3[] { start, end }, color, Lines.End.Normal, startSize, endSize);
		}
		public Wire Arrow(Vector3 start, Vector3 end, Color color = default, float startSize = Lines.LINE_SIZE, float endSize = Lines.SAME_AS_START_SIZE) {
			return Line(new Vector3[] { start, end }, color, Lines.End.Arrow, startSize, endSize);
		}
		public Wire Arrow(Vector3 vector, Color color = default, float startSize = Lines.LINE_SIZE, float endSize = Lines.SAME_AS_START_SIZE) {
			return Line(new Vector3[] { Vector3.zero, vector }, color, Lines.End.Arrow, startSize, endSize);
		}
		public Wire Arrow(Ray ray, Color color = default, float startSize = Lines.LINE_SIZE, float endSize = Lines.SAME_AS_START_SIZE) {
			return Line(new Vector3[] { ray.origin, ray.origin + ray.direction }, color, Lines.End.Arrow, startSize, endSize);
		}
		public Wire Bezier(Vector3 start, Vector3 startControl, Vector3 endControl, Vector3 end, Color color = default, Lines.End cap = Lines.End.Normal, float startSize = Lines.LINE_SIZE, int bezierPointCount = 25, float endSize = Lines.SAME_AS_START_SIZE) {
			Vector3[] bezier = new Vector3[bezierPointCount];
			Math3d.WriteBezier(bezier, start, startControl, endControl, end);
			return Line(bezier, color, cap, startSize, endSize);
		}
		public Wire Line(Vector3 vector, Color color = default, float startSize = Lines.LINE_SIZE, float endSize = Lines.SAME_AS_START_SIZE) {
			return Line(new Vector3[] { Vector3.zero, vector }, color, Lines.End.Normal, startSize, endSize);
		}
		public Wire Line(IList<Vector3> points, Color color = default, Lines.End lineEnds = default, float startSize = Lines.LINE_SIZE, float endSize = Lines.SAME_AS_START_SIZE) {
			if (!IsLine(points, startSize, endSize, lineEnds)) {
				SetLine(points, startSize, endSize, lineEnds);
				if (!lr) { lr = Lines.MakeLineRenderer(gameObject); }
				lr = Lines.MakeLine(lr, points, color, startSize, endSize, lineEnds);
			} //else { Debug.Log("don't need to recalculate line "+name); }
			if (lr) { Lines.SetColor(lr, color); }
			return this;
		}
		public Wire Arc(float angle, Vector3 normal, Vector3 firstPoint, Vector3 center = default, Color color = default,
			Lines.End lineEnds = default, int pointCount = -1, float startSize = Lines.LINE_SIZE, float endSize = Lines.SAME_AS_START_SIZE) {
			if (pointCount < 0) { pointCount = (int)(24 * angle / 180f) + 1; }
			if (!IsArc(firstPoint, normal, center, angle, startSize, endSize, Lines.End.Normal, pointCount)) {
				SetArc(firstPoint, normal, center, angle, startSize, endSize, Lines.End.Normal, pointCount);
				if (!lr) { lr = Lines.MakeLineRenderer(gameObject); }
				Vector3[] linePoints = null;
				Math3d.WriteArc(ref linePoints, pointCount, normal, firstPoint, angle, center);
				lr = Lines.MakeLine(lr, linePoints, color, startSize, endSize, lineEnds);
			} //else { Debug.Log("don't need to recalculate arc "+name);  }
			if (Math3d.EQ2(angle, 360)) { lr.loop = true; }
			Lines.SetColor(lr, color);
			return this;
		}
		public Wire Circle(Vector3 center = default, Vector3 normal = default, Color color = default,
			float radius = 1, float lineSize = Lines.LINE_SIZE, int pointCount = -1) {
			if (Math3d.EQ2(radius, 0)) { return Line(null, color, Lines.End.Normal, lineSize, lineSize); }
			if (normal == default) { normal = Vector3.up; }
			Vector3 firstPoint = Vector3.zero;
			if (kind == Kind.Arc && this._normal == normal && _points != null && _points.Length > 0) {
				float firstRad = _points[0].magnitude;
				if (Math3d.EQ2(firstRad, radius)) {
					firstPoint = _points[0];
				} else {
					firstPoint = _points[0] * (radius / firstRad);
				}
			}
			if (firstPoint == Vector3.zero) {
				firstPoint = Vector3.right;
				if (normal != Vector3.up && normal != Vector3.forward && normal != Vector3.back) {
					firstPoint = Vector3.Cross(normal, Vector3.forward).normalized;
				}
				firstPoint *= radius;
			}
			return Arc(360, normal, firstPoint, center, color, Lines.End.Normal, pointCount, lineSize, lineSize);
		}

		/// <summary>
		/// draw line that orbits a sphere with the given center, from the given start to the given end
		/// </summary>
		/// <param name="sphereCenter"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="color"></param>
		/// <param name="lineEnds"></param>
		/// <param name="startSize"></param>
		/// <param name="endSize"></param>
		/// <param name="pointCount"></param>
		/// <returns></returns>
		public Wire Orbital(Vector3 sphereCenter, Vector3 start, Vector3 end,
			Color color = default, Lines.End lineEnds = default, float startSize = Lines.LINE_SIZE, float endSize = Lines.SAME_AS_START_SIZE, int pointCount = -1) {
			if (!IsOrbital(start, end, sphereCenter, startSize, endSize, lineEnds, pointCount)) {
				SetOrbital(start, end, sphereCenter, startSize, endSize, lineEnds, pointCount);
				Vector3[] linePoints = null;
				Math3d.WriteArcOnSphere(ref linePoints, pointCount, sphereCenter, start, end);
				if (!lr) { lr = Lines.MakeLineRenderer(gameObject); }
				lr = Lines.MakeLine(lr, linePoints, color, startSize, endSize, lineEnds);
			} //else { Debug.Log("don't need to recalculate orbital " + name); }
			Lines.SetColor(lr, color);
			return this;
		}
		public Wire SpiralSphere(Color color = default, Vector3 center = default, float radius = 1, Quaternion rotation = default, float lineSize = Lines.LINE_SIZE) {
			GameObject go = gameObject;
			if (!IsSpiralSphere(center, radius, lineSize, rotation)) {
				SetSpiralSphere(center, radius, lineSize, rotation);
				lr = Lines.MakeSpiralSphere(ref go, radius, center, rotation, color, lineSize);
			} //else { Debug.Log("don't need to recalculate spiral sphere " + name); }
			Lines.SetColor(lr, color);
			return this;
		}
		public Wire Box(Vector3 size, Vector3 center = default, Quaternion rotation = default, Color color = default, float lineSize = Lines.LINE_SIZE) {
			GameObject go = gameObject;
			if (!IsBox(center, size, rotation, lineSize)) {
				SetBox(center, size, rotation, lineSize);
				lr = Lines.MakeBox(ref go, center, size, rotation, color, lineSize);
			} //else { Debug.Log("don't need to recalculate box " + name); }
			Lines.SetColor(lr, color);
			return this;
		}
		private static readonly Vector3[] DefaultQuaternionVisualizationPoints = new Vector3[] { Vector3.forward, Vector3.up };
		public Wire Quaternion(Quaternion q, Color color, Vector3 position = default, Vector3[] startPoints = null,
			Quaternion orientation = default, int arcPoints = -1, float lineSize = Lines.LINE_SIZE) {
			GameObject go = gameObject;
			q.ToAngleAxis(out float an, out Vector3 ax);
			if (startPoints == null) { startPoints = DefaultQuaternionVisualizationPoints; }
			if (!IsQuaternion(an, ax, position, startPoints, orientation, lineSize)) {
				SetQuaternion(an, ax, position, startPoints, orientation, lineSize);
				Wire[] childWires = ChildWires(startPoints.Length, true);
				Lines.MakeQuaternion(ref go, childWires, ax, an, position, color, orientation, arcPoints, lineSize, Lines.ARROW_SIZE, startPoints);
				lr = go.GetComponent<LineRenderer>();
			} //else { Debug.Log("don't need to recalculate quaternion " + name); }
			Lines.SetColor(lr, color);
			return this;
		}
		private Wire[] ChildWires(int objectCount, bool createIfNoneExist) {
			Wire[] wireObjs = null;
			const string _name = "__";
			if (transform.childCount >= objectCount) {
				int childrenWithWire = 0;
				Transform[] children = new Transform[transform.childCount];
				for (int i = 0; i < children.Length; ++i) { children[i] = transform.GetChild(i); }
				Array.ForEach(children, (child) => {
					if (child.name.Contains(_name) && child.GetComponent<Wire>() != null) { ++childrenWithWire; }
				});
				if (childrenWithWire >= objectCount) {
					wireObjs = new Wire[objectCount];
					int validLine = 0;
					for (int i = 0; i < children.Length && validLine < wireObjs.Length; ++i) {
						Wire w;
						if (children[i].name.Contains(_name) && (w = children[i].GetComponent<Wire>()) != null)
							wireObjs[validLine++] = w;
					}
				}
			}
			if (wireObjs == null && createIfNoneExist) {
				wireObjs = new Wire[objectCount];
				for (int i = 0; i < wireObjs.Length; ++i) {
					Wire wireObject = Lines.MakeWire();
					wireObject.name = _name + name + i;
					wireObject.transform.SetParent(transform);
					wireObjs[i] = wireObject;
				}
			}
			return wireObjs;
		}
		public Wire CartesianPlane(Vector3 center, Quaternion rotation, Color color = default, float lineSize = Lines.LINE_SIZE, float extents = 1, float increment = 0.25f) {
			bool colorIsSet = false;
			if (!IsCartesianPlane(center, rotation, lineSize, extents, increment)) {
				SetCartesianPlane(center, rotation, lineSize, extents, increment);
				Vector3 up = rotation * Vector3.up;
				Vector3 right = rotation * Vector3.right;
				Wire[] wires = ChildWires(_count, true);
				Lines.MakeCartesianPlane(center, up, right, wires, color, lineSize, extents, increment);
				colorIsSet = true;
				Vector3 normal = Vector3.Cross(right, up).normalized;
				if (!lr) { lr = Lines.MakeLineRenderer(gameObject); }
				Vector3[] points = new Vector3[] { center, center + normal * increment };
				lr = Lines.MakeLine(lr, points, color, lineSize, lineSize, Lines.End.Arrow);
			} //else { Debug.Log("don't need to recalculate quaternion " + name); }
			if (!colorIsSet) {
				Lines.SetColor(lr, color);
				Wire[] wires = ChildWires(_count, true);
				Array.ForEach(wires, w => Lines.SetColor(w.lr, color));
			}
			return this;
		}
		public Wire Rectangle(Vector3 origin, Vector2 halfSize, Color a_color = default, Quaternion rotation = default, Vector2 offset2d = default, float lineSize = Lines.LINE_SIZE) {
			//if(halfSize == default) { halfSize = Vector2.one / 2; }
			bool colorIsSet = false;
			if (!IsRectangle(origin, offset2d, halfSize, lineSize, rotation)) {
				SetRectangle(origin, offset2d, halfSize, lineSize, rotation);
				Wire[] wires = ChildWires(_count, true);
				Lines.MakeRectangle(wires, origin, offset2d, halfSize, lineSize, a_color, rotation);
				colorIsSet = true;
				if (!lr) { lr = Lines.MakeLineRenderer(gameObject); }
				lr.startWidth = lr.endWidth = 0;
			} //else { Debug.Log("don't need to recalculate quaternion " + name); }
			if (!colorIsSet) {
				//SetColor(lr, a_color);
				Wire[] wires = ChildWires(_count, true);
				Array.ForEach(wires, w => Lines.SetColor(w.lr, a_color));
			}
			return this;
		}
	}
}
