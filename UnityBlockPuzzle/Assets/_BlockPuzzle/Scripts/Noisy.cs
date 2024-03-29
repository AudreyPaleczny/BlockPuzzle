using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// author: mvaganov@hotmail.com
// license: Copyfree, Unlicense, public domain.

/// <summary>
/// puts sounds into a globally accessible space, accessible with <see cref="Noisy.PlaySound(string)"/>.
/// the most recent noise groups of a given name get priority.
/// removing an object with a Noisy removes it from the global space, which may reveal a previously added named noise.
/// enabled workflow: default noises can load early, be overriden with new noisy, and revert if the noisy leaves.
/// </summary>
public class Noisy : MonoBehaviour {
	public Noise[] noises = new Noise[1];
	public bool removeNoisesWhenDestroyed = true;

	/// <summary>used for getting a random noise from a list of noises, without getting the one that was just played</summary>
	/// <param name="minInclusive"></param>
	/// <param name="maxExclusive"></param>
	/// <param name="andNot"></param>
	/// <returns></returns>
	public static int RandomNumberThatIsnt(int minInclusive, int maxExclusive, int andNot = -1) {
		int index = minInclusive;
		if (maxExclusive - minInclusive > 1) {
			if (andNot >= minInclusive && andNot < maxExclusive) {
				index = Random.Range(minInclusive, maxExclusive - 1);
				if (index >= andNot) { index++; }
			} else {
				index = Random.Range(minInclusive, maxExclusive);
			}
		}
		return index;
	}

	[System.Serializable] public class Noise {
		[Tooltip("global name for a sound. if sound is in another noisy, a duplicate name will reference it")]
		public string name;
		[Tooltip("sounds will be randomized from this list at runtime. can be empty if other noisy's have it")]
		public AudioClip[] sounds = new AudioClip[1];
		[Range(0, 1), Tooltip("to keep max volume, leave this at 0 (changes during runtime are fine, OnValidate)")]
		public float volume = 1;
		[Tooltip("set the background music to this? (if this is set, none of the other checkboxes below matter)")]
		public bool backgroundMusic = false;
		[ContextMenuItem("advanced keypress settings... (create component)", nameof(CreateOnKeyPress)),
		 Tooltip("play as soon as object starts? (eg: ambient sound/music or instantiated objects with 'birth' sounds)")]
		public bool playOnStart = false;
		[Tooltip("play at max volume and consistent pitch, regardless of distance? (eg: volume changes by distance, doppler effect)")]
		public bool is2D = false;
		[Tooltip("sound should attach to this object and follow it as it moves? (eg: ambient sound/music/dialog that follows a moving agent)")]
		public bool followsObject = false;
		[Tooltip("start the sound again right after it ends? (eg: ambient sound/music)")]
		public bool loop = false;
		[Tooltip("stop the previous-Noisy-with-the-same-Name before playing another one? (eg: character sound effects, UI feedback sounds, background music)")]
		public bool onePlayAtATime = false;
		[ContextMenuItem("advanced collision settings... (create component)", nameof(CreateOnCollision)),
		 Tooltip("play when this object collides with something? (eg: adding audio output to Rigidbody collision)")]
		public bool onCollision = false;
		[ContextMenuItem("advanced trigger settings... (create component)", nameof(CreateOnTrigger)),
		 Tooltip("play when an object enters this trigger? (eg: ambient noise, reactions to movement through space)")]
		public bool onTrigger = false;
		public PlayFromSoundbankBehavior playFromSoundbankBehavior = PlayFromSoundbankBehavior.RandomNoRepeat;
		[HideInInspector]
		/// The last noise played, used to prevent duplicate repetition
		public int lastNoisePlayed = -1;
		[HideInInspector] public AudioSource activeAudioSource;

		public enum PlayFromSoundbankBehavior { RandomNoRepeat, RandomAllowRepeat, InOrder }

		/// Plays the sound. Cannot have the sound follow the object because a position is given, not a transform
		/// <returns>The sound.</returns>
		/// <param name="p">where the noise is</param>
		public AudioSource PlaySound(Vector3 p) {
			if (!backgroundMusic) {
				activeAudioSource = Noisy.PlaySound(GetSoundToPlay(), p, !is2D, loop, onePlayAtATime ? name : null, volume);
			} else {
				activeAudioSource = Noisy.PlayBackgroundMusic(GetSoundToPlay(), volume);
			}
			return activeAudioSource;
		}

		public AudioSource PlaySound(Transform t) {
			activeAudioSource = PlaySound(t.position);
			if (followsObject) { activeAudioSource.transform.SetParent(t); }
			return activeAudioSource;
		}

		public AudioClip GetSoundToPlay() { return GetSoundToPlay(ref lastNoisePlayed); }

		public AudioClip GetSoundToPlay(ref int indexNotToPlayNext) {
			if (sounds != null && sounds.Length > 0) {
				switch (playFromSoundbankBehavior) {
					case PlayFromSoundbankBehavior.RandomNoRepeat:
						indexNotToPlayNext = RandomNumberThatIsnt(0, sounds.Length, indexNotToPlayNext);
						break;
					case PlayFromSoundbankBehavior.RandomAllowRepeat:
						indexNotToPlayNext = RandomNumberThatIsnt(0, sounds.Length);
						break;
					case PlayFromSoundbankBehavior.InOrder:
						if (++indexNotToPlayNext >= sounds.Length) { indexNotToPlayNext = 0; }
						break;
				}
				return sounds[indexNotToPlayNext];
			}
			return null;
		}

		/// <summary>comparer, used to sort Noise objects into the list</summary>
		public class Comparer : IComparer<Noise> {
			public int Compare(Noise x, Noise y) { return x.name.CompareTo(y.name); }
		}
		public static Comparer compare = new Comparer();
	}

