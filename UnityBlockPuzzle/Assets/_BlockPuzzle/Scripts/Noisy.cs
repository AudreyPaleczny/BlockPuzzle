using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// author: mvaganov@hotmail.com
// license: Copyfree, public domain.
// latest version at: https://pastebin.com/raw/hGU8et8s -- added: using volume defined in editor by default (2020/11/13)
public class Noisy : MonoBehaviour
{
	public Noise[] noises = new Noise[1];
	public bool removeNoisesWhenDestroyed = true;

	/// <summary>used for getting a random noise from a list of noises, without getting the one that was just played</summary>
	/// <param name="minInclusive"></param>
	/// <param name="maxExclusive"></param>
	/// <param name="andNot"></param>
	/// <returns></returns>
	public static int RandomNumberThatIsnt(int minInclusive, int maxExclusive, int andNot = -1)
	{
		int index = minInclusive;
		if (maxExclusive - minInclusive > 1)
		{
			if (andNot >= minInclusive)
			{
				index = Random.Range(minInclusive, maxExclusive - 1);
				if (index >= andNot)
				{
					index++;
				}
			}
			else
			{
				index = Random.Range(minInclusive, maxExclusive);
			}
		}
		return index;
	}

	[System.Serializable]
	public class Noise
	{
		[Tooltip("you can reference the 'sounds' list of another Noise by using it's name and leaving this Noise's 'sounds' as length zero")]
		public string name;
		[Tooltip("any one of these sounds count as the-sound-to-play (will be randomized at runtime)")]
		public AudioClip[] sounds = new AudioClip[1];
		[Range(0, 1), Tooltip("to keep max volume, leave this at 0 (changes during runtime are fine, OnValidate)")]
		public float volumeReduce = 0f;
		[Tooltip("set the background music to this? (if this is set, none of the other checkboxes below matter)")]
		public bool backgroundMusic = false;
		[ContextMenuItem("advanced keypress settings... (create component)", "CreateOnKeyPress"), Tooltip("play as soon as object starts? (eg: ambient sound/music or instantiated objects with 'birth' sounds)")]
		public bool playOnStart = false;
		[Tooltip("play at max volume and consistent pitch, regardless of distance? (eg: volume changes by distance, doppler effect)")]
		public bool is2D = false;
		[Tooltip("sound should attach to this object and follow it as it moves? (eg: ambient sound/music/dialog that follows a moving agent)")]
		public bool followsObject = false;
		[Tooltip("start the sound again right after it ends? (eg: ambient sound/music)")]
		public bool loop = false;
		[Tooltip("stop the previous-Noisy-with-the-same-Name before playing another one? (eg: character dialog, UI feedback sounds, background music)")]
		public bool onePlayAtATime = false;
		[ContextMenuItem("advanced collision settings... (create component)", "CreateOnCollision"), Tooltip("play when this object collides with something? (eg: adding audio output to Rigidbody collision)")]
		public bool onCollision = false;
		[ContextMenuItem("advanced trigger settings... (create component)", "CreateOnTrigger"), Tooltip("play when an object enters this trigger? (eg: ambient noise, reactions to movement through space)")]
		public bool onTrigger = false;
		[HideInInspector]
		/// The last noise played, used to prevent duplicate repetition
		public int lastNoisePlayed = -1;
		[HideInInspector]
		public AudioSource activeAudioSource;

		/// Plays the sound. Cannot have the sound follow the object because a position is given, not a transform
		/// <returns>The sound.</returns>
		/// <param name="p">where the noise is</param>
		public AudioSource PlaySound(Vector3 p)
		{
			if (!backgroundMusic)
			{
				activeAudioSource = Noisy.PlaySound(GetSoundToPlay(), p, !is2D, loop, onePlayAtATime ? name : null, 1 - volumeReduce);
			}
			else
			{
				activeAudioSource = Noisy.PlayBackgroundMusic(GetSoundToPlay(), 1 - volumeReduce);
			}
			return activeAudioSource;
		}

