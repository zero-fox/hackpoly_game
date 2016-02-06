
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BladeCast
{
	public class BCListener : MonoBehaviour
	{
		[System.Serializable]
		public class Listener
		{
			public string 				m_MessageType;
			public int					m_ControllerIndex;
			public string 				m_Handler;

			public Listener (string messageType, int controlIndex, string handler)
			{
				m_MessageType = messageType;
				m_ControllerIndex = controlIndex;
				m_Handler = handler;
			}
		}

		public List<Listener> 		m_Listeners = new List<Listener> ();
		private List<Listener> 		m_ListenersSafe = new List<Listener> ();
		bool 						m_Dirty = false;
		private BCMessenger			m_BCMessenger;
		
		void Start ()
		{
			m_BCMessenger = BCMessenger.Instance;
			if (m_BCMessenger != null)
				m_BCMessenger.RegisterListener (this);
			else
				Debug.LogError ("BCMessenger is missing from scene... can't register listener");
		}

		void OnDestroy ()
		{
			if (m_BCMessenger != null)
				m_BCMessenger.RemoveListener (this);
		}
		/// <summary>
		/// Registers a listener.
		/// </summary>
		/// <param name="listener">Listener -- instance of a listener 
		public Listener Add (Listener listener)
		{
			m_Listeners.Add (listener);
			m_Dirty = true;
			return listener;
		}

		public Listener Add (string messageType, int controlIndex, string forwardToMethod)
		{
			Listener listener = new Listener (messageType, controlIndex, forwardToMethod);
			Add (listener);
			m_Dirty = true;
			return listener;
		}

		/// <summary>
		/// Removes the listener.
		/// </summary>
		/// <returns><c>true</c>, if listener was removed, <c>false</c> otherwise.</returns>
		/// <param name="listener">Listener.</param>
		public bool Remove (Listener listener)
		{
			bool doesContain = m_Listeners.Contains (listener);
			if (doesContain) {
				m_Listeners.Remove (listener);
				m_Dirty = true;
			}
			return doesContain;
		}

		public void OnGameMessage (ControllerMessage msg)
		{
			UpdateSafeList ();
			foreach (Listener listener in m_ListenersSafe.FindAll(bcListener => ((msg.ControllerDest < 0) || (bcListener.m_ControllerIndex < 0) ||
			                                                                 	(bcListener.m_ControllerIndex == msg.ControllerDest)) &&
			                                                  				 	((bcListener.m_MessageType == "*") || (bcListener.m_MessageType == msg.Name)) )) {    
				this.gameObject.BroadcastMessage (listener.m_Handler, msg, SendMessageOptions.RequireReceiver);
			}
			UpdateSafeList ();
		}

		public void UpdateSafeList ()
		{
			if (m_Dirty) {
				m_ListenersSafe = m_Listeners.ToList ();
				m_Dirty = false;
			}
		}
	}
}
