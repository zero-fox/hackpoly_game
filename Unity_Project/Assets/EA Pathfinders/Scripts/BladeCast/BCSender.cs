
/*
Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.
 
This software is solely licensed pursuant to the Hackathon License Agreement,
Available at:  [URL to Hackathon License Agreement].
All other use is strictly prohibited.  
*/


using UnityEngine;

namespace BladeCast
{
	public class BCSender : MonoBehaviour
	{
		/// <summary>
		/// "controllerDest" of "m" is the target controller... 0 (for unity), 1, 2, 3...  (if -1 then for ALL controllers)
		/// "controllerIndex" of l is the receiving controller.  Same as dest...  -1 is wildcard for all messages
		/// </summary>
		public void SendToListeners(ControllerMessage msg)
		{
			BCMessenger.Instance.SendToListeners (msg);
		}

	    /// <summary>
	    /// Sends to listeners -- simple form.  Message only contains "name"
	    /// </summary>
	    public void SendToListeners(string messageName, int cd)
	    {
	        JSONObject msgOut = new JSONObject (JSONObject.Type.OBJECT);
	        SendToListeners (new ControllerMessage (0, cd, messageName, msgOut));
	    }

	    /// <summary>
	    /// Sends to listeners -- simple form.  Same, but with a single string field
	    /// </summary>
	    public void SendToListeners(string messageName, string field1Name, string field1, int cd)
	    {
	        JSONObject msg = new JSONObject (JSONObject.Type.OBJECT);
	        msg.AddField (field1Name, field1);    
	        SendToListeners (new ControllerMessage (0, cd, messageName, msg));
	    }

	    /// <summary>
	    /// Sends to listeners -- simple form.  Same, but with a single bool field
	    /// </summary>
	    public void SendToListeners(string messageName, string field1Name, bool field1, int cd)
	    {
	        JSONObject msg = new JSONObject (JSONObject.Type.OBJECT);
	        msg.AddField (field1Name, field1);    
	        SendToListeners (new ControllerMessage (0, cd, messageName, msg));
	    }
	 
	    /// <summary>
	    /// Sends to listeners -- simple form.  Same, but with a single int field
	    /// </summary>
	    public void SendToListeners(string messageName, string field1Name, int field1, int cd)
	    {
	        JSONObject msg = new JSONObject (JSONObject.Type.OBJECT);
	        msg.AddField (field1Name, field1);    
	        SendToListeners (new ControllerMessage (0, cd, messageName, msg));
	    }

	    /// <summary>
	    /// Sends to listeners -- simple form.  Same, but with a single float field
	    /// </summary>
	    public void SendToListeners(string messageName, string field1Name, float field1, int cd)
	    {
	        JSONObject msg = new JSONObject (JSONObject.Type.OBJECT);
	        msg.AddField (field1Name, field1);    
	        SendToListeners (new ControllerMessage (0, cd, messageName, msg));
	    }
	}
}