		public AudioSource PlaySound(Transform t)
		{
			activeAudioSource = PlaySound(t.position);
			if (followsObject) { activeAudioSource.transform.SetParent(t); }
			return activeAudioSource;
		}

		public AudioClip GetSoundToPlay() { return GetSoundToPlay(ref lastNoisePlayed); }

		public AudioClip GetSoundToPlay(ref int indexNotToPlayNext)
		{
			if (sounds != null && sounds.Length > 0)
			{
				indexNotToPlayNext = RandomNumberThatIsnt(0, sounds.Length, indexNotToPlayNext);
				return sounds[indexNotToPlayNext];
			}
			return null;
		}

		/// <summary>comparer, used to sort Noise objects into the list</summary>
		public class Comparer : IComparer<Noise>
		{
			public int Compare(Noise x, Noise y) { return x.name.CompareTo(y.name); }
		}
		public static Comparer compare = new Comparer();
	}

	void Awake()
	{
		// sort noises for faster access later
		System.Array.Sort(noises, Noise.compare);
		// add all named noises to a single static (global) listing, for easy scripted access later
		for (int i = 0; i < noises.Length; ++i)
		{
			if (noises[i].name != null && noises[i].name.Length > 0)
			{
				int index = Global.allNoises.BinarySearch(noises[i], Noise.compare);
				bool isAlreadyKnown = index >= 0;
				bool hasSoundsFilledOut = noises[i].sounds != null && noises[i].sounds.Length > 0;
				if (!isAlreadyKnown && hasSoundsFilledOut)
				{
					Global.allNoises.Insert(~index, noises[i]);
				}
			}
		}
	}

	/// plays the first sound in the noises list
	public void DoActivateTrigger()
	{
		if (noises.Length > 0 && noises[0] != null) { noises[0].PlaySound(transform.position); }
	}

	/// plays the first sound in the noises list
	public void DoDeactivateTrigger()
	{
		if (noises.Length > 0 && noises[0] != null && noises[0].activeAudioSource != null)
		{
			noises[0].activeAudioSource.Stop();
		}
	}

	/// returns the Noise that was created someplace in the scene with the given name
	/// <returns>The sound.</returns>
	/// <param name="name">Name.</param>
	public static Noise GetSound(string name)
	{
		searched.name = name;
		int i = Global.allNoises.BinarySearch(searched, Noise.compare);
		if (i >= 0) { return Global.allNoises[i]; }
		Debug.LogError("Could not find noise named \"" + name + "\". Valid names include:\n\""
			+ string.Join("\", \"", Global.AllNoiseNames) + "\"");
		return null;
	}

	void Start()
	{
		Noise n;
		for (int i = 0; i < noises.Length; ++i)
		{
			n = noises[i];
			// use the global noise catalog if this Noisy hasn't filled in it's named noise.
			if (n.sounds == null || n.sounds.Length == 0)
			{
				Noise existing = GetSound(n.name);
				if (existing != null) { n.sounds = existing.sounds; }
			}
			if (n.playOnStart || n.backgroundMusic)
			{
				n.PlaySound(transform.position);
			}
			if (n.onCollision)
			{
				Noisy.OnCollisionAdvancedSettings oc = CreateHandler<OnCollisionAdvancedSettings>(n.name);
				oc.noise = n;
			}
			if (n.onTrigger)
			{
				Noisy.OnTriggerAdvancedSettings oc = CreateHandler<OnTriggerAdvancedSettings>(n.name);
				oc.noise = n;
			}
		}
	}

	private void OnDestroy() {
		if (!removeNoisesWhenDestroyed) { return; }
		for (int i = 0; i < noises.Length; ++i) {
			int index = Global.allNoises.BinarySearch(noises[i], Noise.compare);
			if (index >= 0) {
				Global.allNoises.RemoveAt(index);
			}
		}
	}

	private static Noise searched = new Noise();
	/// Plays the named sound (as a 2D sound, full volume)
	public static AudioSource PlaySound(string name)
	{
		Noise n = GetSound(name);
		return (n != null) ? n.PlaySound(Vector3.zero) : null;
	}