	void Awake() {
		// sort noises for faster access later
		System.Array.Sort(noises, Noise.compare);
		EnsureNoisesInGlobalSpace();
	}

	public void EnsureNoisesInGlobalSpace() {
		// add all named noises to a single static (global) listing, for easy scripted access later
		for (int i = 0; i < noises.Length; ++i) {
			if (noises[i].name == null || noises[i].name.Length == 0) { continue; }
			EnsureNoiseInGlobalSpace(noises[i], this);
		}
	}

	public void RemoveNoisesFromGlobalSpace() {
		for (int i = 0; i < noises.Length; ++i) {
			bool hasSoundsFilledOut = noises[i].sounds != null && noises[i].sounds.Length > 0;
			if (!hasSoundsFilledOut) { continue; }
			RemoveNoiseFromGlobalSpace(noises[i], this);
		}
	}

	public static void EnsureNoiseInGlobalSpace(Noise noise, Noisy src) {
		Global.NoiseSet whatToFind = new Global.NoiseSet(noise.name);
		int index = Global.allNoises.BinarySearch(whatToFind, Global.NoiseSet.compare);
		bool isAlreadyKnown = index >= 0;
		bool hasSoundsFilledOut = noise.sounds != null && noise.sounds.Length > 0;
		if (!hasSoundsFilledOut) { return; }
		// if this is the first time a noise has been added
		if (!isAlreadyKnown) {
			whatToFind.Add(noise, src);
			Global.allNoises.Insert(~index, whatToFind);
		} else {
			Global.allNoises[index].ExistingInsertLogic(noise, src);
		}
	}

	public static void RemoveNoiseFromGlobalSpace(Noise noise, Noisy src) {
		Global.NoiseSet noiseSet = new Global.NoiseSet(noise.name);
		int index = Global.allNoises.BinarySearch(noiseSet, Global.NoiseSet.compare);
		if (index < 0) { return; }
		noiseSet = Global.allNoises[index];
		noiseSet.RemoveNoisesFrom(src);
		if (noiseSet.entries.Count != 0) { return; }
		Global.allNoises.RemoveAt(index);
		bool found = s_soundPlayersByCategory.TryGetValue(noise.name, out AudioSource asrc);
		if (!found) { return; }
		if (asrc == null || !asrc.isPlaying) {
			s_soundPlayersByCategory.Remove(noise.name);
		} else {
			WhenDonePlaying whenDone = src.gameObject.AddComponent<WhenDonePlaying>();
			whenDone.asrc = asrc;
			whenDone.whatElseToDoWhenFinished = () => { s_soundPlayersByCategory.Remove(noise.name); };
		}
	}

