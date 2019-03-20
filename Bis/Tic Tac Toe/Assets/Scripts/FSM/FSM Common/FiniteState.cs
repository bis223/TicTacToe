using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FiniteState : object
{	
	/// <summary>
	/// Event depicting that this <see cref="BullyFiniteState"/> has been initialized. Occurs when Init() is called.
	/// </summary>
	public event System.Action OnInitialize;
	/// <summary>
	/// Event depicting that this <see cref="BullyFiniteState"/> has been DeInitialized. Occurs when DeInit() is called.
	/// </summary>
	public event System.Action OnDeInitialize;
	
	bool _active = false;
	/// <summary>
	/// Gets a value indicating whether this <see cref="BullyFiniteState"/> is active.
	/// </summary>
	/// <value>
	/// <c>true</c> if is active; otherwise, <c>false</c>.
	/// </value>
	public bool isActive{get{return _active;}}
	
	[SerializeField] int _id = 0;
	/// <summary>
	/// Gets the zero-based unique identifier.
	/// </summary>
	/// <value>
	/// The identifier.
	/// </value>
	public int id{get{return _id;}}
	/// <summary>
	/// Gets the flag formatted identifier for bitmasking.
	/// </summary>
	/// <value>
	/// The ID as a power-of-two.
	/// </value>
	internal int __maskID{get{return (int) Mathf.Pow(2,_id);}}
	
	[SerializeField] string _name;
	/// <summary>
	/// Gets the state's name.
	/// </summary>
	/// <value>
	/// The name of this state.
	/// </value>
	public string name{get{return _name;}}
	[SerializeField] int _stateMask = -1;
	/// <summary>
	/// Gets or sets the state mask. This is to prevent which states a state can move to.
	/// </summary>
	/// <value>
	/// The state mask.
	/// </value>
	public int stateMask
	{
		get
		{
			return _stateMask;
		}
		set
		{
			_stateMask = value;
		}
	}
	
	public FiniteState(string stateName, int id)
	{
		_name = stateName;
		_id = id;
	}
	
	/// <summary>
	/// Initialize this state.
	/// </summary>
	public void Init()
	{
		_active = true;
		_Init();
		if(OnInitialize != null)
			OnInitialize();
		else 
			Debug.LogWarning("State \"" + name + "\" initialization event has no receiver!");
	}
	
	internal virtual void _Init(){}
	
	/// <summary>
	/// DeInitialize this state.
	/// </summary>
	public void DeInit()
	{
		_active = false;
		_DeInit();
		if(OnDeInitialize != null) 
			OnDeInitialize();
		else 
			Debug.LogWarning("State \"" + name + "\" DeInitialization event has no receiver!");
	}
	
	internal virtual void _DeInit(){}
	
	#region Event Subscription
	
	/// <summary>
	/// Subscribe the specified initializeHandler and DeInitializeHandler. Just a short-hand for assigning actions.
	/// </summary>
	/// <param name='initializeHandler'>
	/// Initialize handler.
	/// </param>
	/// <param name='DeInitializeHandler'>
	/// DeInitialize handler.
	/// </param>
	
	public void Subscribe(System.Action initializeHandler, System.Action DeInitializeHandler)
	{
		OnInitialize += initializeHandler;
		OnDeInitialize += DeInitializeHandler;
	}
	
	/// <summary>
	/// Unsubscribe the specified initializeHandler and DeInitializeHandler.
	/// </summary>
	/// <param name='initializeHandler'>
	/// Initialize handler.
	/// </param>
	/// <param name='DeInitializeHandler'>
	/// DeInitialize handler.
	/// </param>
	public void Unsubscribe(System.Action initializeHandler, System.Action DeInitializeHandler)
	{
		OnInitialize -= initializeHandler;
		OnDeInitialize -= DeInitializeHandler;
	}
	
	#endregion // Event Subscription
	
	/// <summary>
	/// Determines whether this instance can transition to the specified state.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this state can transition to the specified state; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='otherState'>
	/// The state to compare against.
	/// </param>
	public bool CanTransitionToState(FiniteState otherState)
	{
		return CanTransitionToState(otherState.__maskID);
	}
	
	internal bool CanTransitionToState(int otherID)
	{
		//BullyDebug.Log("mask: " + _stateMask + " & other ID:" + otherID + ", " + (_stateMask & otherID)); 
		return ((_stateMask & otherID) != 0);
	}
	
	#if UNITY_EDITOR
	
	/// <summary>
	/// [Editor-Only] INTERNAL: Resets the ID.
	/// </summary>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	internal void ResetID(int id)
	{
		_id = id;
	}
	
	#region Subscription Diagnostics
	
	/// <summary>
	/// [Editor-Only] Gets the number of OnInitialize subscribers for this state.
	/// </summary>
	/// <value>
	/// The number of OnInitialize subscribers for this state.
	/// </value>
	public int initSubscribers
	{
		get
		{
			return initSubscribersArray.Length;
		}
	}


	/// <summary>
	/// [Editor-Only] Gets the System.Delegate subscribers array for OnInitialize.
	/// </summary>
	/// <value>
	/// The OnInitialize subscribers array.
	/// </value>
	public System.Delegate[] initSubscribersArray
	{
		get
		{
			if(OnInitialize == null) 
				return new System.Delegate[0]{};
			return OnInitialize.GetInvocationList();
		}
	}
	/// <summary>
	/// [Editor-Only] Gets the OnInitialize subscriber names.
	/// </summary>
	/// <value>
	/// The OnInitialize subscriber names.
	/// </value>
	public string[] initSubscriberNames
	{
		get
		{
			List<string> s = new List<string>();
			
			foreach(System.Delegate d in initSubscribersArray)
			{
				if(d.Target is MonoBehaviour)
				{
					MonoBehaviour mb = (MonoBehaviour) d.Target;
					s.Add(mb.gameObject.name +"::"+mb.GetType());
				}
			}
			
			return s.ToArray();
		}
	}
	
	/// <summary>
	/// [Editor-Only] Gets the number of OnDeInitialize subscribers for this state.
	/// </summary>
	/// <value>
	/// The number of OnDeInitialize subscribers for this state.
	/// </value>
	public int DeInitSubscribers
	{
		get
		{
			return DeInitSubscribersArray.Length;
		}
	}


	/// <summary>
	/// [Editor-Only] Gets the System.Delegate OnDeInitialize subscribers array.
	/// </summary>
	/// <value>
	/// The OnDeInitialize subscribers array.
	/// </value>
	public System.Delegate[] DeInitSubscribersArray
	{
		get
		{
			if(OnDeInitialize == null) return new System.Delegate[0]{};
			return OnDeInitialize.GetInvocationList();
		}
	}


	/// <summary>
	/// [Editor-Only] Gets the OnDeInitialize subscriber names array.
	/// </summary>
	/// <value>
	/// The OnDeInitialize subscribers names array.
	/// </value>
	public string[] DeInitSubscriberNames
	{
		get
		{
			List<string> s = new List<string>();
			
			foreach(System.Delegate d in DeInitSubscribersArray)
			{
				//BullyDebug.Log(d.Target.GetType() + "," + (d.Target is MonoBehaviour));
				if(d.Target is MonoBehaviour)
				{
					MonoBehaviour mb = (MonoBehaviour) d.Target;
					s.Add(mb.gameObject.name +"::"+mb.GetType());
				}
			}
			
			return s.ToArray();
		}
	}
	
	/// <summary>
	/// [Editor Only] Rename the state.
	/// </summary>
	/// <param name='newName'>
	/// New name.
	/// </param>
	public void Rename(string newName)
	{
		_name = newName;	
	}
	
	#endregion // Subscription Diagnostics
	#endif
}