	public static AudioSource PlaySound(string name, float volume)
	{
		return PlaySound(name, default, false, false, null, volume);
	}

	public static AudioSource PlaySound(string name, Vector3 p, bool is3D, bool isLooped, string soundCategory, float volume)
	{
		Noise n = GetSound(name);
		return (n != null) ? PlaySound(n.GetSoundToPlay(), p, is3D, isLooped, soundCategory, volume) : null;
	}

	public static AudioSource PlaySound(string name, Vector3 p)
	{
		Noise n = GetSound(name);
		return (n != null) ? n.PlaySound(p) : null;
	}

	/// <param name="name"></param>
	/// <param name="t">where to parent the noise (important for 3D sounds)</param>
	/// <returns></returns>
	public static AudioSource PlaySound(string name, Transform t)
	{
		Noise n = GetSound(name);
		return (n != null) ? n.PlaySound(t) : null;
	}

	private static Dictionary<string, AudioSource> s_soundsByCategory = new Dictionary<string, AudioSource>();

	/// Plays the sound.
	/// <returns>Component where the sound is playing from.</returns>
	/// <param name="noise">Noise. returns early if <c>null</c></param>
	/// <param name="p">P. the location to play from. If is3D is false, this parameter is pretty useless.</param>
	/// <param name="is3D">If set to false, sound plays without considering 3D-ness (full volume from anywhere).</param>
	/// <param name="isLooped">If set to <c>true</c> is looped.</param>
	/// <param name="soundCategory">If non-null, prevents multiple sounds with the same soundCategory from playing simultaneously. If null, each instance of the sound will be independent.</param>
	/// <param name="volume"></param>
	public static AudioSource PlaySound(AudioClip noise, Vector3 p, bool is3D, bool isLooped, string soundCategory, float volume)
	{
		if (noise == null) return null;
		AudioSource asrc = null;
		if (soundCategory != null && soundCategory.Length > 0)
		{
			s_soundsByCategory.TryGetValue(soundCategory, out asrc);
		}
		if (asrc == null)
		{
			string noiseName = (soundCategory != null) ? "(" + soundCategory + ")" : "<Noise: " + noise.name + ">";
			GameObject go = new GameObject(noiseName);
			asrc = go.AddComponent<AudioSource>();
			if (soundCategory != null)
			{
				s_soundsByCategory[soundCategory] = asrc;
			}
			asrc.transform.SetParent(Global.Instance().transform);
		}
		else
		{
			asrc.Stop();
		}
		asrc.clip = noise;
		asrc.spatialBlend = is3D ? 1 : 0;
		asrc.transform.position = p;
		if (soundCategory == null && !isLooped)
		{
			Destroy(asrc.gameObject, noise.length); // destroy the noise after it is done playing if not looped
		}
		asrc.loop = isLooped;
		if (volume != asrc.volume) { asrc.volume = volume; }
		asrc.Play();
		return asrc;
	}

	/// convenience method to play background music
	/// <returns>The background music's AudioSource.</returns>
	/// <param name="song">Song.</param>
	/// <param name="volume">Volume.</param>
	public static AudioSource PlayBackgroundMusic(AudioClip song, float volume)
	{
		AudioSource bgMusicPlayer = PlaySound(song, Vector3.zero, false, true, "{background music}", volume);
		return bgMusicPlayer;
	}

	/// creates an accessible listing to all sounds being used by Noisy, visible in the hierarchy & inspector. also handles some static logic.
	public class Global : MonoBehaviour
	{
		/// All Noise objects with unique names and actual data in the 'sounds' array.
		public static List<Noise> allNoises = new List<Noise>();

		private static Noisy.Global instance;
		public static Noisy.Global Instance()
		{
			if (instance == null)
			{
				if ((instance = FindObjectOfType(typeof(Noisy.Global)) as Noisy.Global) == null)
				{
					GameObject g = new GameObject("<" + typeof(Noisy.Global).Name + ">");
					instance = g.AddComponent<Noisy.Global>();
				}
			}
			return instance;
		}
		/// <summary>local members alias showing all noises (can be seen in the inspector, static members cannot)</summary>
		public List<Noise> allTheNoises;

