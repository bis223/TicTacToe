using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class StateBase : MonoBehaviour 
{
	
	#region Public Variables
	public FSM fsm;

	[Tooltip("Imp. - needs to be same as values in FiniteStateList")]
	public string stateID;

	[HideInInspector]
	public bool isRunning; //This is not meant to be set in the inspector. Instead use the default state
	#endregion
	
	#region Private Variables
	protected List<GameObject>  activePanels;
	protected GameObject  		lastPanel;
	#endregion
	
	#region Init
	void Awake()	
	{
	}
	#endregion

	protected virtual void OnEnable()
	{
		fsm.OnFSMInitializedEvent	+= HandleOnFSMInitializedEvent;
	}
	
	protected virtual void OnDisable()
	{
		fsm.OnFSMInitializedEvent	-= HandleOnFSMInitializedEvent;
	}

	void HandleOnFSMInitializedEvent ()
	{
		//auto register state id:
		if ( stateID == "" )
		{

			stateID = gameObject.name;
		}
		//auto register controlling fsm:
		if ( fsm == null ) 
		{

			fsm = transform.parent.GetComponent<FSM>();

		}

		fsm[stateID].Subscribe( Init, DeInit ); 
	}

			
	#region Show Transitions
	protected virtual void Init()
	{
		isRunning = true;
		activePanels = new List<GameObject>();
	}
	
	protected virtual void DeInit()
	{		
		isRunning = false;
	}
	#endregion
	
	#region Protected Methods
    protected void AddPanel(GameObject panel)
    {
        AddPanel(panel, false);
    }

	protected void AddPanel( GameObject panel, bool animate )
	{
		lastPanel = panel;
		activePanels.Add( panel );	
		panel.SetActive(true);
	}
	
	protected void RemovePanel( GameObject panel )
	{
		RemovePanel( panel, false );
	}
	
	protected void RemovePanel( GameObject panel, bool animate )
	{
		if (!activePanels.Contains(panel)) return;
		activePanels.Remove( panel );
		panel.SetActive(false);
	}
	
	protected void RemoveLastPanel( bool animate )
	{
		activePanels.Remove( lastPanel );
		lastPanel.SetActive(false);
		lastPanel = null;
	}
	
	protected void RemoveAllPanels(bool animate = false)
	{
		foreach (GameObject item in activePanels) {
			item.SetActive(false);
		}
		activePanels = new List<GameObject>();
	}
	
	protected void DeactivateAllPanels()
	{
		foreach (GameObject item in activePanels) {
			item.SetActive(false);
		}
	}
	
	protected void ActivateAllPanels()
	{
		foreach (GameObject item in activePanels) {
			item.SetActive(false);
		}
	}
	#endregion
}