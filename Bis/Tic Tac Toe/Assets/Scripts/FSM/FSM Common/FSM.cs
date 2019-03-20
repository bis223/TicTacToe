using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Flags]
public enum FiniteStateList
{
	// NOTE ::: IMP ..IF new screens are added, then make sure it is set as power of 2 flag.
	// IMP....Also, name the gameobject for FSM state of new states same as enum defined here
	MainMenuState				=  1,
	GamePlayState				=  2,
	GameOverState 				=  4
}

/// <summary>
/// 	Finite State Machine class which handles transitions to different screens and maintaining custom activity stack.
/// 	By state, we mean screens.  Need to follow this approach as we want everything to be done in a single scene 
/// 	so that scene transition problem is not encountered i.e. time lag to load a scene. Also, screen transition animation
/// 	could be easily integrated.
/// </summary>
[System.Serializable]
public sealed class FSM : MonoBehaviour
{

	public event System.Action 		OnStateChangeEvent;	// Event that is fired when any state change happens.

	public 		 Stack<FiniteState> history       = new Stack<FiniteState>();	// Activity stack that maintains screen flow, so that it can be tracked when back button is pressed.
	
	const 		 int 			    MAX_STATES    = 30; //max bit mask will allow.
	
	[HideInInspector][SerializeField] 
				List<FiniteState> 	_states       = new List<FiniteState>(256);
	[HideInInspector][SerializeField] 
				List<string> 		_stateKeys 	  = new List<string>(256);	
	[HideInInspector][SerializeField]
				int 				_defaultState = 0;

	public event System.Action 	OnFSMInitializedEvent;	// Event that is fired when FSM is initialised with all its states.


	/// <summary>
	/// Gets the default state.
	/// </summary>
	/// <value>
	/// The default state.
	/// </value>
	public FiniteState defaultState
	{
		get
		{
			FiniteState s = null;
			
			try
			{
				s = _states[_defaultState];
			}
			catch(System.Exception)
			{
				Debug.LogError("Caught exception while clearing. This is harmless.");
			}
			
			return s;
		}
	}
	
	FiniteState _currState;
	/// <summary>
	/// Gets the current state.
	/// </summary>
	/// <value>
	/// The current state.
	/// </value>
	public FiniteState CurrentState
	{
		get
		{
			return _currState;
		}
	}
	
	/// <summary>
	/// Should the default state Initialize on Start?.
	/// </summary>
	public bool autoInit = true;
	
	/// <summary>
	/// Gets the with the specified key.
	/// If there is no state with the name matching the key, a warning is logged and will return null.
	/// </summary>
	/// <param name='key'>
	/// Key.
	/// </param>
	public FiniteState this[string key]
	{
		get
		{
			if(!_stateKeys.Contains(key))
			{
				if(Application.isPlaying) 
					Debug.LogError("No state with the name of \"" + key + "\" exists!");
				return null;
			}
			int index = _stateKeys.IndexOf(key);
			return _states[index];		// Return the specific Finite state machine for the index retreived by the key passed.
		}
	}
	
	/// <summary>
	/// Gets the at the specified index.
	/// </summary>
	/// <param name='index'>
	/// Index.
	/// </param>
	public FiniteState this[int index]
	{
		get
		{
			//if(_states.Count >= index) return _states[_states.Count-1];
			FiniteState s = null;
			
			try
			{
				s = _states[index];
			}
			catch(System.Exception)
			{
				Debug.LogError("Caught exception while clearing. This is harmless.");
			}
			return s;
		}
	}
	
	public int Length{get{return _states.Count;}}
	
	/// <summary>
	/// Unity method. Sets the default state to be the current state.
	/// </summary>
	void Awake()
	{				

	}
	
	/// <summary>
	/// Unity method. 
	/// </summary>
	void Start()
	{
		InitializeStates();
		_currState = defaultState;
		if(_states.Count == 0)
		{
			Debug.LogError("No states!");
			return;
		}

		if(autoInit) 
			_currState.Init();
	}
			

	void _InitHandler(){}

	void _DeInitHandler(){}
	
	/// <summary>
	/// Changes the state. Produces a warning if the state you are moving to does not comply with the mask of the current state, and then does nothing.
	/// </summary>
	/// <param name='stateName'>
	/// State name.
	/// </param>
	public void ChangeState(FiniteStateList stateName)
	{
		if (_currState != null) 
		{
			FiniteState nextState = this[stateName.ToString()];
			if(_currState.CanTransitionToState(nextState) || 
				_currState == nextState ||
				(_currState == this[FiniteStateList.GameOverState.ToString()] && nextState == this[FiniteStateList.MainMenuState.ToString()]))
			{
				_currState.DeInit();
				_currState = this[stateName.ToString()];
				_currState.Init();
				history.Push( _currState );
				if (OnStateChangeEvent != null) 
					OnStateChangeEvent();
			}
			else
			{
				Debug.LogError("State \"" + _currState.name + "\" cannot transition to state \"" + nextState.name + "\". You should check your logic.");
			}
		}
		else
		{
			_currState = this[stateName.ToString()];
			_currState.Init();
			history.Push( _currState );
			history.Push( _currState );
			if (OnStateChangeEvent != null) 
				OnStateChangeEvent();
		}
	}
	
	/// <summary>
	/// Makes the state at 'indexOf' default one.
	/// </summary>
	/// <param name='indexOf'>
	/// Index of.
	/// </param>
	public void MakeDefault(int indexOf)
	{
		_defaultState = indexOf;
	}

	/// <summary>
	/// 	Goes the previous state which was saved in the activity stack.
	/// </summary>
	public void GoToPreviousState()
	{
		//testing this out.. something like this should work.
		history.Pop();
		FiniteState lastState = history.Peek();
		ChangeState((FiniteStateList) System.Enum.Parse(typeof(FiniteStateList), lastState.name));
	}
		

	/// <summary>
	/// Adds the state.
	/// </summary>
	/// <param name='stateName'>
	/// State name.
	/// </param>
	/// <exception cref='System.Exception'>
	/// Throws an error if the maximum number of states (31) is exceeded.
	/// </exception>
	public void InitializeStates()
	{
		if(_states.Count == 0) 
			_defaultState = 0;

		foreach(FiniteStateList stateList in System.Enum.GetValues(typeof(FiniteStateList)))
		{
			FiniteState s = new FiniteState(stateList.ToString(), (int) stateList);
			_states.Add(s);
			_stateKeys.Add(stateList.ToString());
		}
		if(OnFSMInitializedEvent != null)
			OnFSMInitializedEvent();
	}	
}