		/// <summary>the names of all of the noises in a List</summary>
		public static List<string> AllNoiseNames { get { return allNoises.ConvertAll(n => n.name); } }

		void Start() { allTheNoises = allNoises; }
	}

	TYPE CreateHandler<TYPE>(string nameOfNoise) where TYPE : NoisyHandler
	{
		if (name != null)
		{
			TYPE[] triggers = GetComponents<TYPE>();
			for (int i = 0; i < triggers.Length; ++i)
			{
				if (triggers[i].advancedNoiseOverride == nameOfNoise)
					return triggers[i];
			}
		}
		return gameObject.AddComponent<TYPE>();
	}

	void CreateOnTrigger() { CreateHandler<OnTriggerAdvancedSettings>(null); }
	void CreateOnCollision() { CreateHandler<OnCollisionAdvancedSettings>(null); }
	void CreateOnKeyPress() { CreateHandler<OnKeyPressAdvancedSettings>(null); }

	public class NoisyHandler : MonoBehaviour
	{
		[Tooltip("if this is set, Noise (below) will be overwritten at runtime by a Noise with this name")]
		public string advancedNoiseOverride;
		public Noise noise;
		[Tooltip("remove this handler after playing the sound once. (eg: one-time acknowledgement)")]
		public bool justOnce;
		protected void NoisyHandlerStart()
		{
			if (advancedNoiseOverride != null && advancedNoiseOverride.Length > 0)
			{
				noise = Noisy.GetSound(advancedNoiseOverride);
			}
		}
		void Start() { NoisyHandlerStart(); }
	}

	public class NoisyObjectInteractHandler : NoisyHandler
	{
		[Tooltip("identify which GameObjects can trigger this. (eg: only player, only certain item)")]
		public string triggersOnlyByTag;
		public bool IsValidTrigger(GameObject go)
		{
			return triggersOnlyByTag == null || triggersOnlyByTag.Length == 0 || go.tag == triggersOnlyByTag;
		}
	}

	public class OnTriggerAdvancedSettings : NoisyObjectInteractHandler
	{
		void OnTriggerEnter(Collider c)
		{
			if (!IsValidTrigger(c.gameObject)) return;
			if (noise.followsObject)
			{
				noise.PlaySound(transform);
				if (justOnce) Destroy(this);
			}
			else
			{
				noise.PlaySound(c.transform.position);
			}
		}
	}

	public class OnCollisionAdvancedSettings : NoisyObjectInteractHandler
	{
		void OnCollisionEnter(Collision c)
		{
			if (!IsValidTrigger(c.gameObject)) return;
			if (noise.followsObject)
			{
				noise.PlaySound(transform);
				if (justOnce) Destroy(this);
			}
			else
			{
				noise.PlaySound(c.contacts[0].point);
			}
		}
	}

	public class OnKeyPressAdvancedSettings : NoisyHandler
	{
		public KeyCode key = KeyCode.None;
		public enum KeyEvent { press, release, hold };
		public KeyEvent eventType = KeyEvent.press;
		public bool IsTriggered()
		{
			switch (eventType)
			{
				case KeyEvent.press: return Input.GetKeyDown(key);
				case KeyEvent.release: return Input.GetKeyUp(key);
				case KeyEvent.hold: return Input.GetKey(key);
			}
			return false;
		}
		void Start()
		{
			NoisyHandlerStart();
			if (noise.sounds == null || noise.sounds.Length == 0)
			{
				Noisy n = GetComponent<Noisy>();
				if (n != null && n.noises != null && n.noises.Length > 0)
				{
					this.noise = n.noises[0];
				}
			}
		}
		void Update()
		{
			if (IsTriggered())
			{
				noise.PlaySound(transform);
				if (justOnce) Destroy(this);
			}
		}
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (noises == null) return;
		for (int i = 0; i < noises.Length; ++i)
		{
			Noise n = noises[i];
			if (n != null && n.activeAudioSource != null) { n.activeAudioSource.volume = 1 - n.volumeReduce; }
		}
	}
#endif
}