	public class WhenDonePlaying : MonoBehaviour {
		public AudioSource asrc;
		public System.Action whatElseToDoWhenFinished;
		private void Update() {
			if (asrc != null && asrc.isPlaying) { return; }
			whatElseToDoWhenFinished.Invoke();
			Destroy(this);
		}
	}

	/// plays the first sound in the noises list
	public void DoActivateTrigger() {
		if (noises.Length == 0 || noises[0] == null) { return; }
		noises[0].PlaySound(transform.position);
	}

	/// plays the first sound in the noises list
	public void DoDeactivateTrigger() {
		if (noises.Length == 0 || noises[0] == null || noises[0].activeAudioSource == null) { return; }
		noises[0].activeAudioSource.Stop();
	}

	/// returns the Noise that was created someplace in the scene with the given name
	/// <returns>The sound.</returns>
	/// <param name="name">Name.</param>
	public static Noise GetSound(string name) {
		Global.NoiseSet searched = new Global.NoiseSet(name);
		int i = Global.allNoises.BinarySearch(searched, Global.NoiseSet.compare);
		if (i >= 0) { return Global.allNoises[i].entries[0].noise; }
		Debug.LogError("Could not find noise named \"" + name + "\". Valid names include:\n\""
			+ string.Join("\", \"", Global.AllNoiseNames) + "\"");
		return null;
	}

	void Start() {
		System.Array.ForEach(noises, InitialProcessFor);
	}

	private void InitialProcessFor(Noise n) {
		if (n.sounds == null || n.sounds.Length == 0) {
			Noise existing = GetSound(n.name);
			if (existing != null) { n.sounds = existing.sounds; }
		}
		if (n.playOnStart || n.backgroundMusic) {
			n.PlaySound(transform.position);
		}
		if (n.onCollision) {
			Noisy.OnCollisionAdvancedSettings oc = CreateHandler<OnCollisionAdvancedSettings>(n.name);
			oc.noise = n;
		}
		if (n.onTrigger) {
			Noisy.OnTriggerAdvancedSettings oc = CreateHandler<OnTriggerAdvancedSettings>(n.name);
			oc.noise = n;
		}
	}

	private void OnDestroy() {
		if (!removeNoisesWhenDestroyed) { return; }
		RemoveNoisesFromGlobalSpace();
	}

	/// Plays the named sound (as a 2D sound, full volume)
	public static AudioSource PlaySound(string name) {
		Noise n = GetSound(name);
		return (n != null) ? n.PlaySound(Vector3.zero) : null;
	}

	public static AudioSource PlaySound(string name, float volume) {
		return PlaySound(name, default, false, false, null, volume);
	}

	public static AudioSource PlaySound(string name, Vector3 p, bool is3D, bool isLooped, string soundCategory, float volume) {
		Noise n = GetSound(name);
		return (n != null) ? PlaySound(n.GetSoundToPlay(), p, is3D, isLooped, soundCategory, volume) : null;
	}

	public static AudioSource PlaySound(string name, Vector3 p) {
		Noise n = GetSound(name);
		return (n != null) ? n.PlaySound(p) : null;
	}

	/// <param name="name"></param>
	/// <param name="t">where to parent the noise (important for 3D sounds)</param>
	/// <returns></returns>
	public static AudioSource PlaySound(string name, Transform t) {
		Noise n = GetSound(name);
		return (n != null) ? n.PlaySound(t) : null;
	}

	/// <summary>
	/// every sound has it's own AudioSource. this allows overlapping sounds but only of different types.
	/// </summary>
	private static Dictionary<string, AudioSource> s_soundPlayersByCategory = new Dictionary<string, AudioSource>();

