
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/

using UnityEngine;

/// <summary>
/// Mono singleton. From http://wiki.unity3d.com/index.php?title=Singleton#Generic_Based_Singleton_for_MonoBehaviours
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static T m_Instance = null;
	public static T Instance {
		get {
			if (m_Instance) {
				return m_Instance;
			} else {
				Debug.LogError ("MonoSingleton: Problem finding instance of: " + typeof(T).ToString ());
			}
			
			return m_Instance;
		}
	}
	
	/// <summary>
	/// Awake function. Override when necessary and call base.Awake() first.
	/// </summary>
	protected virtual void Awake ()
	{
		if (m_Instance == null) {
			m_Instance = this as T;
			DontDestroyOnLoad (gameObject);
		}
	}
	
	/// <summary>
	/// Clear the reference when the application quits. Override when necessary and call base.OnApplicationQuit() last.
	/// </summary>
	protected virtual void OnApplicationQuit ()
	{
		m_Instance = null;
	}
}
