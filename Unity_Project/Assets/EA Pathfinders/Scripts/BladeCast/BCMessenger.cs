
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
	/// <summary>
	/// A message to or from a (game) controller (ie: html5) or any game object that subscribes to it.
	/// </summary>
	public class ControllerMessage
	{
		public int 			ControllerSource 	{get; set;}				// the controller index of the source (0 = game client)
		public int 			ControllerDest		{get; set;}				// the controller index of the dest (0 = game client)
		public string 		Name   				{get; set;}
		public JSONObject  	Payload {get; set;}

		/// <summary>
		/// A controller Message. 
		/// cs -- control source (who is sending the message)  (0 = unity, 1 = cntrl #1... etc)
		/// cd -- control destination (target of message).  (0 = unity (local), 1 = cntrl #1... etc,  -1 is to all)
		/// mn -- message name
		/// payload - the contents of the message (can be null).
		/// </summary>
		public ControllerMessage(int cs, int cd, string mn, JSONObject payload)
		{
			ControllerSource = cs;
			ControllerDest = cd;
			Name = mn;
			Payload = payload;
		}
	}
	
	public class BCMessenger : MonoSingleton<BCMessenger>
	{
		private List<BCListener> m_BCListeners = new List<BCListener>();
		private List<BCListener> m_BCListenersSafe = new List<BCListener>();
		private bool			 m_Dirty = false;
		private Queue 			 m_MsgQueue;
		private Queue 			 m_SyncedMsgQueue;

		void Start()
		{
			// setup communications with controllers...
			// Set up the message queues
			m_MsgQueue = new Queue ();
			m_SyncedMsgQueue = Queue.Synchronized (m_MsgQueue);

			// register message handler...
			BCLibProvider.Instance.BladeCast_Game_RegisterMessageCB (OnGameMessage);
		}

		/// <summary>
		/// Messages are added to the queue in BCConnection. Here, we check if there are any messages
		/// and process them in the main thread.
		/// Messages need to be in the following format:
		/// {"type":"go_action", "index": 1}
		/// index -- the index of the controller that is sending the message (0 = unity, 1 = ctrl 1...)
		/// type -- the type of message (
		/// </summary>
		void Update ()
		{
			// We check the count here so that we don't lock the queue just to find out it's empty.
			while (m_SyncedMsgQueue.Count > 0)
			{
				lock (m_SyncedMsgQueue)
				{
					if (m_SyncedMsgQueue.Count > 0)
					{
						JSONObject json = m_SyncedMsgQueue.Dequeue () as JSONObject;
						
						if (json != null) 
						{
							bool validate = true;
							validate &= BCLibProvider.Instance.FieldCheck(json, "index");
							validate &= BCLibProvider.Instance.FieldCheck(json, "type");
							if(validate)
								SendToListeners (new ControllerMessage (json ["index"].i, 0, json ["type"].str, json));
						} 
						else 
						{
							Debug.LogError ("Non JSON Object as Messge in BCInput");
						}
					}
				}
			}
		}

		/// <summary>
		/// Registers a listener.
		/// </summary>
		/// <param name="listener">Listener -- instance of a listener 
		public BCListener RegisterListener(BCListener bcListener)
		{
			m_BCListeners.Add (bcListener);
			m_Dirty = true;
			return bcListener;
		}

		/// <summary>
		/// Removes the listener.
		/// </summary>
		/// <returns><c>true</c>, if listener was removed, <c>false</c> otherwise.</returns>
		/// <param name="listener">Listener.</param>
		public bool RemoveListener(BCListener bcListener)
		{
			bool doesContain = m_BCListeners.Contains (bcListener);
			if (doesContain) 
			{
				m_BCListeners.Remove (bcListener);
				m_Dirty = true;
			}
			return doesContain;
		}

		/// <summary>
		/// "controllerDest" of "m" is the target controller... 0 (for unity), 1, 2, 3...  (if -1 then for ALL controllers)
		/// "controllerIndex" of l is the receiving controller.  Same as dest...  -1 is wildcard for all messages
		/// </summary>
		public void SendToListeners(ControllerMessage msg)
		{
			List<BCListener> deadListeners = new List<BCListener>();
			UpdateSafeList ();
			foreach (BCListener bcListener in m_BCListenersSafe) 
			{
				if (bcListener == null)
					deadListeners.Add(bcListener);
				else
					bcListener.OnGameMessage(msg);

				// send to attached controllers
				RouteControllerMessages(msg);
			}

			foreach (BCListener bcListener in deadListeners)
				RemoveListener (bcListener);

			UpdateSafeList ();
		}

		/// <summary>
		/// Forwards messages to the controllers... listens for any "controller" messages (msgs > 0)
		/// and sends them.
		/// </summary>
		private void RouteControllerMessages(ControllerMessage msg)
		{
			// just messages for controllers... (-1 dest is for ALL)
			if ((msg.ControllerDest > 0) || (msg.ControllerDest < 0))
			{
				JSONObject json = msg.Payload;
				json.AddField("type", msg.Name);
				json.AddField ("controller_dest", msg.ControllerDest.ToString());
				//Debug.Log ("Game Message Sent: " + json.ToString ());
				BCLibProvider.Instance.BladeCast_Game_SendMessage (json);
			}
		}

		// Handle LibProviderGame callbacks (inbound messages from controllers)
		private void OnGameMessage(System.IntPtr token, JSONObject json, int controllerIndex)
		{
			lock (m_SyncedMsgQueue)
			{
				// adding the controller index into payload!
				json.AddField ("index", controllerIndex);
				m_SyncedMsgQueue.Enqueue (json);
			}
			//Debug.Log("Game Message Received: " + contents);
		}

		private void UpdateSafeList()
		{
			if(m_Dirty)
			{
				m_BCListenersSafe = m_BCListeners.ToList ();
				m_Dirty = false;
			}
		}
	}
}