	/// Plays the sound.
	/// <returns>Component where the sound is playing from.</returns>
	/// <param name="noise">Noise. returns early if <c>null</c></param>
	/// <param name="p">P. the location to play from. If is3D is false, this parameter is pretty useless.</param>
	/// <param name="is3D">If set to false, sound plays without considering 3D-ness (full volume from anywhere).</param>
	/// <param name="isLooped">If set to <c>true</c> is looped.</param>
	/// <param name="soundCategory">If non-null, prevents multiple sounds with the same soundCategory from playing simultaneously. If null, each instance of the sound will be independent.</param>
	/// <param name="volume"></param>
	public static AudioSource PlaySound(AudioClip noise, Vector3 p, bool is3D, bool isLooped, string soundCategory, float volume) {
		if (noise == null) return null;
		AudioSource asrc = null;
		if (soundCategory != null && soundCategory.Length > 0) {
			s_soundPlayersByCategory.TryGetValue(soundCategory, out asrc);
		}
		if (asrc == null) {
			string noiseName = (soundCategory != null) ? "(" + soundCategory + ")" : "<Noise: " + noise.name + ">";
			GameObject go = new GameObject(noiseName);
			asrc = go.AddComponent<AudioSource>();
			if (soundCategory != null) {
				s_soundPlayersByCategory[soundCategory] = asrc;
			}
			asrc.transform.SetParent(Global.Instance().transform);
		} else {
			asrc.Stop();
		}
		asrc.clip = noise;
		asrc.spatialBlend = is3D ? 1 : 0;
		asrc.transform.position = p;
		if (soundCategory == null && !isLooped) {
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
	public class Global : MonoBehaviour {
		[System.Serializable] public class NoiseSet {
			public string name;

			public List<NoiseSource> entries;

			[System.Serializable] public struct NoiseSource {
				public Noise noise; public Noisy src;
				public NoiseSource(Noise noise, Noisy src) { this.noise = noise; this.src = src; }
			}

			public void Add(Noise noise, Noisy src) => Insert(-1, noise, src);

			public void Insert(int index, Noise noise, Noisy src) {
				if (entries == null) { entries = new List<NoiseSource>(); }
				if (index == -1) {
					index = entries.Count;
				}
				entries.Insert(index, new NoiseSource(noise, src));
			}

			public void ExistingInsertLogic(Noise noise, Noisy noisy) {
				int inSet = entries.FindIndex(e => e.src == noisy);
				if (inSet != -1) {
					Insert(0, noise, noisy);
				} else {
					if (entries[inSet].noise == noise) { return; } // ignore duplicate calls
					Debug.LogError($"{this} already has an entry for {noise.name}, " +
						$"replacing old entry {entries[inSet].noise.name} with new entry {noise.name}");
					entries[inSet] = new NoiseSource(noise, noisy);
				}
			}

			public int RemoveNoisesFrom(Noisy src) {
				int removed = -1;
				for (int i = entries.Count - 1; i >= 0; --i) {
					NoiseSource noiseSource = entries[i];
					if (noiseSource.src == src) {
						entries.RemoveAt(i);
						removed = i;
					}
				}
				return removed;
			}

			public NoiseSet(string name) {
				this.name = name;
				entries = new List<NoiseSource>();
			}
			/// <summary>comparer, used to sort Noise objects into the list</summary>
			public class Comparer : IComparer<NoiseSet> {
				public int Compare(NoiseSet x, NoiseSet y) { return x.name.CompareTo(y.name); }
			}
			public static Comparer compare = new Comparer();
		}
		/// All Noise objects with unique names and actual data in the 'sounds' array.
		public static List<NoiseSet> allNoises = new List<NoiseSet>();

		private static Noisy.Global instance;
		public static Noisy.Global Instance() {
			if (instance == null) {
				if ((instance = FindObjectOfType(typeof(Noisy.Global)) as Noisy.Global) == null) {
					GameObject g = new GameObject("<" + typeof(Noisy.Global).Name + ">");
					instance = g.AddComponent<Noisy.Global>();
				}
			}
			return instance;
		}
		/// <summary>local members alias showing all noises (can be seen in the inspector, static members cannot)</summary>
		[SerializeField] private List<NoiseSet> _allTheNoises;

		/// <summary>the names of all of the noises in a List</summary>
		public static List<string> AllNoiseNames { get { return allNoises.ConvertAll(set => set.name); } }

		void Start() { _allTheNoises = allNoises; }
	}

	TYPE CreateHandler<TYPE>(string nameOfNoise) where TYPE : NoisyHandler {
		if (name != null) {
			TYPE[] triggers = GetComponents<TYPE>();
			for (int i = 0; i < triggers.Length; ++i) {
				if (triggers[i].advancedNoiseOverride == nameOfNoise)
					return triggers[i];
			}
		}
		return gameObject.AddComponent<TYPE>();
	}

	void CreateOnTrigger() { CreateHandler<OnTriggerAdvancedSettings>(null); }
	void CreateOnCollision() { CreateHandler<OnCollisionAdvancedSettings>(null); }
	void CreateOnKeyPress() { CreateHandler<OnKeyPressAdvancedSettings>(null); }

	public class NoisyHandler : MonoBehaviour {
		[Tooltip("if this is set, Noise (below) will be overwritten at runtime by a Noise with this name")]
		public string advancedNoiseOverride;
		public Noise noise;
		[Tooltip("remove this handler after playing the sound once. (eg: one-time acknowledgement)")]
		public bool justOnce;
		protected void NoisyHandlerStart() {
			if (advancedNoiseOverride != null && advancedNoiseOverride.Length > 0) {
				noise = Noisy.GetSound(advancedNoiseOverride);
			}
		}
		void Start() { NoisyHandlerStart(); }
	}

	public class NoisyObjectInteractHandler : NoisyHandler {
		[Tooltip("identify which GameObjects can trigger this. (eg: only player, only certain item)")]
		public string triggersOnlyByTag;
		public bool IsValidTrigger(GameObject go) {
			return triggersOnlyByTag == null || triggersOnlyByTag.Length == 0 || go.tag == triggersOnlyByTag;
		}
	}

	public class OnTriggerAdvancedSettings : NoisyObjectInteractHandler {
		void OnTriggerEnter(Collider c) {
			if (!IsValidTrigger(c.gameObject)) return;
			if (noise.followsObject) {
				noise.PlaySound(transform);
				if (justOnce) Destroy(this);
			} else {
				noise.PlaySound(c.transform.position);
			}
		}
	}

	public class OnCollisionAdvancedSettings : NoisyObjectInteractHandler {
		void OnCollisionEnter(Collision c) {
			if (!IsValidTrigger(c.gameObject)) return;
			if (noise.followsObject) {
				noise.PlaySound(transform);
				if (justOnce) Destroy(this);
			} else {
				noise.PlaySound(c.contacts[0].point);
			}
		}
	}

	public class OnKeyPressAdvancedSettings : NoisyHandler {
		public KeyCode key = KeyCode.None;
		public enum KeyEvent { press, release, hold };
		public KeyEvent eventType = KeyEvent.press;
		public bool IsTriggered() {
			switch (eventType) {
				case KeyEvent.press: return Input.GetKeyDown(key);
				case KeyEvent.release: return Input.GetKeyUp(key);
				case KeyEvent.hold: return Input.GetKey(key);
			}
			return false;
		}
		void Start() {
			NoisyHandlerStart();
			if (noise.sounds == null || noise.sounds.Length == 0) {
				Noisy n = GetComponent<Noisy>();
				if (n != null && n.noises != null && n.noises.Length > 0) {
					this.noise = n.noises[0];
				}
			}
		}
		void Update() {
			if (IsTriggered()) {
				noise.PlaySound(transform);
				if (justOnce) { enabled = false; }
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
			if (n != null && n.activeAudioSource != null) { n.activeAudioSource.volume = n.volume; }
		}
	}
#endif
}
